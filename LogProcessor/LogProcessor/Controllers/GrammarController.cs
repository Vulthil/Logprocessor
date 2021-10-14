using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LogProcessor.Interfaces;
using LogProcessor.Models.Dto;
using LogProcessor.Models.Infrastructure;
using LogProcessor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.Logging;

namespace LogProcessor.Controllers
{
    public class GrammarController : BaseController
    {
        private readonly ISessionTypeHelper _sessionTypeHelper;

        public GrammarController(ILogger<GrammarController> logger, ISessionTypeHelper sessionTypeHelper) : base(logger)
        {
            _sessionTypeHelper = sessionTypeHelper;
        }


        [HttpPost]
        [ProducesResponseType(typeof(ResultDto), 200)]
        [ProducesResponseType(typeof(ResultDto), 400)]
        public async Task<IActionResult> UploadText(GrammarTextModelDto model)
        {
            var stringWriter = new StringWriter();
            if (await _sessionTypeHelper.AddSessionType(model, stringWriter))
            {
                return Ok(new ResultDto());
            }
            return BadRequest(new ResultDto("Error: " + stringWriter.ToString()));
        }

        [HttpGet]
        public ValueTask<List<SessionType>> GetSessionTypes()
        {
            return _sessionTypeHelper.GetSessionTypes();
        }

        [HttpDelete("{id}")]
        public ValueTask RemoveSessionType(int id)
        {
            return _sessionTypeHelper.RemoveSessionType(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            await _sessionTypeHelper.ChangeStatus(id);
            return Ok();
        }
    }
}