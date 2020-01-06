using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PersonalSiteApi.BackgroundServices
{
    public class RepoFetcherService : IHostedService
    {
        private const string ServiceName = nameof(RepoFetcherService);
        private readonly ILogger<RepoFetcherService> _logger;
        private readonly IServiceProvider _services;
        private TimeSpan Interval { get; set; }
        private TimeSpan InitialWait { get; set; }


        public RepoFetcherService(IServiceProvider services, ILogger<RepoFetcherService> logger)
        {
            _services = services;
            _logger = logger;
            InitialWait = TimeSpan.FromSeconds(30);
            Interval = TimeSpan.FromSeconds(30);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var unused = new Timer(FetchAndUploadRepos, null, InitialWait,Interval);
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
            throw new NotImplementedException();
        }

        private void FetchAndUploadRepos(object state)
        {
            using (var scope = _services.CreateScope())
            {
                var repoFetcherBL = scope.ServiceProvider.GetRequiredService<IGithubRepoFetcherJob>();
                repoFetcherBL.BeginJobAsync();
            }
        }
    }
}