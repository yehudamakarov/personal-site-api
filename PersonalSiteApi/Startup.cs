using Infrastructure.Notification.JobStatus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonalSiteApi.StartupHelper;
using zipkin4net;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;
using zipkin4net.Middleware;

namespace PersonalSiteApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();
            services.ConfigureAuthentication(Configuration);
            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwagger();
            services.AddSrc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.CreateLogger("zipkin-tracing-middleware");
            var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            lifetime.ApplicationStarted.Register(() => {
                TraceManager.SamplingRate = 1.0f;
                var logger = new TracingLogger(loggerFactory, "zipkin4net");
                var httpSender = new HttpZipkinSender("zipkin-service", "application/json");
                var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer());
                TraceManager.RegisterTracer(tracer);
                TraceManager.Start(logger);
            });
            lifetime.ApplicationStopped.Register(() => TraceManager.Stop());
            app.UseTracing("personal-site-api");
            
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            else
                app.UseHsts();

            app.UseCors("SignalRPolicy");
            app.UseAuthentication();
            app.UseSignalR(
                hubRouteBuilder => { hubRouteBuilder.MapHub<JobStatusUpdatesHub>("/hubs/JobStatusUpdates"); }
            );
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    if (env.IsProduction())
                        options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Personal Site API v1");
                    if (env.IsDevelopment())
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Personal Site API v1");
                }
            );
            app.UseMvc();
        }
    }
}