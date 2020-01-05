using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PersonalSiteApi.BackgroundServices
{
    public class AddToProjectsService : IHostedService
    {
        private const string ServiceName = nameof(AddToProjectsService);
        private readonly ILogger<AddToProjectsService> _logger;
        private readonly IServiceProvider _services;

        public AddToProjectsService(IServiceProvider services, ILogger<AddToProjectsService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var unused = new Timer(AddPinnedReposToProjects, null, TimeSpan.FromSeconds(60),
                    TimeSpan.FromHours(12));
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

        private void AddPinnedReposToProjects(object state)
        {
            using (var scope = _services.CreateScope())
            {
                var addToProjectsJob = scope.ServiceProvider.GetRequiredService<IAddToProjectsJob>();
                addToProjectsJob.BeginJobAsync();
            }
        }
    }
}