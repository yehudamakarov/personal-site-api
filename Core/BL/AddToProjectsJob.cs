using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Logging;

namespace Core.BL
{
    public class AddToProjectsJob : IAddToProjectsJob
    {
        private readonly IGithubRepoBL _githubRepoBL;
        private readonly ILogger<AddToProjectsJob> _logger;

        public AddToProjectsJob(IGithubRepoBL githubRepoBL, ILogger<AddToProjectsJob> logger)
        {
            _githubRepoBL = githubRepoBL;
            _logger = logger;
        }

        public async Task BeginJobAsync()
        {
            var reposResult = await _githubRepoBL.GetPinnedRepos();
            var projects = await MakeReposIntoProjects(reposResult.Data);
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