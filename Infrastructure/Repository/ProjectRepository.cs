using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
    public class ProjectRepository : RepositoryBase, IProjectRepository
    {
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(IConfiguration configuration, ILogger<ProjectRepository> logger) : base(configuration)
        {
            _logger = logger;
        }

        public Task<List<Project>> GetAllProjects()
        {
            throw new NotImplementedException();
        }

        public Task<Project> GetProjectByName(string projectName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     sets a project to the firestore with an id from it's source pinned repo
        /// </summary>
        /// <param name="project"></param>
        /// <param name="projectName"></param>
        /// <param name="githubDatabaseId"></param>
        /// <returns></returns>
        public async Task<Project> UploadProjectAsync(Project project, string projectName, string githubDatabaseId)
        {
            _logger.LogInformation("Beginning upload of {projectName}", projectName);

            // Get collection ref
            var projectsRef = Db.Collection("projects");

            // Make document ref in collection
            var projectRef = projectsRef.Document(githubDatabaseId);

            // write / update in Db
            var unused = await projectRef.SetAsync(project, SetOptions.MergeAll);

            // get result of write
            var snapshot = await projectRef.GetSnapshotAsync();

            // convert to model
            var upLoadedProject = snapshot.ConvertTo<Project>();

            return upLoadedProject;
        }
    }
}