using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Results;
using Core.Types;
using Microsoft.Extensions.Logging;

namespace Core.BL
{
    public class RepoBL : IRepoBL
    {
        private readonly ILogger<RepoBL> _logger;
        private readonly IRepoRepository _repoRepository;

        public RepoBL(
            IRepoRepository repoRepository, ILogger<RepoBL> logger)
        {
            _repoRepository = repoRepository;
            _logger = logger;
        }

        public async Task<PinnedReposResult> GetPinnedRepos(bool onlyCurrent)
        {
            try
            {
                var repos = await _repoRepository.GetPinnedReposAsync(onlyCurrent);
                var mostRecentFirst = repos.OrderByDescending(repo => repo.UpdatedAt);
                var result = new PinnedReposResult
                {
                    Data = new List<PinnedRepo>(mostRecentFirst),
                    Details = new ResultDetails { ResultStatus = ResultStatus.Success }
                };
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Retrieving pinned repositories from Firebase failed.");
                var result = new PinnedReposResult
                {
                    Details = new ResultDetails { ResultStatus = ResultStatus.Failure }
                };
                return result;
            }
        }
    }
}