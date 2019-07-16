using Microsoft.Extensions.DependencyInjection;

namespace PersonalSiteApi.StartupHelper
{
	public static class StartupCors
	{
		public static IServiceCollection ConfigureCors(this IServiceCollection services)
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
			return services;
		}
	}
}