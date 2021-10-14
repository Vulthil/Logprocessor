using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogProcessor.Interfaces;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Services.Shared.Models;
using LogProcessor.Infrastructure;

namespace LogProcessor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : BaseController
    {
        private readonly ILogService _logService;

        public LogController(ILogger<LogController> logger, ILogService logService): base(logger)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> StartUp([FromServices]ISessionTypeHelper sessionHelper)
        {
            return Ok(await sessionHelper.GetSessionTypes());
        }

        [HttpPost]
        public async Task<IActionResult> PostBulkLogMessages([FromBody] IEnumerable<LogMessage> messages)
        {
            var now = DateTime.Now;
            await messages
                .GroupBy(m => m.SessionId)
                .ToAsyncEnumerable()
                .ForEachAwaitAsync(async ms => await _logService.LogMessages(ms.OrderBy(m => m.Time), now));

            return Ok();
        }

    }
}
