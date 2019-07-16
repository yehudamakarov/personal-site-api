using System;
using Core.BL;
using Core.Interfaces;
using Infrastructure.Infrastructure;
using Infrastructure.Notification.RepoSync;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PersonalSiteApi.BackgroundServices;
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
			services.ConfigureCors();
			services.ConfigureAuthentication(Configuration);
			services.AddSignalR();

			services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			services.AddSwagger();
			services.AddSrc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(
			IApplicationBuilder app,
			IHostingEnvironment env
		)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			else
				app.UseHsts();

			app.UseCors("SignalRPolicy");
			app.UseSignalR(
				hubRouteBuilder => { hubRouteBuilder.MapHub<RepoSyncNotificationHub>("/hubs/repoSyncJobUpdates"); }
			);
			app.UseAuthentication();
			app.UseSwagger(options => options.RouteTemplate = "swagger/{documentname}/swagger.json");
			app.UseSwaggerUI(
				options =>
				{
					options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Personal Site API v1");
					options.RoutePrefix = "swagger";
				}
			);
			app.UseMvc();
		}
	}
}