using System;
using System.Threading.Tasks;
using Core.Enums.ResultReasons;
using Core.Interfaces;
using Core.Results.GithubRepo;
using Microsoft.Extensions.Logging;

namespace Core.BL
{
    public class GithubRepoBL : IGithubRepoBL
    {
        private readonly ILogger<GithubRepoBL> _logger;
        private readonly IRepoRepository _repoRepository;

        public GithubRepoBL(
            IRepoRepository repoRepository, ILogger<GithubRepoBL> logger)
        {
            _repoRepository = repoRepository;
            _logger = logger;
        }

        public async Task<PinnedReposResult> GetPinnedRepos()
        {
            try
            {
                var repos = await _repoRepository.GetPinnedReposAsync();
                var result = new PinnedReposResult
                    {Data = repos, Reason = PinnedReposResultReason.Success};
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Retrieving pinned repositories from Firebase failed.");
                var result = new PinnedReposResult {Reason = PinnedReposResultReason.Error};
                return result;
            }
        }
    }
}