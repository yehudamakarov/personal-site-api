using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface IGithubInfrastructure
    {
        Task<IEnumerable<PinnedRepo>> FetchPinnedReposAsync();
    }
}