using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Notification.RepoSync
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RepoSyncNotificationHub : Hub<IGithubRepoFetcherNotification>
    {
        
    }
}