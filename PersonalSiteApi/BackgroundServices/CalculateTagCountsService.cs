using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PersonalSiteApi.BackgroundServices
{
    public class CalculateTagCountsService : IHostedService
    {
        private const string ServiceName = nameof(CalculateTagCountsService);
        private readonly ILogger<CalculateTagCountsService> _logger;
        private readonly IServiceProvider _services;

        public CalculateTagCountsService(IServiceProvider services, ILogger<CalculateTagCountsService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var unused = new Timer(CalculateTagCounts, null, TimeSpan.FromSeconds(15), TimeSpan.FromHours(1));
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

        private void CalculateTagCounts(object state)
        {
            using (var scope = _services.CreateScope())
            {
                var calculateTagCountsJob = scope.ServiceProvider.GetRequiredService<ICalculateTagCountsJob>();
                calculateTagCountsJob.BeginJobAsync();
            }
        }
    }
}