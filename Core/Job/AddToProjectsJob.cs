using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Logging;

namespace Core.Job
{
    public class AddToProjectsJob : IAddToProjectsJob
    {
        private readonly IGithubRepoBL _githubRepoBL;
        private readonly ILogger<AddToProjectsJob> _logger;
        private readonly IProjectBL _projectBL;

        public AddToProjectsJob(IGithubRepoBL githubRepoBL, ILogger<AddToProjectsJob> logger, IProjectBL projectBL)
        {
            _githubRepoBL = githubRepoBL;
            _logger = logger;
            _projectBL = projectBL;
        }

        public async Task BeginJobAsync()
        {
            var reposResult = await _githubRepoBL.GetPinnedRepos(false);
            var projects = MakeReposIntoProjects(reposResult.Data);
            await _projectBL.UploadProjects(projects);
        }

        private IEnumerable<Project> MakeReposIntoProjects(IEnumerable<Repo> myRepos)
        {
            return myRepos
                .Select(myRepo => new Project
                    { Name = myRepo.Name, Description = myRepo.Description, GithubRepoDatabaseId = myRepo.DatabaseId })
                .ToList();
        }
    }
}