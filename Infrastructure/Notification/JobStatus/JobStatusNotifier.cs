using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Interfaces;
using Core.Results;
using Core.Types;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Notification.JobStatus
{
    public class JobStatusNotifier : IJobStatusNotifier
    {
        private readonly IHubContext<JobStatusUpdatesHub, IJobStatusUpdatesHub> _hubContext;

        public JobStatusNotifier(IHubContext<JobStatusUpdatesHub, IJobStatusUpdatesHub> hubContext)
        {
            _hubContext = hubContext;
        }


        public async Task PushGithubRepoFetcherJobStatusUpdate(
            Dictionary<string, JobStage> itemStatus,
            JobStage jobStage
        )
        {
            await _hubContext.Clients.All.PushGithubRepoFetcherJobStatusUpdate(
                new GithubRepoFetcherJobStatus { Item = itemStatus, JobStage = jobStage }
            );
        }

        public async Task PushCalculateTagCountsJobStatusUpdate(JobStage jobStage)
        {
            await _hubContext.Clients.All.PushCalculateTagCountsJobStatusUpdate(
                new CalculateTagCountsJobStatus { JobStage = jobStage }
            );
        }

        public async Task PushMapTagJobStatusUpdate(TagResult tagResult, JobStage jobStage)
        {
            await _hubContext.Clients.All.PushMapTagJobStatusUpdate(
                new MapTagJobStatus { Item = tagResult, JobStage = jobStage }
            );
        }

        public async Task PushRenameTagJobStatusUpdate(TagResult tagResult, JobStage jobStage)
        {
            await _hubContext.Clients.All.PushRenameTagJobStatusUpdate(
                new RenameTagJobStatus { Item = tagResult, JobStage = jobStage }
            );
        }
    }
}