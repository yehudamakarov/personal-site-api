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

    public class CalculateTagCountsJobStatus : IJobStatus<TagResult>
    {
        public JobStage JobStage { get; set; }
        public TagResult Item { get; set; }
    }

    public interface IJobStatus<T>
    {
        JobStage JobStage { get; set; }
        T Item { get; set; }
    }

    public class GithubRepoFetcherJobStatus : IJobStatus<Dictionary<string, JobStage>>
    {
        public JobStage JobStage { get; set; }
        public Dictionary<string, JobStage> Item { get; set; }
    }

    public class MapTagJobStatus : IJobStatus<TagResult>
    {
        public JobStage JobStage { get; set; }
        public TagResult Item { get; set; }

        public MapTagJobStatus() { }

        public MapTagJobStatus(string tagId, JobStage jobStage)
        {
            JobStage = JobStage.Error;
            Item = new TagResult
            {
                Data = new Tag { TagId = tagId },
                Details = new ResultDetails
                {
                    Message = $"There was a problem mapping {tagId}",
                    ResultStatus = ResultStatus.Failure
                }
            };
        }
    }
}