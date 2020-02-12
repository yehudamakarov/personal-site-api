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
        Task PushMapTagJobStatusUpdate(string uniqueKey, TagResult tagResult, JobStage jobStage);
        Task PushRenameTagJobStatusUpdate(string uniqueKey, TagResult tagResult, JobStage jobStage);
        Task PushDeleteTagJobStatusUpdate(string uniqueKey, string tagId, JobStage jobStage);
    }
}