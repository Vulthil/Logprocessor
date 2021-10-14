using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.JSInterop;
using SessionTypes.Grammar;
using UI.Client.Config;

namespace UI.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            

            builder.Services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            var remoteLogUrl = builder.Configuration["RemoteLogProcessorUrl"];

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(remoteLogUrl) });
            var types = typeof(BaseClient.BaseClient).Assembly.GetTypes()
                .Where(t => t.BaseType == typeof(BaseClient.BaseClient));
            foreach (var type in types)
            {
                builder.Services.AddScoped(type);
            }


            builder.Services.AddScoped<SessionTypeCompiler>();
            builder.Services.AddSingleton<Store>();
            builder.Services.Configure<PersistentStorageQueryConfiguration>(builder.Configuration.GetSection("QueryConfiguration"));
            await builder.Build().RunAsync();
        }
    }
}
