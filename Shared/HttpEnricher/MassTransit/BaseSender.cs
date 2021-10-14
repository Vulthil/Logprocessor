using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HttpEnricher.Config;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Shared.Models;

namespace HttpEnricher.MassTransit
{
    public interface ISender
    {
        public Task Send<T>(T message, ConsumeContext context = null) where T : class;
        public Task Send<T>(object message, ConsumeContext context = null) where T : class;
        public Task Publish<T>(object message, ConsumeContext context = null) where T : class;
        Task Respond<T>(object message, ConsumeContext context) where T : class;
        Task Respond<T>(T message, ConsumeContext context) where T : class;
        Task<Response<T2>> Request<T, T2>(IRequestClient<T> client, T message) where T : class where T2 : class;
        Task<Response<T2>> Request<T, T2>(IRequestClient<T> client, object message) where T : class where T2 : class;
        Task<Response<T2, T3>> Request<T, T2, T3>(IRequestClient<T> client, object message) where T : class where T2 : class where T3 : class;
    }

    public class BaseSender : ISender
    {
        private readonly ILogger<BaseSender> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IOptions<LogConfig> _options;
        private readonly IConversationIdProvider _conversationIdProvider;
        private readonly LogConfig _config;

        public BaseSender(ILogger<BaseSender> logger,
            IPublishEndpoint publishEndpoint,
            ISendEndpointProvider sendEndpointProvider,
            IOptions<LogConfig> options,
            IConversationIdProvider conversationIdProvider)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
            _options = options;
            _conversationIdProvider = conversationIdProvider;
            _config = options.Value;
        }

        public Task Send<T>(T message, ConsumeContext context = null) where T : class
        {
            return Send<T>((object)message, context);
        }

        public Task Send<T>(object message, ConsumeContext context = null) where T : class
        {
            return (context ?? _sendEndpointProvider).Send<T>(message);
        }
        public  Task Publish<T>(object message, ConsumeContext context = null) where T : class
        {
            return (context ?? _publishEndpoint).Publish<T>(message);
        }
        public Task Respond<T>(object message, ConsumeContext context) where T : class
        {
            return context.RespondAsync<T>(message);
        }

        public Task<Response<T2>> Request<T, T2>(IRequestClient<T> client, T message) where T : class where T2 : class
        {
            return Request<T,T2>(client, (object) message);
        }

        public async Task<Response<T2>> Request<T, T2>(IRequestClient<T> client, object message) where T : class where T2 : class
        {
            var response = await client.GetResponse<T2>(message);
            LogResponseMessage(response);
            return response;
        }

        public async Task<Response<T2, T3>> Request<T, T2, T3>(IRequestClient<T> client, object message)
            where T : class where T2 : class where T3 : class
        {
            var response = await client.GetResponse<T2, T3>(message);
            LogResponseMessage(response);
            return response;
        }

        private void LogResponseMessage(Response response)
        {
            if (_config.ShouldLog)
            {
                var logMessage = new LogMessage()
                {
                    SessionId = (_conversationIdProvider.GetConversationId() ?? response.ConversationId).ToString(),
                    Direction = Direction.Inbound,
                    Destination = _config.Name, // TODO: Mby not needed ?
                    TargetApi = response.Message.GetType().Name,
                    Origin = response.Host.ProcessName,
                };
                _logger.LogInformation("{LogMessage}", logMessage);
            }
        }

        public Task Respond<T>(T message, [NotNull] ConsumeContext context) where T : class
        {
            return Respond<T>((object)message, context);
        }
    }
}