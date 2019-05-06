using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Logging;

namespace Core.BL
{
    public class GithubRepoFetcherBL : IGithubRepoFetcherBL
    {
        private readonly IGithubRepoInfrastructure _githubRepoInfrastructure;
        private readonly IGithubRepoRepository _githubRepoRepository;
        private readonly ILogger<GithubRepoFetcherBL> _logger;

        public GithubRepoFetcherBL(IGithubRepoInfrastructure githubRepoInfrastructure,
            IGithubRepoRepository githubRepoRepository, ILogger<GithubRepoFetcherBL> logger)
        {
            _githubRepoInfrastructure = githubRepoInfrastructure;
            _githubRepoRepository = githubRepoRepository;
            _logger = logger;
        }

        public async Task BeginJobAsync()
        {
            await MakeAllPinnedReposNonCurrent();
            var repos = await FetchPinnedRepos();
            var timeStampedRepos = MarkWithTimestamp(repos);
            var uploadedRepos = await SetPinnedReposAsCurrent(timeStampedRepos);
        }

        private async Task<IEnumerable<Repo>> SetPinnedReposAsCurrent(IEnumerable<Repo> repos)
        {
            var reposList = repos.ToList();
            foreach (var repo in reposList)
            {
                repo.Current = true;
            }

            return await _githubRepoRepository.UploadReposAsync(reposList);
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

            var nonCurrentRepos = await _githubRepoRepository.UploadReposAsync(currentPinnedReposList);
            _logger.LogInformation("Completed making all pinned repositories un-pinned.");
            return nonCurrentRepos;
        }


        private static IEnumerable<Repo> MarkWithTimestamp(IEnumerable<Repo> items)
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