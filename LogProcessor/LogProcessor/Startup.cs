using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using LogProcessor.Controllers;
using LogProcessor.Converters;
using LogProcessor.HostedServices;
using LogProcessor.HostedServices.Channels;
using LogProcessor.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LogProcessor.Interfaces;
using LogProcessor.Model.Json;
using LogProcessor.Model.Utilities;
using LogProcessor.Services;
using LogProcessor.Services.Hosted;
using Microsoft.EntityFrameworkCore;
using SessionTypes.Grammar;

namespace LogProcessor
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            //var builder = new ConfigurationBuilder()
            //    .AddJsonFile("Demo.session")
            //    .Build();
            services.AddMemoryCache();

            services.AddScoped<IDataSeeder, DataSeeder>();
            services.AddScoped<ISessionTypeCompiler, SessionTypeCompiler>();
            services.AddScoped<IConfiguredMemoryCache, ConfiguredMemoryCache>();

            //services.Configure<ServiceAutomata>(builder);
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.Converters.Add(new ValueTupleFactory());
                });
            services.AddOpenApiDocument(options =>
            {
            });
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LogProcessor", Version = "v1" });
            //});
            //services.AddSingleton<IStringJsonDeserializerFactory, StringJsonDeserializerFactory>();
            services.AddScoped<ILogService, LogService>();
            services.AddSingleton(typeof(GenericChannel<>));
            services.AddSingleton(typeof(IChannelWriter<>), typeof(GenericChannelWriter<>));
            services.AddSingleton(typeof(IChannelReader<>), typeof(GenericChannelReader<>));
            services.AddDbContext<LogContext>(p => p
                //.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => { loggingBuilder.AddConsole(); }))
                .UseNpgsql(Configuration.GetConnectionString("Default"))
            );
            services.AddSingleton<ISessionManager, SessionManager>();
            services.AddScoped<ISessionTypeHelper, SessionTypeHelper>();
            services.AddSingleton<IAutomatonFactory<string,string>, StringAutomatonFactory>();
            services.AddScoped<IProtocolErrorHandler, ProtocolErrorHandler>();
            services.AddScoped<IProtocolErrorRepoitory, LogErrorRepository>();
            //services.AddSingleton(p => (IAutomatonFactoryInitializer)p.GetRequiredService<IAutomatonFactory<string, string>>());
            services.AddHostedService<ConfigurationInitializer>();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    var strings = Configuration["CorsOrigins"]
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.RemovePostFix("/"))
                        .ToArray();
                    builder
                        .WithOrigins(
                            strings
                        )
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi();
                app.UseSwaggerUi3();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LogProcessor v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            // app.UseHttpsRedirection();
            
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}
