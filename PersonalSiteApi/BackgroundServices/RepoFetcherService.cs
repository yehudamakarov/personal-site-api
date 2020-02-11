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
    public class RepoFetcherService : IHostedService, IDisposable
    {
        private const string ServiceName = nameof(RepoFetcherService);
        private readonly ILogger<RepoFetcherService> _logger;
        private readonly IServiceProvider _services;
        private Timer _timer;
        private TimeSpan Interval { get; }
        private TimeSpan InitialWait { get; }


        public RepoFetcherService(IServiceProvider services, ILogger<RepoFetcherService> logger,
            IConfiguration configuration)
        {
            _services = services;
            _logger = logger;
            InitialWait = TimeSpan.FromSeconds(30);
            var parsed = double.TryParse(configuration["RECURRING_JOB_TIME_INTERVAL_IN_SECONDS"], out var seconds);
            Interval = TimeSpan.FromSeconds(parsed ? seconds : 43200);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _timer = new Timer(FetchAndUploadRepos, null, InitialWait, Interval);
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
            _logger.LogWarning(nameof(RepoFetcherService) + "was stopped asynchronously");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void FetchAndUploadRepos(object state)
        {
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var repoFetcherBL = scope.ServiceProvider.GetRequiredService<IGithubRepoFetcherJob>();
                    repoFetcherBL.BeginJobAsync();
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