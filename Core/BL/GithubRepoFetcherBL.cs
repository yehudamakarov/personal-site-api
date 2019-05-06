using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Google.Api.Gax.Grpc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.BL
{
	public class GithubRepoFetcherBL : IGithubRepoFetcherBL
	{
		private readonly IGithubRepoInfrastructure _githubRepoInfrastructure;
		private readonly IGithubRepoRepository _githubRepoRepository;
		private readonly ILogger<GithubRepoFetcherBL> _logger;

		private string _jobStatus;
		private Dictionary<string, string> _itemStatus = new Dictionary<string, string>();
		

		public GithubRepoFetcherBL(
			IGithubRepoInfrastructure githubRepoInfrastructure,
			IGithubRepoRepository githubRepoRepository, ILogger<GithubRepoFetcherBL> logger
		)
		{
			_githubRepoInfrastructure = githubRepoInfrastructure;
			_githubRepoRepository = githubRepoRepository;
			_logger = logger;
		}

		public async Task BeginJobAsync()
		{
			_logger.LogInformation("Beginning job");
			UpdateJobStatus(JobUpdatesStage.PreparingDatabase);
			await MakeAllPinnedReposNonCurrent();

			UpdateJobStatus(JobUpdatesStage.Fetching);
			var repos = await FetchPinnedRepos();
			var reposList = repos.ToList();

			UpdateAllItemsStatus(JobUpdatesStage.Uploading, reposList);
			var timeStampedRepos = MarkWithTimestamp(reposList);
			var uploadedRepos = await UploadPinnedReposAsCurrent(timeStampedRepos);

			UpdateJobStatus(JobUpdatesStage.Done);
			_logger.LogInformation("Completed job");
		}

		private async Task<IEnumerable<Repo>> UploadPinnedReposAsCurrent( IEnumerable<Repo> repos )
		{
			_logger.LogInformation("Setting pinned as current to Firestore.");
			var reposList = repos.ToList();
			foreach (var repo in reposList)
			{
				_logger.LogInformation("Making {@repo} 'current'", repo);
				repo.Current = true;
			}

			return await UploadReposAsync(reposList);
		}

		private async Task<IEnumerable<Repo>> UploadReposAsync( IEnumerable<Repo> reposList )
		{
			var uploadTasks = reposList.Select(_githubRepoRepository.UploadRepoAsync);
			var initiatedUploadTasks =
				(from uploadTask in uploadTasks select AwaitUploadAndSendUpdate(uploadTask))
				.ToArray();
			var uploadedRepos = await Task.WhenAll(initiatedUploadTasks);
			_logger.LogInformation(
				"Completed uploading repos. {uploadedRepos}",
				JsonConvert.SerializeObject(uploadedRepos)
			);
			return uploadedRepos;
		}

		private async Task<Repo> AwaitUploadAndSendUpdate( Task<Repo> uploadTask )
		{
			var repo = await uploadTask;
			UpdateItemStatus(JobUpdatesStage.Done, repo);
			return repo;
		}
		private async Task<IEnumerable<Repo>> FetchPinnedRepos()
		{
			return await _githubRepoInfrastructure.FetchPinnedReposAsync();
		}

		private async Task<IEnumerable<Repo>> MakeAllPinnedReposNonCurrent()
		{
			_logger.LogInformation("Making all pinned repositories un-pinned.");
			var currentPinnedRepos = await _githubRepoRepository.GetCurrentPinnedRepos();
			var currentPinnedReposList = currentPinnedRepos.ToList();
			foreach (var currentPinnedRepo in currentPinnedReposList)
			{
				currentPinnedRepo.Current = false;
			}
			var nonCurrentRepos =
				await UploadReposAsync(currentPinnedReposList);
			_logger.LogInformation("Completed making all pinned repositories un-pinned.");
			return nonCurrentRepos;
		}

		private static IEnumerable<Repo> MarkWithTimestamp( IEnumerable<Repo> items )
		{
			IEnumerable<Repo> toMarkWithTimestamp = items.ToList();
			var timeFetched = DateTime.Now;
			foreach (var item in toMarkWithTimestamp)
			{
				item.TimeFetched = timeFetched;
			}

			return toMarkWithTimestamp;
		}


		private void UpdateItemStatus( JobUpdatesStage stage, Repo repo )
		{
			_itemStatus[repo.DatabaseId] = stage.ToString();
			SendUpdate();
		}

		private void SendUpdate()
		{
			throw new NotImplementedException();
		}

		private void UpdateAllItemsStatus( JobUpdatesStage stage, IEnumerable<Repo> repos )
		{
			foreach (var repo in repos)
			{
				_itemStatus[repo.DatabaseId] = stage.ToString();
			}
			UpdateJobStatus(stage);
		}

		private void UpdateJobStatus( JobUpdatesStage stage )
		{
			_jobStatus = stage.ToString();
		}
	}
}