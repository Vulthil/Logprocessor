using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LogProcessor.Interfaces;
using LogProcessor.Models;
using LogProcessor.Models.Dto;
using LogProcessor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace LogProcessor.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : BaseController{
        private readonly ISessionManager _sessionManager;
        private readonly IProtocolErrorRepoitory _errorRepository;
        private readonly IConfiguredMemoryCache _cache;

        public SessionController(ISessionManager sessionManager, IProtocolErrorRepoitory errorRepository, IConfiguredMemoryCache cache, ILogger<SessionController> logger) : base(logger)
        {
            _sessionManager = sessionManager;
            _errorRepository = errorRepository;
            _cache = cache;
        }

        [HttpGet("tracked")]
        public TableRequestResponse<TrackedSessionDto> GetTrackedSessions([FromQuery]int skip = 0, [FromQuery]int take = 10)
        {
            var (sessions, total) = _sessionManager.TrackedSessions(skip, take);
            return new TableRequestResponse<TrackedSessionDto>(sessions.Select(s => new TrackedSessionDto(s)), total);
        }

        [HttpGet("faulted")]
        public TableRequestResponse<FaultedSessionDto> GetFaultedSessions([FromQuery]int skip = 0, [FromQuery]int take = 10)
        {
            var (sessions, total) = _errorRepository.GetViolatingMessages(skip, take);
            return new TableRequestResponse<FaultedSessionDto>(sessions.Select(s => new FaultedSessionDto(s)), total);
        }

        [HttpGet("awaiting")]
        public TableRequestResponse<AwaitingSessionDto> GetAwaitingSessions([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            var (sessions, total) = _sessionManager.AwaitingSessions(skip, take);
            return new TableRequestResponse<AwaitingSessionDto>(sessions.Select(s => new AwaitingSessionDto(s)), total);
        }

        [HttpPost("evict/{sessionId}")]
        public async Task<IActionResult> EvictSession(string sessionId, [FromQuery]bool withError = true)
        {
            return await Evict(sessionId, withError) ? Ok() : NoContent();
        }
        private async Task<bool> Evict(string sessionId, bool withError)
        {
            var existed = _sessionManager.EvictSession(sessionId);

            if (withError && existed)
            {
                var error = EvictionResult.ForcedEvictionResult(sessionId);
                await _errorRepository.CommitProtocolError(error);
                await _cache.CacheError(error);
            }

            return existed;
        }

        [HttpPost("evict")]
        public async Task<IActionResult> EvictSessions([FromBody]string[] sessionIds, [FromQuery] bool withError = true)
        {
            var evictionTasks = sessionIds.Select(s => Evict(s, withError));
            await Task.WhenAll(evictionTasks);
            return Ok();
        }

        [HttpGet("testCache")]
        public IActionResult TestCache()
        {
            var lim = 1_000_000;
            var list = new int[lim];
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < lim; i++)
            {
                _cache.Set(i,1337,TimeSpan.FromHours(24));
            }

            var addTime = sw.ElapsedMilliseconds;
            sw.Restart();
            for (int i = 0; i < lim; i++)
            {
                list[i] = _cache.Get<int>(i);
            }
            return Ok((addTime, sw.ElapsedMilliseconds));
        }

    }
}
