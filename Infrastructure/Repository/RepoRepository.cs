using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
    public class RepoRepository : RepositoryBase, IRepoRepository
    {
        private readonly ILogger<RepoRepository> _logger;

        public RepoRepository(
            IConfiguration configuration,
            ILogger<RepoRepository> logger
        ) : base(configuration)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Repo>> GetPinnedReposAsync()
        {
            _logger.LogInformation("Retrieving 'current' repositories from Firestore.");
            var pinnedReposRef = Db.Collection("pinned-repositories");
            var pinnedCurrentReposSnapshot = await pinnedReposRef.WhereEqualTo("Current", true)
                .GetSnapshotAsync();

            var pinnedCurrentRepos =
                pinnedCurrentReposSnapshot.Documents.Select(docSnapshot => docSnapshot.ConvertTo<Repo>());

            return pinnedCurrentRepos;
        }

        public async Task<Repo> UploadRepoAsync(Repo repo)
        {
            _logger.LogInformation("Beginning upload of {databaseId}, with info of {@repo}", repo.DatabaseId, repo);
            var repoWithUtc = ConvertTimesToUtc(repo);

            // Get collection ref
            var pinnedReposRef = Db.Collection("pinned-repositories");

            // Make document ref in collection
            var pinnedRepoRef = pinnedReposRef.Document(repoWithUtc.DatabaseId);

            // write to Db
            var writeResult = await pinnedRepoRef.SetAsync(repoWithUtc);

            // get result of write
            var snapshot = await pinnedRepoRef.GetSnapshotAsync();

            // convert to model
            var updatedRepo = snapshot.ConvertTo<Repo>();

            return updatedRepo;
        }

        private static Repo ConvertTimesToUtc(Repo repo)
        {
            var utcRepo = repo;
            utcRepo.TimeFetched = repo.TimeFetched.ToUniversalTime();
            utcRepo.CreatedAt = repo.CreatedAt.ToUniversalTime();
            utcRepo.UpdatedAt = repo.UpdatedAt.ToUniversalTime();
            return utcRepo;
        }
    }
}