using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using HttpEnricher;
using Serilog;

namespace ShippingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SerilogExtensions.ConfigureLogging();
            CreateHost(args);
        }
        
        private static void CreateHost(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {
                Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}", ex);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
