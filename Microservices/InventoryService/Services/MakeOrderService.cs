using System.Threading.Tasks;
using HttpEnricher.Config;
using HttpEnricher.MassTransit;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Shared.Messages;

namespace InventoryService.Services
{
    public class MakeOrderService : BaseConsumer<MakeOrder>
    {
        public MakeOrderService(ILogger<MakeOrderService> logger,
            IOptions<LogConfig> options,
            ISender sender, IConversationIdProvider conversationIdProvider)
            : base(logger, options, sender, conversationIdProvider)
        {
        }

        protected override Task Execute(ConsumeContext<MakeOrder> context)
        {
            var message = context.Message;

            var shippingOrder = new
            {

            };
            return Publish<ShipOrder>(shippingOrder);
        }

       
    }
}