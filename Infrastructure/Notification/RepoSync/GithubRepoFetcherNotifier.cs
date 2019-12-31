using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Notification.RepoSync
{
    public class GithubRepoFetcherNotifier : IGithubRepoFetcherNotifier
    {
        private readonly IHubContext<RepoSyncNotificationHub, IGithubRepoFetcherNotification> _hubContext;

        public GithubRepoFetcherNotifier(
            IHubContext<RepoSyncNotificationHub, IGithubRepoFetcherNotification> hubContext
        )
        {
            _hubContext = hubContext;
        }


        public async Task PushUpdate(Dictionary<string, string> itemStatus, string jobStatus)
        {
            await _hubContext.Clients.All.PushUpdate(itemStatus, jobStatus);
            await Task.Delay(200);
        }
    }
}