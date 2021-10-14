using System.Threading.Tasks;
using HttpEnricher.Config;
using HttpEnricher.MassTransit;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Shared.Messages;

namespace ShippingService.Services
{
    public class ShipOrderService : BaseConsumer<ShipOrder>
    {
        public ShipOrderService(ILogger<ShipOrderService> logger,
            IOptions<LogConfig> options, 
            ISender sender, IConversationIdProvider conversationIdProvider)
            : base(logger, options, sender, conversationIdProvider)
        {
        }

        protected override Task Execute(ConsumeContext<ShipOrder> context)
        {
            return Task.CompletedTask;
        }
    }
}