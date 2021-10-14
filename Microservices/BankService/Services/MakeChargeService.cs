using System.Threading.Tasks;
using HttpEnricher.Config;
using HttpEnricher.MassTransit;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Shared.Messages;
using Services.Shared.Models;

namespace BankService.Services
{
    public class MakeChargeService : BaseConsumer<MakeCharge>
    {
        public MakeChargeService(ILogger<MakeChargeService> logger,
            IOptions<LogConfig> options,
            ISender sender, IConversationIdProvider conversationIdProvider) 
            : base(logger, options, sender, conversationIdProvider)
        {
        }

        protected override Task Execute(ConsumeContext<MakeCharge> context)
        {
            LogExternalMessage("RealBank", "RealBankMakeCharge", context);
            return Task.CompletedTask;
        }
    }
}