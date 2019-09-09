using System.Threading.Tasks;
using Core.Results;

namespace Core.Interfaces
{
    public interface IRepoBL
    {
        Task<PinnedReposResult> GetPinnedRepos(bool onlyCurrent);
    }
}