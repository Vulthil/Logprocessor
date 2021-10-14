using System.Collections.Generic;
using System.Threading.Tasks;
using LogProcessor.Models;
using LogProcessor.Models.Infrastructure;

namespace LogProcessor.Interfaces
{
    public interface IProtocolErrorRepoitory
    {
        Task CommitProtocolError(ErrorResult errorResult);
        Task CommitProtocolErrors(List<ErrorResult> errorResults);

        (IEnumerable<ViolatingMessage> sessions, int total) GetViolatingMessages(int skip, int take);
    }
}