using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LogProcessor.Controllers;
using LogProcessor.Models.Dto;
using LogProcessor.Models.Infrastructure;

namespace LogProcessor.Interfaces
{
    public interface ISessionTypeHelper
    {
        Task<bool> AddSessionType(GrammarTextModelDto model, TextWriter output = null, bool overwrite = false);
        ValueTask<List<SessionType>> GetSessionTypes();
        ValueTask<string> GetSessionTypeText(int id);
        Task ChangeStatus(int id);
        ValueTask RemoveSessionType(int id);
    }
}