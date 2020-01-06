using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Notification.JobStatus
{
    [Authorize(Roles = Roles.Administrator)]
    public class JobStatusUpdatesHub : Hub<IJobStatusUpdatesHub> { }
}