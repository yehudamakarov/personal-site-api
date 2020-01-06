using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IJobStatusNotifier
    {
        Task PushGithubRepoFetcherJobStatusUpdate(Dictionary<string, GithubRepoFetcherJobStage> itemStatus,
            GithubRepoFetcherJobStage jobStatus);
    }
}