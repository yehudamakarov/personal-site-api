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

namespace PersonalSiteApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration) { Configuration = configuration; }

		private IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors(
				corsOptions => corsOptions.AddPolicy(
					"SignalRPolicy",
					corsPolicyBuilder =>
					{
						corsPolicyBuilder.AllowAnyMethod()
							.AllowAnyHeader()
							.WithOrigins("http://localhost:3000")
							.AllowCredentials();
					}
				)
			);
			services.AddSignalR();

			services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(
					options =>
					{
						var signingKeyBytes = Convert.FromBase64String(Configuration["JWT_SIGNING_KEY"]);
						options.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
							ValidateIssuer = false,
							ValidateAudience = false
						};
						
					}
				);

			services.AddScoped<IAuthenticationBL, AuthenticationBL>();
			services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();


			services.AddScoped<IGithubRepoBL, GithubRepoBL>();
			services.AddScoped<IGithubRepoFetcherBL, GithubRepoFetcherBL>();
			services.AddScoped<IRepoInfrastructure, RepoInfrastructure>();
			services.AddScoped<IRepoRepository, RepoRepository>();

			services.AddScoped<IGithubRepoFetcherNotifier, GithubGithubRepoFetcherNotifier>();

			services.AddHostedService<RepoFetcherService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
			app.UseMvc();
		}
	}
}