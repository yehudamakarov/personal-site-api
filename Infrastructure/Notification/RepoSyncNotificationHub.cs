using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Notification
{
	public class RepoSyncNotificationHub : Hub<IRepoSyncNotification>, IRepoSyncNotificationHub
	{
		
	}

	public interface IRepoSyncNotificationHub
	{
		Task SendUpdate( Dictionary<string, string> itemStatus, JobUpdatesStage jobStatus; 
	}

	public interface IRepoSyncNotification
	{
		Task SendUpdate( Dictionary<string, string> itemStatus, JobUpdatesStage jobStatus );
	}
}