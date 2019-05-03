using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core.BL
{
    public class RepoFetcherBL : IRepoFetcherBL
    {
        private readonly IRepoRepository _repoRepository;
        private readonly HttpClient _httpClient;

        public RepoFetcherBL(IRepoRepository repoRepository)
        {
            _repoRepository = repoRepository;
            _httpClient = new HttpClient();
        }

        public async Task BeginJobAsync()
        {
            var repos = await GetTopRepos();
            var timeStampedRepos = MarkWithTimestamp(repos);
            var uploadedRepos = await UploadToStorage(timeStampedRepos);
        }

        private static async Task<IEnumerable<Repo>> UploadToStorage(IEnumerable<Repo> repos)
        {
            var uploadTasks = repos.Select(UploadRepo)
                .ToArray();
            var uploadsInProgress = (from uploadTask in uploadTasks
                    select AwaitUpload(uploadTask))
                .ToArray();
            var uploadedRepos = await Task.WhenAll(uploadsInProgress);
            return uploadedRepos;
        }

        private static Task<Repo> UploadRepo(Repo repo)
        {
            throw new NotImplementedException();
        }

        private static async Task<Repo> AwaitUpload(Task<Repo> uploadTask)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<Repo> MarkWithTimestamp(IEnumerable<Repo> items)
        {
            IEnumerable<Repo> toMarkWithTimestamp = items.ToList();
            var timeFetched = DateTime.UtcNow;
            foreach (var item in toMarkWithTimestamp)
            {
                item.TimeFetched = timeFetched;
            }

            return toMarkWithTimestamp;
        }

        private async Task<IEnumerable<Repo>> GetTopRepos()
        {
            var resp = await _httpClient.GetAsync("repoUrl");

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException("The request to fetch repositories from GitHub failed.");

            var respContent = await resp.Content.ReadAsStringAsync();
            var repos = await TranslateTo<Repo>(respContent);
            return repos;
        }

        private static async Task<IEnumerable<T>> TranslateTo<T>(string content)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Repo
    {
        public DateTime TimeFetched { get; set; }
    }
}