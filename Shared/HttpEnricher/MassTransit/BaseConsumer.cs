using MassTransit;
using System.Threading.Tasks;
using HttpEnricher.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;
using Services.Shared.Messages;
using Services.Shared.Models;

namespace HttpEnricher.MassTransit
{
    public abstract class BaseConsumer
    {
        protected readonly ILogger<BaseConsumer> Logger;
        protected readonly IOptions<LogConfig> Options;
        protected readonly LogConfig Config;
        protected readonly ISender Sender;
        private readonly IConversationIdProvider _conversationIdProvider;

        protected BaseConsumer(ILogger<BaseConsumer> logger, IOptions<LogConfig> options,
            ISender sender,
            IConversationIdProvider conversationIdProvider)
        {
            Logger = logger;
            Options = options;
            Sender = sender;
            _conversationIdProvider = conversationIdProvider;
            Config = Options.Value;
        }

        public void LogExternalMessage(string destination, string targetApi, ConsumeContext context)
        {
            if (Config.ShouldLog)
            {
                var sessionId = (_conversationIdProvider.GetConversationId() ?? context.ConversationId).ToString();
                using (LogContext.PushProperty("SessionId", sessionId))
                {
                    var logMessage = new LogMessage()
                    {
                        SessionId = sessionId,
                        Direction = Direction.Outbound,
                        Destination = destination,
                        TargetApi = targetApi,
                        Origin = Config.Name,
                    };
                    Logger.LogInformation("{LogMessage}", logMessage);
                }
            }
        }
    }

    public abstract class BaseConsumer<T> : BaseConsumer, IConsumer<T> where T : class
        {
            private ConsumeContext<T> _context;

            protected BaseConsumer(ILogger<BaseConsumer<T>> logger, IOptions<LogConfig> options,
                ISender sender,
                IConversationIdProvider conversationIdProvider) : base(logger, options, sender, conversationIdProvider)
            {
            }

            protected abstract Task Execute(ConsumeContext<T> context);

            protected Task Send<T2>(T2 message) where T2 : class
            {
                return Send<T2>((object)message);
            }
            protected Task Send<T2>(object message) where T2 : class
            {
                return Sender.Send<T2>(message, _context);
            }
            protected Task Publish<T2>(object message) where T2 : class
            {
                return Sender.Publish<T2>(message, _context);
            }
            protected Task Respond<T2>(T2 message) where T2 : class
            {
                return Respond<T2>((object)message);
            }
            protected Task Respond<T2>(object message) where T2 : class
            {
                return Sender.Respond<T2>(message, _context);
            }
            public async Task Consume(ConsumeContext<T> context)
            {
                _context = context;
                await Execute(_context);
            }
        }
    }