using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface IRepoFetcherBL
	{
		Task BeginJobAsync();
	}
}