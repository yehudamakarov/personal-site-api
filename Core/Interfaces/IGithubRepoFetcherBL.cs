using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface IGithubRepoFetcherBL
	{
		Task BeginJobAsync();
	}
}