using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IJobStatusNotifier
    {
        Task PushGithubRepoFetcherJobStatusUpdate(Dictionary<string, JobStage> itemStatus,
            JobStage jobStage);
        Task PushCalculateTagCountsJobStatusUpdate(JobStage jobStage);
    }
}