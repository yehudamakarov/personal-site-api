using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Notification.RepoSync
{
    public class RepoSyncNotificationHub : Hub<IGithubRepoFetcherNotification>
    {
        public async Task PushUpdate(Dictionary<string, string> itemStatus, string jobStatus)
        {
            await Clients.All.PushUpdate(itemStatus, jobStatus);
        }
    }
}