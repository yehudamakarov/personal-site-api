using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface IRepoInfrastructure
    {
        Task<IEnumerable<Repo>> FetchPinnedReposAsync();
    }
}