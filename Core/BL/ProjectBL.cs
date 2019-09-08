using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Results;
using Core.Types;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.BL
{
    public class ProjectBL : IProjectBL
    {
        private readonly ILogger<ProjectBL> _logger;
        private readonly IProjectRepository _projectRepository;

        public ProjectBL(ILogger<ProjectBL> logger, IProjectRepository projectRepository)
        {
            _logger = logger;
            _projectRepository = projectRepository;
        }

        public Task<ProjectsResult> GetAllProjects()
        {
            throw new NotImplementedException();
        }

        public Task<ProjectResult> GetProjectByName(string projectName)
        {
            throw new NotImplementedException();
        }

        public async Task<ProjectsResult> UploadProjects(IEnumerable<Project> projects)
        {
            var uploadTasks = projects.Select(project =>
                _projectRepository.UploadProjectAsync(project, project.Name, project.GithubRepoDatabaseId));
            var initiatedUploadTasks =
                (from uploadTask in uploadTasks select AwaitUpload(uploadTask))
                .ToArray();
            var uploadedProjects = await Task.WhenAll(initiatedUploadTasks);
            _logger.LogInformation(
                "Completed uploading projects. {uploadedProjects}",
                JsonConvert.SerializeObject(uploadedProjects)
            );
            return new ProjectsResult
            {
                Data = new List<Project>(uploadedProjects),
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }

        private static async Task<Project> AwaitUpload(Task<Project> uploadTask)
        {
            return await uploadTask;
        }
    }
}