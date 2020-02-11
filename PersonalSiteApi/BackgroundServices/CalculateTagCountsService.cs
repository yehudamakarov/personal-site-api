using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PersonalSiteApi.BackgroundServices
{
    public class CalculateTagCountsService : IHostedService, IDisposable
    {
        private const string ServiceName = nameof(CalculateTagCountsService);
        private readonly ILogger<CalculateTagCountsService> _logger;
        private readonly IServiceProvider _services;
        private Timer _timer;
        private TimeSpan Interval { get; }


        public CalculateTagCountsService(IServiceProvider services, ILogger<CalculateTagCountsService> logger, IConfiguration configuration)
        {
            _services = services;
            _logger = logger;
            var parsed = double.TryParse(configuration["RECURRING_JOB_TIME_INTERVAL_IN_SECONDS"], out var seconds);
            Interval = TimeSpan.FromSeconds(parsed ? seconds : 43200);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _timer = new Timer(CalculateTagCounts, null, TimeSpan.FromSeconds(15), Interval);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Job in {ServiceName} Failed!", ServiceName);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning(ServiceName + "was stopped asynchronously.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void CalculateTagCounts(object state)
        {
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var calculateTagCountsJob = scope.ServiceProvider.GetRequiredService<ICalculateTagCountsJob>();
                    calculateTagCountsJob.BeginJobAsync();
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