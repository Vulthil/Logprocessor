using System.Threading.Tasks;
using HttpEnricher.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;
using Services.Shared.Models;

namespace HttpEnricher
{
    public class RequestSessionActionFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger<RequestSessionActionFilterAttribute> _logger;
        private readonly LogConfig _config;

        public RequestSessionActionFilterAttribute(IOptions<LogConfig> options, ILogger<RequestSessionActionFilterAttribute> logger)
        {
            _logger = logger;
            _config = options.Value;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using (LogContext.PushProperty("SessionId", context.HttpContext.GetOrAddHeader(Constants.HeaderName)))
            {
                if (_config.ShouldLog)
                {
                
                    var logMessage = new LogMessage()
                    {
                        SessionId = context.HttpContext.GetOrAddHeader(Constants.HeaderName),
                        Direction = Direction.Inbound,
                        Destination = _config.Name,
                        Origin = "BuyerGateway.Client", // TODO: Get ClientName from context
                        TargetApi = ((ControllerBase) context.Controller).ControllerContext.ActionDescriptor.ActionName
                    };
                    _logger.LogInformation("{LogMessage}", logMessage);
                }

                await next();

                if (_config.ShouldLog)
                {

                    var logMessage = new LogMessage()
                    {
                        SessionId = context.HttpContext.GetOrAddHeader(Constants.HeaderName),
                        Direction = Direction.Outbound,
                        Destination = "BuyerGateway.Client", // TODO: Get ClientName from context
                        Origin = _config.Name,
                        TargetApi = ((ControllerBase) context.Controller).ControllerContext.ActionDescriptor.ActionName
                    };
                    _logger.LogInformation("{LogMessage}", logMessage);
                }
            }
        }
    }
}