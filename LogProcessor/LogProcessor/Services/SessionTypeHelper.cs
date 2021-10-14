using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LogProcessor.Controllers;
using LogProcessor.Infrastructure;
using LogProcessor.Interfaces;
using LogProcessor.Models.Dto;
using LogProcessor.Models.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SessionTypes.Grammar;

namespace LogProcessor.Services
{
    public class SessionTypeHelper : ISessionTypeHelper
    {
        private readonly ISessionTypeCompiler _sessionTypeCompiler;
        private readonly ISessionManager _sessionManager;
        private readonly IAutomatonFactory<string, string> _automatonFactory;
        private readonly LogContext _dbContext;

        public SessionTypeHelper(
            ISessionTypeCompiler sessionTypeCompiler,
            ISessionManager sessionManager,
            IAutomatonFactory<string, string> automatonFactory,
            LogContext dbContext)
        {
            _sessionTypeCompiler = sessionTypeCompiler;
            _sessionManager = sessionManager;
            _automatonFactory = automatonFactory;
            _dbContext = dbContext;
        }
        public async Task<bool> AddSessionType(GrammarTextModelDto model, TextWriter output = null, bool overwrite = false)
        {
            output ??= TextWriter.Null;
            try
            {
                var generator = _sessionTypeCompiler.CompileText(model.InputText, output, output);
                if (generator != null)
                {
                    var externals = model.ExternalParticipants.ToArray();
                    var sessionType = new SessionType
                    {
                        Name = model.SessionTypeName,
                        Text = model.InputText,
                        Id = model.Id ?? 0,
                        InternalParticipants = generator.InternalParticipants.Except(externals).ToArray(),
                        ExternalParticipants = generator.ExternalParticipants.Union(externals).ToArray(),
                        ShouldLoad = true
                    };

                    var exists = await _dbContext.SessionTypes.AsQueryable().AnyAsync(s => EF.Functions.ILike(s.Name, model.SessionTypeName) && s.Id != sessionType.Id);
                    if (!exists)
                    {

                        var servicesInUse = (await _dbContext.SessionTypes.AsQueryable()
                                .Where(s => !EF.Functions.ILike(s.Name, model.SessionTypeName) && s.Id != sessionType.Id)
                                .ToListAsync())
                            .Select(s => new
                            {
                                s.Name,
                                intersection = s.InternalParticipants.Intersect(sessionType.InternalParticipants)
                            })
                            .Where(s => s.intersection.Any())
                            .ToList();

                        if (!servicesInUse.Any())
                        {
                            _dbContext.SessionTypes.Update(sessionType);
                            await _dbContext.SaveChangesAsync();
                            _automatonFactory.Unset(sessionType.Name);
                            _sessionManager.StopTrackingSessionsFor(sessionType.Name);
                            await InitializeSession(sessionType, output);
                            return true;
                        }
                        
                        await output.WriteLineAsync(
                            $"The following services are in use elsewhere: \n{string.Join($",\n", servicesInUse.Select(s => $"In [{s.Name}]: [{string.Join(", ", s.intersection)}]"))}");
                    }
                    else
                    {
                        await output.WriteLineAsync($"The name {model.SessionTypeName} is already in use.");
                    }
                }
            }
            catch (Exception e)
            {
                if (e is not CompileException)
                {
                    await output.WriteLineAsync(e.ToString());
                }
            }
            return false;
        }

        private ValueTask<SessionType> GetSessionType(int id)
        {
            return _dbContext.SessionTypes.FindAsync(id);
        }
        public ValueTask<List<SessionType>> GetSessionTypes()
        {
            return AsyncEnumerable.ToListAsync(_dbContext.SessionTypes);
        }

        public async ValueTask<string> GetSessionTypeText(int id)
        {
            var sessionType = await GetSessionType(id);
            if (sessionType == null)
            {
                return "";
            }

            return sessionType.Text;
        }

        public async Task ChangeStatus(int id)
        {
            var sessionType = await GetSessionType(id);
            sessionType.ShouldLoad = !sessionType.ShouldLoad;
            if (!sessionType.ShouldLoad)
            {
                _automatonFactory.Unset(sessionType.Name);
                _sessionManager.StopTrackingSessionsFor(sessionType.Name);
            }
            else
            {
                await InitializeSession(sessionType);
            }
            
            await _dbContext.SaveChangesAsync();
            
        }

        private async Task InitializeSession(SessionType sessionType, TextWriter output = null)
        {
            output ??= TextWriter.Null;
            var serviceAutomata =
                _sessionTypeCompiler.CompileText(sessionType.Text, output, output);
            var serviceDefinition = serviceAutomata.GetDefinition();

            serviceDefinition.ServicesDefinitions = serviceDefinition.ServicesDefinitions
                .Where(f => !sessionType.ExternalParticipants?.Contains(f.Key) ?? true)
                .ToDictionary(f => f.Key, f => f.Value);
            serviceDefinition.Name = sessionType.Name;
            await _automatonFactory.InitializeAutomatonConfigurations(serviceDefinition);
        }

        public async ValueTask RemoveSessionType(int id)
        {
            var sessionType = await GetSessionType(id);
            //_dbContext.SessionTypes.Remove(new SessionType(){Id = id}).Entity);
          
            _automatonFactory.Unset(sessionType.Name);
            _sessionManager.StopTrackingSessionsFor(sessionType.Name);
            _dbContext.SessionTypes.Remove(sessionType);
            await _dbContext.SaveChangesAsync();
        }
    }
}