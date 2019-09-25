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
        private readonly ILogger<AddToProjectsJob> _logger;
        private readonly IProjectBL _projectBL;
        private readonly IRepoBL _repoBL;

        public AddToProjectsJob(IRepoBL repoBL, ILogger<AddToProjectsJob> logger, IProjectBL projectBL)
        {
            _repoBL = repoBL;
            _logger = logger;
            _projectBL = projectBL;
        }

        public async Task BeginJobAsync()
        {
            var reposResult = await _repoBL.GetPinnedRepos(false);
            var projectsAndMergeFields = MakeReposIntoProjects(reposResult.Data);
            await _projectBL.UploadProjects(projectsAndMergeFields);
        }

        /// <summary>
        /// merge fields are generated here, because we only want to merge what is collected from the job's fetch.
        /// </summary>
        /// <param name="myRepos"></param>
        /// <returns></returns>
        private (List<Project>, string[]) MakeReposIntoProjects(IEnumerable<PinnedRepo> myRepos)
        {
            var projects = myRepos
                .Select(myRepo => new Project
                {
                    // Id
                    GithubRepoDatabaseId = myRepo.DatabaseId,

                    // Fields
                    ProjectName = myRepo.Name,
                    ProjectDescription = myRepo.Description,
                    IsPinnedRepo = true,
                    GithubUrl = myRepo.Url,
                    UpdatedAt = myRepo.UpdatedAt,
                    CreatedAt = myRepo.CreatedAt
                })
                .ToList();
            // only merge by fields
            var mergeFields = new[]
            {
                nameof(Project.ProjectName),
                nameof(Project.ProjectDescription),
                nameof(Project.IsPinnedRepo),
                nameof(Project.GithubUrl), 
                nameof(Project.UpdatedAt), 
                nameof(Project.CreatedAt), 
            };
            return (projects, mergeFields);
        }
    }
}