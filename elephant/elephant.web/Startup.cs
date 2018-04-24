using System;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using elephant.web.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace elephant.web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddLogging();
            return new Container()
                .WithDependencyInjectionAdapter(services, throwIfUnresolved: type => type.Name.EndsWith("Controller"))
                .ConfigureServiceProvider<DIConfig>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
            app.UseWebSockets();
            app.UseWebSocketsMiddleware(provider.GetService<WebSocketsHandler>());
            var sLogger = new LoggerConfiguration().WriteTo.File("log.txt").MinimumLevel.Debug().CreateLogger();
            var loggerfactory = provider.GetService<ILoggerFactory>().AddConsole(LogLevel.Information).AddSerilog(sLogger);
        }
    }
}
