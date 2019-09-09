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
    }
}