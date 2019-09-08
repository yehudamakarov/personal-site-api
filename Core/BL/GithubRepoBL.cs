using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Results;
using Core.Types;
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

        public async Task<PinnedReposResult> GetPinnedRepos(bool onlyCurrent)
        {
            try
            {
                var repos = await _repoRepository.GetPinnedReposAsync(onlyCurrent);
                var result = new PinnedReposResult
                {
                    Data = new List<Repo>(repos), Details = new ResultDetails { ResultStatus = ResultStatus.Success }
                };
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Retrieving pinned repositories from Firebase failed.");
                var result = new PinnedReposResult
                    { Details = new ResultDetails { ResultStatus = ResultStatus.Failure } };
                return result;
            }
        }
    }
}