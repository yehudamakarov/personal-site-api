using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Notification.JobStatus
{
    public class JobStatusNotifier : IJobStatusNotifier
    {
        private readonly IHubContext<JobStatusUpdatesHub, IJobStatusUpdatesHub> _githubRepoFetcherHubContext;

        public JobStatusNotifier(
            IHubContext<JobStatusUpdatesHub, IJobStatusUpdatesHub> githubRepoFetcherHubContext
        )
        {
            _githubRepoFetcherHubContext = githubRepoFetcherHubContext;
        }


        public async Task PushGithubRepoFetcherJobStatusUpdate(Dictionary<string, GithubRepoFetcherJobStage> itemStatus,
            GithubRepoFetcherJobStage jobStatus)
        {
            await _githubRepoFetcherHubContext.Clients.All.PushGithubRepoFetcherJobStatusUpdate(
                new GithubRepoFetcherJobStatus
                {
                    ItemStatus = itemStatus,
                    JobStatus = jobStatus
                });
        }
    }
}