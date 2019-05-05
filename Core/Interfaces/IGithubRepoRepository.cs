using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface IGithubRepoRepository
    {
        Task<Repo> UploadRepoAsync(Repo repo);
        Task<IEnumerable<Repo>> GetCurrentPinnedRepos();
        Task<IEnumerable<Repo>> UploadReposAsync(IEnumerable<Repo> repos);
    }
}