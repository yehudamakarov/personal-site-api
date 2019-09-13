using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllProjects();
        Task<Project> GetProjectById(string projectId);
        Task<Project> UploadProjectAsync(Project project, string projectName, string githubDatabaseId,
            string[] mergeFields);

        Task<Project> GetProjectByName(string projectName);
    }
}