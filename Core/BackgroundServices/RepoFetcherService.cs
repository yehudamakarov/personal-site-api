using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Core.BackgroundServices
{
	public class RepoFetcherService : IHostedService
	{
		private readonly IRepoFetcherBL _repoFetcherBL;
		private Timer _timer;

		public RepoFetcherService( IRepoFetcherBL repoFetcherBL)
		{
			_repoFetcherBL = repoFetcherBL;
		}

		public Task StartAsync( CancellationToken cancellationToken )
		{
			_timer = new Timer(FetchAndUploadRepos, null, TimeSpan.Zero, TimeSpan.FromHours(6));
			return Task.CompletedTask;
		}

		private void FetchAndUploadRepos( object state )
		{
			throw new NotImplementedException();
		}

		public Task StopAsync( CancellationToken cancellationToken )
		{
			throw new NotImplementedException();
		}
	}
}