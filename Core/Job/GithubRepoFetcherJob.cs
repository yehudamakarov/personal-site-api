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
        private readonly IGithubInfrastructure _githubInfrastructure;
        private readonly IGithubRepoFetcherNotifier _githubRepoFetcherNotifier;
        private readonly Dictionary<string, string> _itemStatus = new Dictionary<string, string>();
        private readonly ILogger<GithubRepoFetcherJob> _logger;
        private readonly IRepoRepository _repoRepository;

        private string _jobStatus;

        public GithubRepoFetcherJob(
            IGithubInfrastructure githubInfrastructure,
            IRepoRepository repoRepository,
            ILogger<GithubRepoFetcherJob> logger,
            IGithubRepoFetcherNotifier githubRepoFetcherNotifier
        )
        {
            _githubInfrastructure = githubInfrastructure;
            _repoRepository = repoRepository;
            _logger = logger;
            _githubRepoFetcherNotifier = githubRepoFetcherNotifier;
        }

        public async Task BeginJobAsync()
        {
            await UpdateJobStatus(JobUpdatesStage.Fetching);
            var repos = await _githubInfrastructure.FetchPinnedReposAsync();
            var reposList = repos.ToList();

            _logger.LogInformation("Beginning job");
            await UpdateJobStatus(JobUpdatesStage.PreparingDatabase);
            await MakeAllPinnedReposNonCurrent();

            await UpdateAllItemsStatus(JobUpdatesStage.Uploading, reposList);
            var timeStampedRepos = MarkWithTimestamp(reposList);
            var unused = await UploadPinnedReposAsCurrent(timeStampedRepos);

            await UpdateJobStatus(JobUpdatesStage.Done);
            _logger.LogInformation("Completed job");
        }

        private async Task<IEnumerable<PinnedRepo>> UploadPinnedReposAsCurrent(IEnumerable<PinnedRepo> repos)
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

        private async Task<IEnumerable<PinnedRepo>> UploadReposAsync(IEnumerable<PinnedRepo> reposList)
        {
            // faster
            // var uploadTasks = reposList.Select(_repoRepository.UploadRepoAsync);
            // var initiatedUploadTasks =
            //     (from uploadTask in uploadTasks select AwaitUploadAndSendUpdate(uploadTask)).ToArray();
            // var uploadedRepos = await Task.WhenAll(initiatedUploadTasks);
            // _logger.LogInformation(
            //     "Completed uploading repos. {uploadedRepos}",
            //     JsonConvert.SerializeObject(uploadedRepos)
            // );
            // return uploadedRepos;
            var uploadedRepos = new List<PinnedRepo>();
            foreach (var pinnedRepo in reposList)
            {
                var result = await AwaitUploadAndSendUpdate(_repoRepository.UploadRepoAsync(pinnedRepo));
                uploadedRepos.Add(result);
            }
            _logger.LogInformation(
                "Completed uploading repos. {uploadedReposNames}",
                string.Join(" | ", uploadedRepos.Select(repo => repo.Name))
            );
            return uploadedRepos;
        }

        private async Task<PinnedRepo> AwaitUploadAndSendUpdate(Task<PinnedRepo> uploadTask)
        {
            var repo = await uploadTask;
            await UpdateItemStatus(JobUpdatesStage.Done, repo);
            await Task.Delay(400);
            return repo;
        }

        private async Task<IEnumerable<PinnedRepo>> MakeAllPinnedReposNonCurrent()
        {
            _logger.LogInformation("Making all pinned repositories un-pinned.");
            var currentPinnedRepos = await _repoRepository.GetPinnedReposAsync(true);
            var currentPinnedReposList = currentPinnedRepos.ToList();
            foreach (var currentPinnedRepo in currentPinnedReposList)
                currentPinnedRepo.Current = false;
            var nonCurrentRepos = await UploadReposAsync(currentPinnedReposList);
            _logger.LogInformation("Completed making all pinned repositories un-pinned.");
            return nonCurrentRepos;
        }

        private static IEnumerable<PinnedRepo> MarkWithTimestamp(IEnumerable<PinnedRepo> items)
        {
            IEnumerable<PinnedRepo> toMarkWithTimestamp = items.ToList();
            var timeFetched = DateTime.Now;
            foreach (var item in toMarkWithTimestamp) item.TimeFetched = timeFetched;

            return toMarkWithTimestamp;
        }

        private async Task UpdateItemStatus(JobUpdatesStage stage, PinnedRepo pinnedRepo)
        {
            _itemStatus[pinnedRepo.DatabaseId] = stage.ToString();
            await _githubRepoFetcherNotifier.PushUpdate(_itemStatus, _jobStatus);
        }

        private async Task UpdateAllItemsStatus(JobUpdatesStage stage, IEnumerable<PinnedRepo> repos)
        {
            foreach (var repo in repos) _itemStatus[repo.DatabaseId] = stage.ToString();
            await UpdateJobStatus(stage);
        }

        private async Task UpdateJobStatus(JobUpdatesStage stage)
        {
            _jobStatus = stage.ToString();
            await _githubRepoFetcherNotifier.PushUpdate(_itemStatus, _jobStatus);
        }
    }
}