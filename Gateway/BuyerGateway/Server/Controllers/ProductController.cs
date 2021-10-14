using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HttpEnricher.MassTransit;
using MassTransit;
using Services.Shared.Messages;

namespace BuyerGateway.Server.Controllers
{
    public class ProductController : BaseController
    {
        private readonly ISender _sender;
        private readonly IRequestClient<GetSKUDetails> _skuDetailsRequestClient;
        private readonly IRequestClient<VerifyCC> _verifyCcRequestClient;
        private readonly IConversationIdProvider _conversationIdProvider;

        public ProductController(ISender sender,
            IRequestClient<GetSKUDetails> skuDetailsRequestClient,
            IRequestClient<VerifyCC> verifyCCRequestClient,
            IConversationIdProvider conversationIdProvider)
        {
            _sender = sender;
            _skuDetailsRequestClient = skuDetailsRequestClient;
            _verifyCcRequestClient = verifyCCRequestClient;
            _conversationIdProvider = conversationIdProvider;
        }


        [HttpPost("PlaceOrder")]
        [DisplayName("PlaceOrder")]
        public async Task<ActionResult> PlaceOrder()
        {
            var response = await _sender.Request<GetSKUDetails, SKUDetails>(_skuDetailsRequestClient, new
            {
                Amount = 2,
                SKU = "Yeet1337"
            });
            return Ok(response.Message);
        }

        [HttpPost("ConfirmOrder")]
        [DisplayName("ConfirmOrder")]
        public async Task<ActionResult> ConfirmOrder()
        {
            var isValid = false;
            var response = await _sender.Request<VerifyCC, ValidCC, InvalidCC>(_verifyCcRequestClient, new {});
            if (response.Is(out Response<ValidCC> validResponse))
            {
                isValid = true;
                var t1 = _sender.Publish<MakeCharge>(new {});

                var t2 = _sender.Publish<MakeOrder>(new {});
                
                await Task.WhenAll(t1, t2);
            }

            if (isValid)
            {
                return Ok();
            }

            return BadRequest("Could not verify CC");
        }

        [HttpPost("CancelOrder")]
        [DisplayName("CancelOrder")]
        public async Task<ActionResult> CancelOrder()
        {
            return Ok();
        }

        [HttpPost("Invalidate")]
        [DisplayName("Invalidate")]
        public async Task<ActionResult> Invalidate()
        {
            // For the client to provoke a message not expected by protocol
            return Ok();
        }
    }
}
