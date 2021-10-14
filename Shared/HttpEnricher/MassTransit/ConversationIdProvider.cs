using System;
using HttpEnricher.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HttpEnricher.MassTransit
{
    public class ConversationIdProvider : IConversationIdProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LogConfig _config;

        public ConversationIdProvider(IHttpContextAccessor httpContextAccessor, IOptions<LogConfig> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _config = options.Value;
        }

        public Guid? GetConversationId()
        {
            if (_config.IsInitiator)
            {
                return Guid.Parse(_httpContextAccessor.HttpContext.GetOrAddHeader(Constants.HeaderName));
            }
            return null;
        }
    }
}