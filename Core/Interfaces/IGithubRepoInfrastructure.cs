using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
	public interface IGithubRepoInfrastructure
	{
		Task<IEnumerable<Repo>> FetchPinnedReposAsync();
	}
}