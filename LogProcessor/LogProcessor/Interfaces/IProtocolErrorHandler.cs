using System.Collections.Generic;
using System.Threading.Tasks;
using LogProcessor.Models;

namespace LogProcessor.Interfaces
{
    public interface IProtocolErrorHandler
    {
        Task PersistError(ErrorResult error);
        Task PersistErrors(List<ErrorResult> batch);
    }
}