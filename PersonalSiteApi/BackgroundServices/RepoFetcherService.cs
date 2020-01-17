using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
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
        private TimeSpan Interval { get; set; }
        private TimeSpan InitialWait { get; set; }


        public RepoFetcherService(IServiceProvider services, ILogger<RepoFetcherService> logger)
        {
            _services = services;
            _logger = logger;
            InitialWait = TimeSpan.FromSeconds(30);
            Interval = TimeSpan.FromSeconds(600);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _timer = new Timer(FetchAndUploadRepos, null, InitialWait,Interval);
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