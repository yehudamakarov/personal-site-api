using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.GoogleCloudLogging;

namespace PersonalSiteApi
{
	public class Program
	{
		public static int Main( string[] args )
		{
			var config = new ConfigurationBuilder().AddEnvironmentVariables()
				.Build();
			var googleCloudLoggingConfig =
				new GoogleCloudLoggingSinkOptions(config["GOOGLE_PROJECT_ID"])
					{ UseJsonOutput = true };
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
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

		public static IWebHostBuilder CreateWebHostBuilder( string[] args ) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseSerilog();
	}
}