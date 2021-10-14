using HttpEnricher;
using Microsoft.AspNetCore.Mvc;

namespace BuyerGateway.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(RequestSessionActionFilterAttribute))]
    public class BaseController : ControllerBase
    {
    }
}