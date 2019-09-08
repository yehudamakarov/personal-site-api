using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.Job
{
    public class GithubRepoFetcherJob : IGithubRepoFetcherJob
    {
        private readonly IGithubRepoFetcherNotifier _githubRepoFetcherNotifier;
        private readonly IGithubRepoInfrastructure _githubRepoInfrastructure;
        private readonly Dictionary<string, string> _itemStatus = new Dictionary<string, string>();
        private readonly ILogger<GithubRepoFetcherJob> _logger;
        private readonly IRepoRepository _repoRepository;

        private string _jobStatus;

        public GithubRepoFetcherJob(
            IGithubRepoInfrastructure githubRepoInfrastructure,
            IRepoRepository repoRepository, ILogger<GithubRepoFetcherJob> logger,
            IGithubRepoFetcherNotifier githubRepoFetcherNotifier
        )
        {
            _githubRepoInfrastructure = githubRepoInfrastructure;
            _repoRepository = repoRepository;
            _logger = logger;
            _githubRepoFetcherNotifier = githubRepoFetcherNotifier;
        }

        public async Task BeginJobAsync()
        {
            _logger.LogInformation("Beginning job");
            UpdateJobStatus(JobUpdatesStage.PreparingDatabase);
            await MakeAllPinnedReposNonCurrent();

            UpdateJobStatus(JobUpdatesStage.Fetching);
            var repos = await _githubRepoInfrastructure.FetchPinnedReposAsync();
            var reposList = repos.ToList();

            UpdateAllItemsStatus(JobUpdatesStage.Uploading, reposList);
            var timeStampedRepos = MarkWithTimestamp(reposList);
            var unused = await UploadPinnedReposAsCurrent(timeStampedRepos);

            UpdateJobStatus(JobUpdatesStage.Done);
            _logger.LogInformation("Completed job");
        }

        private async Task<IEnumerable<Repo>> UploadPinnedReposAsCurrent(IEnumerable<Repo> repos)
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

        private async Task<IEnumerable<Repo>> UploadReposAsync(IEnumerable<Repo> reposList)
        {
            var uploadTasks = reposList.Select(_repoRepository.UploadRepoAsync);
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

        private async Task<Repo> AwaitUploadAndSendUpdate(Task<Repo> uploadTask)
        {
            var repo = await uploadTask;
            UpdateItemStatus(JobUpdatesStage.Done, repo);
            return repo;
        }

        private async Task<IEnumerable<Repo>> MakeAllPinnedReposNonCurrent()
        {
            _logger.LogInformation("Making all pinned repositories un-pinned.");
            var currentPinnedRepos = await _repoRepository.GetPinnedReposAsync(true);
            var currentPinnedReposList = currentPinnedRepos.ToList();
            foreach (var currentPinnedRepo in currentPinnedReposList)
                currentPinnedRepo.Current = false;
            var nonCurrentRepos =
                await UploadReposAsync(currentPinnedReposList);
            _logger.LogInformation("Completed making all pinned repositories un-pinned.");
            return nonCurrentRepos;
        }

        private static IEnumerable<Repo> MarkWithTimestamp(IEnumerable<Repo> items)
        {
            IEnumerable<Repo> toMarkWithTimestamp = items.ToList();
            var timeFetched = DateTime.Now;
            foreach (var item in toMarkWithTimestamp) item.TimeFetched = timeFetched;

            return toMarkWithTimestamp;
        }

        private void UpdateItemStatus(JobUpdatesStage stage, Repo repo)
        {
            _itemStatus[repo.DatabaseId] = stage.ToString();
            _githubRepoFetcherNotifier.PushUpdate(_itemStatus, _jobStatus);
        }

        private void UpdateAllItemsStatus(JobUpdatesStage stage, IEnumerable<Repo> repos)
        {
            foreach (var repo in repos) _itemStatus[repo.DatabaseId] = stage.ToString();
            UpdateJobStatus(stage);
        }

        private void UpdateJobStatus(JobUpdatesStage stage)
        {
            _jobStatus = stage.ToString();
            _githubRepoFetcherNotifier.PushUpdate(_itemStatus, _jobStatus);
        }
    }
}