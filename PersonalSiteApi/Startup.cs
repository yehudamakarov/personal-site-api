using Core.Types;
using Google.Cloud.Diagnostics.AspNetCore;
using Google.Cloud.Diagnostics.Common;
using Infrastructure.Notification.JobStatus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonalSiteApi.StartupHelper;

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
            // services.AddOptions();
            // services.Configure<StackdriverOptions>(
            //     Configuration.GetSection("Stackdriver"));
            
            services.AddGoogleExceptionLogging(options =>
            {
                options.ProjectId = Configuration["GOOGLE_PROJECT_ID"];
                options.ServiceName = Configuration["SERVICE_NAME"];
                options.Version = Configuration["SERVICE_VERSION"];
            });

            // Add trace service.
            services.AddGoogleTrace(options =>
            {
                options.ProjectId = Configuration["GOOGLE_PROJECT_ID"];
                options.Options = TraceOptions.Create(
                    bufferOptions: BufferOptions.NoBuffer());
            });

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
            // GoogleTrace //
            // ------------------------------------------------------------------------------------- //
            // Configure logging service.
            loggerFactory.AddGoogle(app.ApplicationServices, Configuration["GOOGLE_PROJECT_ID"]);
            var logger = loggerFactory.CreateLogger("testStackdriverLogging");
            // Write the log entry.
            logger.LogInformation("Stackdriver sample started. This is a log message.");
            // Configure error reporting service.
            app.UseGoogleExceptionLogging();
            // Configure trace service.
            app.UseGoogleTrace();
            // ------------------------------------------------------------------------------------- //
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