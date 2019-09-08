using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Results;
using Core.Types;

namespace Core.Interfaces
{
    public interface IProjectBL
    {
        Task<ProjectsResult> GetAllProjects();
        Task<ProjectResult> GetProjectByName(string projectName);
        Task<ProjectsResult> UploadProjects(IEnumerable<Project> projects);
    }
}