using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Enums;
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
        private readonly ITagBL _tagBL;

        public ProjectBL(ILogger<ProjectBL> logger, IProjectRepository projectRepository, ITagBL tagBL)
        {
            _logger = logger;
            _projectRepository = projectRepository;
            _tagBL = tagBL;
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

            var uploadedProjects = await UploadProjects(projects, mergeFields);
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
            return project == null
                ? new ProjectResult
                {
                    Data = null,
                    Details = new ResultDetails
                    {
                        Message = $"No project with name {projectName} was found.",
                        ResultStatus = ResultStatus.Failure
                    }
                }
                : new ProjectResult
                {
                    Data = project,
                    Details = new ResultDetails { ResultStatus = ResultStatus.Success }
                };
        }

        public async Task<ProjectResult> UpdateProject(Project project)
        {
            try
            {
                await UpdateTagIdsOfProject(project);
                var updatedProject = await _projectRepository.UpdateProject(project);
                return new ProjectResult
                {
                    Data = updatedProject,
                    Details = new ResultDetails
                    {
                        Message = "Success",
                        ResultStatus = ResultStatus.Success
                    }
                };
            }
            catch (Exception exception)
            {
                const string message = "The Project may not have been saved.";
                _logger.LogError(exception, message);
                return new ProjectResult
                {
                    Data = project,
                    Details = new ResultDetails
                    {
                        Message = message,
                        ResultStatus = ResultStatus.Failure
                    }
                };
            }
        }

        private async Task<Project[]> UploadProjects(IEnumerable<Project> projects, string[] mergeFields)
        {
            var uploadTasks = projects.Select(project =>
                UploadProjectAsync(project, mergeFields));
            var initiatedUploadTasks =
                (from uploadTask in uploadTasks select AwaitTask(uploadTask))
                .ToArray();
            var uploadedProjects = await Task.WhenAll(initiatedUploadTasks);
            return uploadedProjects;
        }

        private Task<Project> UploadProjectAsync(Project project, string[] mergeFields)
        {
            return _projectRepository.UploadProjectAsync(project, project.ProjectName, project.GithubRepoDatabaseId,
                mergeFields);
        }

        private async Task UpdateTagIdsOfProject(Project newProject)
        {
            var currentProject = await _projectRepository.GetProjectById(newProject.GithubRepoDatabaseId);
            await UpdateTags(currentProject.TagIds, newProject.TagIds);
        }

        private async Task UpdateTags(IReadOnlyCollection<string> currentProjectTagIds,
            IReadOnlyCollection<string> newProjectTagIds)
        {
            await CreateOrFindTags(newProjectTagIds);
            await AdjustTagCounts(currentProjectTagIds, newProjectTagIds);
        }

        private async Task AdjustTagCounts(IReadOnlyCollection<string> currentProjectTagIds,
            IReadOnlyCollection<string> newProjectTagIds)
        {
            var toIncrement = newProjectTagIds.Except(currentProjectTagIds).ToList();
            var toDecrement = currentProjectTagIds.Except(newProjectTagIds).ToList();
            await _tagBL.UpdateTagCounts(toIncrement, TagCountUpdates.Increment, 1);
            await _tagBL.UpdateTagCounts(toDecrement, TagCountUpdates.Decrement, 1);
        }

        private async Task CreateOrFindTags(IEnumerable<string> newProjectTagIds)
        {
            var createOrFindTagTasks = newProjectTagIds.Select(tagId => _tagBL.CreateOrFindByTagId(tagId));
            var initiatedCreateOrFindTagsTasks =
                (from findTagsTask in createOrFindTagTasks select AwaitTask(findTagsTask)).ToArray();
            await Task.WhenAll(initiatedCreateOrFindTagsTasks);
        }

        private static async Task<T> AwaitTask<T>(Task<T> task)
        {
            return await task;
        }
    }
}