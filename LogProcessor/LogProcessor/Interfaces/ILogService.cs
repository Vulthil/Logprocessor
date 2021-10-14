using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Shared.Models;

namespace LogProcessor.Interfaces
{
    public interface ILogService
    {
        Task LogMessages(IEnumerable<LogMessage> messages, DateTime requestTime);
#if DEBUG
        int GetSessionManagerCount();
#endif
    }
}