using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.BackgroundServices
{
    public class RepoFetcherService : IHostedService
    {
        private readonly IServiceProvider _services;
        private Timer _timer;

        public RepoFetcherService(IServiceProvider services)
        {
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(FetchAndUploadRepos, null, TimeSpan.Zero, TimeSpan.FromHours(6));
            return Task.CompletedTask;
        }

        private void FetchAndUploadRepos(object state)
        {
            using (var scope = _services.CreateScope())
            {
                var repoFetcherBL = scope.ServiceProvider.GetRequiredService<IGithubRepoFetcherBL>();
                repoFetcherBL.BeginJobAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}