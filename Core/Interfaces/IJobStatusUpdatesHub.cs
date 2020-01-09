using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IJobStatusUpdatesHub
    {
        Task PushGithubRepoFetcherJobStatusUpdate(GithubRepoFetcherJobStatus status);
        Task PushCalculateTagCountsJobStatusUpdate(CalculateTagCountsJobStatus status);
    }

    public class CalculateTagCountsJobStatus
    {
        public JobStage JobStage { get; set; }
    }

    public class GithubRepoFetcherJobStatus
    {
        public Dictionary<string, JobStage> ItemStatus { get; set; }
        public JobStage JobStage { get; set; }
    }
}