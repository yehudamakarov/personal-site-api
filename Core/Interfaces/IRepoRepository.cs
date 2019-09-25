using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface IRepoRepository
    {
        Task<PinnedRepo> UploadRepoAsync(PinnedRepo pinnedRepo);
        Task<IEnumerable<PinnedRepo>> GetPinnedReposAsync(bool onlyCurrent);
    }
}