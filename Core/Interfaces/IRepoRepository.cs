using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
	public interface IRepoRepository
	{
		Task<Repo> UploadRepoAsync( Repo repo );
		Task<IEnumerable<Repo>> GetPinnedReposAsync();
	}
}