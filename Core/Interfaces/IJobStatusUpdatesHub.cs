using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IJobStatusUpdatesHub
    {
        Task PushGithubRepoFetcherJobStatusUpdate(GithubRepoFetcherJobStatus status);
    }

    public class GithubRepoFetcherJobStatus
    {
        public Dictionary<string, GithubRepoFetcherJobStage> ItemStatus { get; set; }
        public GithubRepoFetcherJobStage JobStatus { get; set; }
    }
}