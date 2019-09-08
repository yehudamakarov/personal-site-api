using System.Threading.Tasks;
using Core.Results;

namespace Core.Interfaces
{
    public interface IGithubRepoBL
    {
        Task<PinnedReposResult> GetPinnedRepos(bool onlyCurrent);
    }
}