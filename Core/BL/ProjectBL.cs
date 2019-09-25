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

        public async Task<ProjectsResult> GetAllProjects()
        {
            var results = await _projectRepository.GetAllProjects();
            if (results.Count == 0)
                return new ProjectsResult
                {
                    Data = results,
                    Details = new ResultDetails
                    {
                        Message = "None were found.",
                        ResultStatus = ResultStatus.Warning
                    }
                };

            return new ProjectsResult
            {
                Data = results,
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }

        public async Task<ProjectResult> GetProjectById(string projectId)
        {
            var project = await _projectRepository.GetProjectById(projectId);
            if (project == null)
                return new ProjectResult
                {
                    Data = null,
                    Details = new ResultDetails
                    {
                        Message = $"No project with id {projectId} was found.",
                        ResultStatus = ResultStatus.Failure
                    }
                };

            return new ProjectResult
            {
                Data = project,
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }

        public async Task<ProjectsResult> UploadProjects((List<Project>, string[]) projectsAndMergeFields)
        {
            var (projects, mergeFields) = projectsAndMergeFields;

            var uploadTasks = projects.Select(project =>
                _projectRepository.UploadProjectAsync(project, project.ProjectName, project.GithubRepoDatabaseId,
                    mergeFields));
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

        public async Task<ProjectResult> GetProjectByName(string projectName)
        {
            var project = await _projectRepository.GetProjectByName(projectName);
            if (project == null)
                return new ProjectResult
                {
                    Data = null,
                    Details = new ResultDetails
                    {
                        Message = $"No project with name {projectName} was found.",
                        ResultStatus = ResultStatus.Failure
                    }
                };

            return new ProjectResult
            {
                Data = project,
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }

        private static async Task<Project> AwaitUpload(Task<Project> uploadTask)
        {
            return await uploadTask;
        }
    }
}