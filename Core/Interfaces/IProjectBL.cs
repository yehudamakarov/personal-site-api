using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Results;
using Core.Types;

namespace Core.Interfaces
{
    public interface IProjectBL
    {
        Task<ProjectsResult> GetAllProjects();
        Task<ProjectResult> GetProjectById(string projectId);
        Task<ProjectsResult> UploadProjects((List<Project>, string[]) projects);
        Task<ProjectResult> GetProjectByName(string projectName);
        Task<ProjectResult> UpdateProject(Project project);
        Task<ProjectsResult> GetProjectsByTagId(string tagId);
    }
}