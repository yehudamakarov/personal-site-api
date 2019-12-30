using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface IProjectRepository
    {
        Task<IList<Project>> GetAllProjects();
        Task<Project> GetProjectById(string projectId);

        Task<Project> UploadProjectAsync(
            Project project,
            string projectName,
            string githubDatabaseId,
            string[] mergeFields
        );

        Task<Project> GetProjectByName(string projectName);
        Task<Project> UpdateProject(Project project);
        Task<IList<Project>> GetProjectsByTagId(string tagId);
    }
}