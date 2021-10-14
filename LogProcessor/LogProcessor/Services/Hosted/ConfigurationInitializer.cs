using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LogProcessor.Infrastructure;
using LogProcessor.Interfaces;
using LogProcessor.Model.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SessionTypes.Grammar;

namespace LogProcessor.Services.Hosted
{
    public class ConfigurationInitializer : IHostedService
    {
        private IAutomatonFactory<string, string> _automatonFactory;
        private readonly ILogger<ConfigurationInitializer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHostEnvironment _environment;

        public ConfigurationInitializer(IAutomatonFactory<string, string> automatonFactory,
            ILogger<ConfigurationInitializer> logger,
            IServiceScopeFactory serviceScopeFactory,
            IHostEnvironment environment)
        {
            _automatonFactory = automatonFactory;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _environment = environment;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LogContext>();
                var sessionTypeCompiler = scope.ServiceProvider.GetRequiredService<ISessionTypeCompiler>();

                var loadedSessions = await context.SessionTypes.AsQueryable().Where(s => s.ShouldLoad)
                    .ToListAsync(cancellationToken);
                foreach (var loadedSession in loadedSessions)
                {
                    var stringWriter = new StringWriter();
                    try
                    {
                       
                        var serviceAutomata =
                            sessionTypeCompiler.CompileText(loadedSession.Text, stringWriter, stringWriter);
                        var serviceDefinition = serviceAutomata.GetDefinition();

                        serviceDefinition.ServicesDefinitions = serviceDefinition.ServicesDefinitions
                            .Where(f => !loadedSession.ExternalParticipants?.Contains(f.Key) ?? true)
                            .ToDictionary(f => f.Key, f => f.Value);
                        serviceDefinition.Name = loadedSession.Name;
                        await _automatonFactory.InitializeAutomatonConfigurations(serviceDefinition);
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(stringWriter.ToString());
                        throw;
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
