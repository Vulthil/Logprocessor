using System;
using System.Linq;
using System.Reflection;
using GreenPipes;
using HttpEnricher.Config;
using HttpEnricher.MassTransit;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoreLinq.Extensions;
using Serilog.Context;
using Services.Shared.Dummy;
using Services.Shared.Models;
using Task = System.Threading.Tasks.Task;

namespace HttpEnricher
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration, Action<LogConfig> configure = null)
        {
            if (configure == null)
            {
                services.Configure<LogConfig>(configuration.GetSection("LogConfig"));
            }
            else
            {
                services.Configure(configure);
            }
            services.AddHttpContextAccessor();
            services.AddSingleton<IConversationIdProvider, ConversationIdProvider>();
            services.AddScoped(typeof(ISender), typeof(BaseSender));
            services.AddMassTransit(x =>
            {
                var requestTypes = Assembly.GetAssembly(typeof(Dummy))?.ExportedTypes.Where(t =>
                    t.IsInterface && t.Namespace == "Services.Shared.Messages");

                requestTypes?.ForEach(p => x.AddRequestClient(p));

                var consumers = Assembly.GetEntryAssembly()?.ExportedTypes.Where(t =>
                    t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(BaseConsumer)));

                consumers?.ForEach(p => x.AddConsumer(p));
                x.UsingRabbitMq((context, cfg) =>
                {
                    //cfg.ConnectReceiveObserver(context.GetRequiredService<IReceiveObserver>());
                    cfg.UsePublishFilter(typeof(PublishFilter<>), context);
                    cfg.UseSendFilter(typeof(SendFilter<>), context);
                    cfg.UseConsumeFilter(typeof(ConsumeFilter<>), context);
                    cfg.UseHealthCheck(context);

                    cfg.Host(new Uri($"rabbitmq://{configuration["RABBITMQ_HOST"]}/"), h =>
                    {
                        h.Username(configuration["RABBITMQ_DEFAULT_USER"]);
                        h.Password(configuration["RABBITMQ_DEFAULT_PASS"]);
                    });
                    cfg.ConfigureSend(s => s.UseSendExecute(q =>
                    {
                        q.ConversationId = context.GetRequiredService<IConversationIdProvider>().GetConversationId() ?? q.ConversationId;
                    }));
                    cfg.ConfigurePublish(s => s.UseExecute(q =>
                    {
                        q.ConversationId = context.GetRequiredService<IConversationIdProvider>().GetConversationId() ?? q.ConversationId;
                    }));


                    consumers?.ForEach(p =>
                    {
                        var type = p.BaseType?.GetGenericArguments().FirstOrDefault();
                        var queueName = type?.Name ??
                            (new TemporaryEndpointDefinition()).GetEndpointName(DefaultEndpointNameFormatter.Instance);
                        cfg.ReceiveEndpoint(queueName, ep =>
                        {
                            ep.PrefetchCount = 16;
                            ep.UseMessageRetry(r => r.Interval(2, 100));
                            ep.UseMessageScope(context);
                            ep.UseInMemoryOutbox();
                            ep.ConfigureConsumer(context, p);
                        });
                    });

                });
            });
            services.AddMassTransitHostedService();
            return services;
        }
    }
    public class PublishFilter<T> : IFilter<PublishContext<T>> where T : class
    {
        private readonly ILogger<PublishFilter<T>> _logger;
        private readonly IConversationIdProvider _conversationIdProvider;
        private readonly LogConfig _config;

        public PublishFilter(ILogger<PublishFilter<T>> logger, IOptions<LogConfig> options,
            IConversationIdProvider conversationIdProvider)
        {
            _logger = logger;
            _conversationIdProvider = conversationIdProvider;
            _config = options.Value;
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            if (_config.ShouldLog)
            {
                var logMessage = new LogMessage()
                {
                    SessionId = (_conversationIdProvider.GetConversationId() ?? context.ConversationId).ToString(),
                    Direction = Direction.Outbound,
                    Destination = "", // TODO: Mby not needed ?
                    TargetApi = context.Message.GetType().Name,
                    Origin = _config.Name,
                };
                using (LogContext.PushProperty("SessionId", logMessage.SessionId))
                {
                    _logger.LogInformation("{LogMessage}", logMessage);
                }
            }

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
    public class SendFilter<T> : IFilter<SendContext<T>> where T : class
    {
        private readonly ILogger<SendFilter<T>> _logger;
        private readonly IConversationIdProvider _conversationIdProvider;
        private readonly LogConfig _config;

        public SendFilter(ILogger<SendFilter<T>> logger, IOptions<LogConfig> options,
            IConversationIdProvider conversationIdProvider)
        {
            _logger = logger;
            _conversationIdProvider = conversationIdProvider;
            _config = options.Value;
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            if (_config.ShouldLog)
            {
                var logMessage = new LogMessage()
                {
                    SessionId = (_conversationIdProvider.GetConversationId() ?? context.ConversationId).ToString(),
                    Direction = Direction.Outbound,
                    Destination = "", // TODO: Mby not needed ?
                    TargetApi = context.Message.GetType().Name,
                    //Payload = context.Message.GetPayloadTypes(),
                    Origin = _config.Name,
                };
                using (LogContext.PushProperty("SessionId", logMessage.SessionId))
                {
                    _logger.LogInformation("{LogMessage}", logMessage);
                }
            }

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }

    public class ConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private readonly ILogger<ConsumeFilter<T>> _logger;
        private readonly LogConfig _config;

        public ConsumeFilter(ILogger<ConsumeFilter<T>> logger, IOptions<LogConfig> options)
        {
            _logger = logger;
            _config = options.Value;
        }
        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            if (_config.ShouldLog)
            {
                var logMessage = new LogMessage()
                {
                    SessionId = context.ConversationId.ToString(),
                    Destination = _config.Name,
                    TargetApi = context.Message.GetType().Name,
                    Direction = Direction.Inbound,
                    Origin = context.Host.ProcessName // TODO: Mby not needed ?
                };

                using (LogContext.PushProperty("SessionId", logMessage.SessionId))
                {
                    _logger.LogInformation("{LogMessage}", logMessage);
                }
            }

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
