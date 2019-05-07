using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Notification.RepoSync
{
    public class RepoSyncNotifier : IRepoSyncNotifier
    {
        private readonly IHubContext<RepoSyncNotificationHub, IRepoSyncNotification> _hubContext;

        public RepoSyncNotifier(IHubContext<RepoSyncNotificationHub, IRepoSyncNotification> hubContext)
        {
            _hubContext = hubContext;
        }


        public async Task PushUpdate(Dictionary<string, string> itemStatus, string jobStatus)
        {
            await _hubContext.Clients.All.PushUpdate(itemStatus, jobStatus);
        }
    }
}