using System.Collections.Generic;
using System.Linq;
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
        private readonly CollectionReference _projectCollection;


        public ProjectRepository(IConfiguration configuration, ILogger<ProjectRepository> logger) : base(configuration)
        {
            _projectCollection = Db.Collection("projects");
            _logger = logger;
        }

        public async Task<IList<Project>> GetAllProjects()
        {
            var query = await _projectCollection.GetSnapshotAsync();
            return query.Documents.Select(documentSnapshot => documentSnapshot.ConvertTo<Project>()).ToList();
        }

        public async Task<Project> GetProjectById(string projectId)
        {
            var reference = _projectCollection.Document(projectId);
            var documentSnapshot = await reference.GetSnapshotAsync();
            return documentSnapshot?.ConvertTo<Project>();
        }

        /// <summary>
        ///     we don't want to overwrite anything besides the fields we have on the class. Since some initialized
        ///     properties may be null, we specify merge fields.
        /// </summary>
        /// <param name="project">
        ///     if there are any null property values that should NOT overwrite fields on the
        ///     documents; specify the non-null properties in the <paramref name="mergeFields" />mergeFields> param.
        /// </param>
        /// <param name="githubDatabaseId"></param>
        /// <param name="mergeFields"></param>
        /// <returns></returns>
        public async Task<Project> UploadProjectAsync(
            Project project,
            string githubDatabaseId,
            string[] mergeFields
        )
        {
            // Make document ref in collection
            var projectRef = _projectCollection.Document(githubDatabaseId);
            // write / update in Db
            var unused = await projectRef.SetAsync(project, SetOptions.MergeFields(mergeFields));
            // get result of write
            var snapshot = await projectRef.GetSnapshotAsync();
            // convert to model
            return snapshot.ConvertTo<Project>();
        }

        public async Task<Project> GetProjectByName(string projectName)
        {
            var snapshot = await _projectCollection.WhereEqualTo(nameof(Project.ProjectName), projectName)
                .GetSnapshotAsync();
            var result = snapshot.Select(documentSnapshot => documentSnapshot.ConvertTo<Project>()).FirstOrDefault();
            return result;
        }

        public async Task<Project> UpdateProject(Project project)
        {
            return await UploadProjectAsync(
                project,
                project.GithubRepoDatabaseId,
                new[] { nameof(Project.DeploymentUrl), nameof(Project.TagIds), nameof(Project.ProjectTitle) }
            );
        }

        public async Task<IList<Project>> GetProjectsByTagId(string tagId)
        {
            var snapshot = await _projectCollection.WhereArrayContains(nameof(Project.TagIds), tagId)
                .GetSnapshotAsync();
            return snapshot.Documents.Select(documentSnapshot => documentSnapshot.ConvertTo<Project>()).ToList();
        }
    }
}