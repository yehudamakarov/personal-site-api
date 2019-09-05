using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Logging;

namespace Core.BL
{
    public class AddToProjectsBL : IAddToProjectsBL
    {
        private readonly IRepoRepository _repoRepository;
        private readonly ILogger<AddToProjectsBL> _logger;

        public AddToProjectsBL(IRepoRepository repoRepository, ILogger<AddToProjectsBL> logger)
        {
            _repoRepository = repoRepository;
            _logger = logger;
        }

        public async Task BeginJobAsync()
        {
            var myRepos = await _repoRepository.GetPinnedReposAsync();
            var projects = await MakeReposIntoProjects(myRepos);
        }

        private async Task<Project> MakeReposIntoProjects(IEnumerable<Repo> myRepos)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Project
    {
    }
}