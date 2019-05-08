using System.Threading.Tasks;
using Core.Results.GithubRepo;

namespace Core.Interfaces
{
	public interface IGithubRepoBL
	{
		Task<PinnedReposResult> HandlePinnedRepos();
	}
}