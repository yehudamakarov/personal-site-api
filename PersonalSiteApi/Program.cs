using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.GoogleCloudLogging;

namespace PersonalSiteApi
{
	public class Program
	{
		public static int Main(string[] args)
		{
			// environment-variables-configuration-provider by default only gets env variables from ASPNET_<variable> variables. add with the below.
			// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/index?view=aspnetcore-2.2#environment-variables-configuration-provider
			var config = new ConfigurationBuilder().AddEnvironmentVariables()
				.Build();
			var googleCloudLoggingConfig = new GoogleCloudLoggingSinkOptions(config["GOOGLE_PROJECT_ID"])
			{
				UseJsonOutput = true,
				ResourceType = "k8s_pod",
				UseSourceContextAsLogName = true,
			};
			Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
				.Enrich.FromLogContext()
				.WriteTo.Console(
					outputTemplate:
					"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} -- Properties: {Properties:j} {NewLine}{Exception}"
				)
				.WriteTo.GoogleCloudLogging(googleCloudLoggingConfig)
				.CreateLogger();
			try
			{
				Log.Information("Starting web host...");
				CreateWebHostBuilder(args)
					.Build()
					.Run();
				return 0;
			}
			catch (Exception exception)
			{
				Log.Fatal(exception, "Host terminated suddenly.");
				return 1;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseSerilog();
		}
	}
}