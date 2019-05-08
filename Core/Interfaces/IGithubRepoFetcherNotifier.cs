using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IGithubRepoFetcherNotifier
    {
        Task PushUpdate(Dictionary<string, string> itemStatus, string jobStatus);
    }
}