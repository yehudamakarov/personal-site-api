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
        private IServiceProvider _services;
        private readonly ILogger<AddToProjectsService> _logger;
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
                _timer = new Timer(AddPinnedReposToProjects, null, TimeSpan.FromSeconds(30), TimeSpan.FromHours(12));
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Job Failed!");
                throw;
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private void AddPinnedReposToProjects(object state)
        {
            using (var scope = _services.CreateScope())
            {
                var addToProjectsBL = scope.ServiceProvider.GetRequiredService<IAddToProjectsJob>();
                addToProjectsBL.BeginJobAsync();
            }
        }
    }
}