using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Google.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PersonalSiteApi.BackgroundServices
{
    public class AddToProjectsService : IHostedService, IDisposable
    {
        private const string ServiceName = nameof(AddToProjectsService);
        private readonly ILogger<AddToProjectsService> _logger;
        private readonly IServiceProvider _services;
        private Timer _timer;

        public AddToProjectsService(IServiceProvider services, ILogger<AddToProjectsService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _timer = new Timer(AddPinnedReposToProjects, null, TimeSpan.FromSeconds(60),
                    TimeSpan.FromHours(12));
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Job in {ServiceName} Failed!", ServiceName);
                
            }
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning(ServiceName + "was stopped asynchronously.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void AddPinnedReposToProjects(object state)
        {
            // // https://stackoverflow.com/questions/45883171/how-to-handle-exceptions-in-system-threading-timer
            // throw new Exception("manual!!! :)");

            try
            {
                using (var scope = _services.CreateScope())
                {
                    var addToProjectsJob = scope.ServiceProvider.GetRequiredService<IAddToProjectsJob>();
                    addToProjectsJob.BeginJobAsync();
                }
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, $"{ServiceName} is encountering an  issue.");
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}