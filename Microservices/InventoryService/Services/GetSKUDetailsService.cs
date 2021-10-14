using System.Threading.Tasks;
using HttpEnricher.Config;
using HttpEnricher.MassTransit;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Shared.Messages;

namespace InventoryService.Services
{
    public class GetSKUDetailsService : BaseConsumer<GetSKUDetails>
    {
        public GetSKUDetailsService(ILogger<GetSKUDetailsService> logger,
            IOptions<LogConfig> options,
            ISender sender, IConversationIdProvider conversationIdProvider)
            : base(logger, options, sender, conversationIdProvider)
        {
        }

        protected override Task Execute(ConsumeContext<GetSKUDetails> context)
        {
            var message = context.Message;

            return Respond<SKUDetails>(new {
                SKU = message.SKU,
                Amount = message.Amount,
                Price = 22.5,
                Qty = 100,
            });
        }
    }
}