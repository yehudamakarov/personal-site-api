using System;
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
        Task PushRenameTagJobStatusUpdate(RenameTagJobStatus renameTagJobStatus);
    }

    public interface IJobStatus<T>
    {
        string UniqueKey { get; set; }
        JobStage JobStage { get; set; }
        T Item { get; set; }
    }

    public class CalculateTagCountsJobStatus : IJobStatus<TagResult>
    {
        public string UniqueKey { get; set; }
        public JobStage JobStage { get; set; }
        public TagResult Item { get; set; }
    }

    public class GithubRepoFetcherJobStatus : IJobStatus<Dictionary<string, JobStage>>
    {
        public string UniqueKey { get; set; }
        public JobStage JobStage { get; set; }
        public Dictionary<string, JobStage> Item { get; set; }
    }

    public class RenameTagJobStatus : IJobStatus<TagResult>
    {
        public string UniqueKey { get; set; }
        public JobStage JobStage { get; set; }
        public TagResult Item { get; set; }
    }

    public class MapTagJobStatus : IJobStatus<TagResult>
    {
        public string UniqueKey { get; set; }
        public JobStage JobStage { get; set; }
        public TagResult Item { get; set; }

        public MapTagJobStatus() { }

        public MapTagJobStatus(string tagId, JobStage jobStage)
        {
            switch (jobStage)
            {
                case JobStage.None:
                    break;
                case JobStage.PreparingDatabase:
                    break;
                case JobStage.FetchingFromGithub:
                    break;
                case JobStage.CountingTagged:
                    break;
                case JobStage.UploadingToDatabase:
                    break;
                case JobStage.InProgress:
                    break;
                case JobStage.Done:
                    break;
                case JobStage.Warning:
                    break;
                case JobStage.Error:
                {
                    JobStage = jobStage;
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(jobStage), jobStage, null);
            }
        }
    }
}