using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.BL
{
	public class GithubRepoFetcherBL : IGithubRepoFetcherBL
	{
		private readonly IGithubRepoInfrastructure _githubRepoInfrastructure;
		private readonly IGithubRepoRepository _githubRepoRepository;
		private readonly ILogger<GithubRepoFetcherBL> _logger;

		public GithubRepoFetcherBL(
			IGithubRepoInfrastructure githubRepoInfrastructure,
			IGithubRepoRepository githubRepoRepository, ILogger<GithubRepoFetcherBL> logger )
		{
			_githubRepoInfrastructure = githubRepoInfrastructure;
			_githubRepoRepository = githubRepoRepository;
			_logger = logger;
		}

		public async Task BeginJobAsync()
		{
			_logger.LogInformation("Beginning job");
			await MakeAllPinnedReposNonCurrent();
			var repos = await FetchPinnedRepos();
			var timeStampedRepos = MarkWithTimestamp(repos);
			var uploadedRepos = await UploadPinnedReposAsCurrent(timeStampedRepos);
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

			return await _githubRepoRepository.UploadReposAsync(reposList);
		}

		private async Task<IEnumerable<Repo>> MakeAllPinnedReposNonCurrent()
		{
			_logger.LogInformation("Making all pinned repositories un-pinned.");
			var currentPinnedRepos = await _githubRepoRepository.GetCurrentPinnedRepos();
			var currentPinnedReposList = currentPinnedRepos.ToList();
			_logger.LogInformation(
				"Making 'current' repositories 'non-current' in collection {currentPinnedRepos}.",
				JsonConvert.SerializeObject(currentPinnedRepos)
			);
			foreach (var currentPinnedRepo in currentPinnedReposList)
			{
				currentPinnedRepo.Current = false;
			}
			_logger.LogInformation(
				"Completed making 'current' repositories 'non-current' in collection {currentPinnedRepos}.",
				JsonConvert.SerializeObject(currentPinnedRepos)
			);
			_logger.LogInformation(
				"Setting 'non-current' {currentPinnedRepos} to Firestore.",
				JsonConvert.SerializeObject(currentPinnedRepos)
			);
			var nonCurrentRepos =
				await _githubRepoRepository.UploadReposAsync(currentPinnedReposList);
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

		private async Task<IEnumerable<Repo>> FetchPinnedRepos()
		{
			return await _githubRepoInfrastructure.FetchPinnedReposAsync();
		}
	}
}