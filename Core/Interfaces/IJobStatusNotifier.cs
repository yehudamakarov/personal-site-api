using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Results;
using Core.Types;

namespace Core.Interfaces
{
    public interface IJobStatusNotifier
    {
        Task PushGithubRepoFetcherJobStatusUpdate(Dictionary<string, JobStage> itemStatus,
            JobStage jobStage);
        Task PushCalculateTagCountsJobStatusUpdate(JobStage jobStage);
        Task PushMapTagJobStatusUpdate(TagResult tagResult, JobStage jobStage);
        Task PushRenameTagJobStatusUpdate(TagResult tagResult, JobStage jobStage);
    }
}