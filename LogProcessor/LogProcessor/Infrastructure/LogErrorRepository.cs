using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogProcessor.Interfaces;
using LogProcessor.Models;
using LogProcessor.Models.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Services.Shared.Models;

namespace LogProcessor.Infrastructure
{
    public class LogErrorRepository : IProtocolErrorRepoitory
    {
        private readonly LogContext _dataContext;
        public LogErrorRepository(LogContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task CommitProtocolError(ErrorResult errorResult)
        {
            //return Task.CompletedTask;
            var msgEntity = _dataContext.LogMessages.Add(new LogMessageEntity(errorResult.Message, errorResult.ErrorMessage));
            if (IsPoisoned(errorResult.Message, out var violatingMessage))
            {
                _dataContext.PoisonedMessages.Add(new PoisonedMessage(msgEntity.Entity, violatingMessage));
            }
            else
            {
                _dataContext.ViolatingMessages.Add(new ViolatingMessage(msgEntity.Entity));
            }

            return _dataContext.SaveChangesAsync();

        }

        public async Task CommitProtocolErrors(List<ErrorResult> errorResults)
        {
            var grouped = errorResults.GroupBy(e => e.Message.SessionId);
            var existingIds = _dataContext.ViolatingMessages.AsQueryable()
                .Where(p => grouped.Select(g => g.Key).Contains(p.LogMessage.Message.SessionId))
                .Select(vm => new {vm.Id, vm.LogMessage.Message.SessionId}).ToDictionary(x => x.SessionId, x => x.Id);

            var isPoisoned = grouped.ToLookup(g => existingIds.ContainsKey(g.Key));

            var baseEntities = new List<object>();
            var violating = new List<object>();
            var poisoned = new List<object>();

            foreach (var grouping in isPoisoned[true])
            {
                foreach (var p in grouping)
                {
                    var m = new LogMessageEntity(p.Message, p.ErrorMessage);
                    baseEntities.Add(m);
                    poisoned.Add(new PoisonedMessage(m, existingIds[m.Message.SessionId]));
                }
            }
            foreach (var grouping in isPoisoned[false])
            {
                var head = grouping.First();
                var m = new LogMessageEntity(head.Message, head.ErrorMessage);
                baseEntities.Add(m);
                var violator = new ViolatingMessage(m);
                violating.Add(violator);
                foreach (var p in grouping)
                {
                    if (p == head)
                    {
                        continue;
                    }
                    m = new LogMessageEntity(p.Message, p.ErrorMessage);
                    baseEntities.Add(m);
                    poisoned.Add(new PoisonedMessage(m, violator));
                }
            }
            await _dataContext.AddRangeAsync(baseEntities.Concat(poisoned).Concat(violating));
            await _dataContext.SaveChangesAsync();
        }

        public (IEnumerable<ViolatingMessage> sessions, int total) GetViolatingMessages(int skip, int take)
        {
            var query = _dataContext.ViolatingMessages
                .AsQueryable().OrderBy(x => x.Id)
                .Include(x => x.LogMessage)
                .Include(x => x.PoisonedMessages).ThenInclude(x => x.LogMessage);
            return (query.Skip(skip).Take(take).ToList(), query.Count());
        }

        private bool IsPoisoned(LogMessage msg, out ViolatingMessage violatingMessage)
        {
            violatingMessage =  _dataContext.ViolatingMessages.FirstOrDefault(m => m.LogMessage.Message.SessionId == msg.SessionId);
            return violatingMessage != null;
        }
    }
}