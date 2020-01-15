using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Results;
using Core.Types;

namespace Core.Interfaces
{
    public interface IJobStatusUpdatesHub
    {
        Task PushGithubRepoFetcherJobStatusUpdate(GithubRepoFetcherJobStatus status);
        Task PushCalculateTagCountsJobStatusUpdate(CalculateTagCountsJobStatus status);
        Task PushMapTagJobStatusUpdate(MapTagJobStatus mapTagJobStatus);
    }

    public class CalculateTagCountsJobStatus : JobStatus { }

    public class JobStatus
    {
        public JobStage JobStage { get; set; }
    }

    public class GithubRepoFetcherJobStatus
    {
        public Dictionary<string, JobStage> ItemStatus { get; set; }
        public JobStage JobStage { get; set; }
    }

    public class MapTagJobStatus : JobStatus {
        public TagResult TagResult { get; set; }
    }
}