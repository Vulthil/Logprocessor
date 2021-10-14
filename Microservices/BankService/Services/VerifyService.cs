using System;
using System.Threading.Tasks;
using HttpEnricher.Config;
using HttpEnricher.MassTransit;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Shared.Messages;

namespace BankService.Services
{
    public class VerifyService : BaseConsumer<VerifyCC>
    {

        public VerifyService(ILogger<VerifyService> logger,
            IOptions<LogConfig> options,
            ISender sender, IConversationIdProvider conversationIdProvider)
            : base(logger, options, sender, conversationIdProvider)
        {
        }

        protected override Task Execute(ConsumeContext<VerifyCC> context)
        {
            var m = new
            {
                //IsValid = true // TODO: Mby not.
            };
            if (new Random().Next(10) < 2)
            {
                return Respond<InvalidCC>(m);
            }
            return Respond<ValidCC>(m);
        }
    }

}
