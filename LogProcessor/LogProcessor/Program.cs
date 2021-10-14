using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading;
using LogProcessor.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using LogProcessor.Controllers;
using LogProcessor.Interfaces;
using LogProcessor.Models.Dto;
using LogProcessor.Models.Infrastructure;
using LogProcessor.Services;

namespace LogProcessor
{
    internal interface IDataSeeder
    {
        public Task MigrateAsync(CancellationToken cancellationToken = default);
        public Task SeedAsync(CancellationToken cancellationToken = default);
    }

    internal class DataSeeder : IDataSeeder
    {
        private readonly LogContext _context;
        private readonly ISessionTypeHelper _sessionTypeHelper;

        public DataSeeder(LogContext context, ISessionTypeHelper sessionTypeHelper)
        {
            _context = context;
            _sessionTypeHelper = sessionTypeHelper;
        }

        public Task MigrateAsync(CancellationToken cancellationToken = default)
        {
            return _context.Database.MigrateAsync(cancellationToken);
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (! (await _context.SessionTypes.AnyAsync(cancellationToken)))
            {
                var model = new GrammarTextModelDto
                {
                    SessionTypeName = "BuyerSellerBank"
                };
                var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var path = Path.Combine(assemblyPath, "SessionTypes","Initial", "session" + ".lt");
                if (File.Exists(path))
                {
                    model.InputText = await File.ReadAllTextAsync(path, cancellationToken);
                    await _sessionTypeHelper.AddSessionType(model, null, true);
                }
                
            }
        }
    }
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    var databaseSetup = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
                    await databaseSetup.MigrateAsync();
                    await databaseSetup.SeedAsync();
                }
                catch (Exception e)
                { 
                    logger.LogCritical(e, "Something went wrong during database migration.");
                    return;
                }
            }
            await host.RunAsync();
            //host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
