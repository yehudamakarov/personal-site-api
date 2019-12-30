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

        public async Task<IList<PinnedRepo>> GetPinnedReposAsync(bool onlyCurrent)
        {
            _logger.LogInformation("Retrieving 'current' repositories from Firestore.");
            var pinnedReposRef = Db.Collection("pinned-repositories");
            QuerySnapshot pinnedCurrentReposSnapshot;
            if (onlyCurrent)
                pinnedCurrentReposSnapshot = await pinnedReposRef.WhereEqualTo(nameof(PinnedRepo.Current), true)
                    .GetSnapshotAsync();
            else
                pinnedCurrentReposSnapshot = await pinnedReposRef.GetSnapshotAsync();

            var pinnedCurrentRepos =
                pinnedCurrentReposSnapshot.Documents.Select(docSnapshot => docSnapshot.ConvertTo<PinnedRepo>()).ToList();

            return pinnedCurrentRepos;
        }

        public async Task<PinnedRepo> UploadRepoAsync(PinnedRepo pinnedRepo)
        {
            _logger.LogInformation("Beginning upload of {databaseId}, with info of {@repo}", pinnedRepo.DatabaseId,
                pinnedRepo);
            var repoWithUtc = ConvertTimesToUtc(pinnedRepo);

            // Get collection ref
            var pinnedReposRef = Db.Collection("pinned-repositories");

            // Make document ref in collection
            var pinnedRepoRef = pinnedReposRef.Document(repoWithUtc.DatabaseId);

            // write / update in Db
            var unused = await pinnedRepoRef.SetAsync(repoWithUtc);

            // get result of write
            var snapshot = await pinnedRepoRef.GetSnapshotAsync();

            // convert to model
            var updatedRepo = snapshot.ConvertTo<PinnedRepo>();

            return updatedRepo;
        }

        private static PinnedRepo ConvertTimesToUtc(PinnedRepo pinnedRepo)
        {
            var utcRepo = pinnedRepo;
            utcRepo.TimeFetched = pinnedRepo.TimeFetched.ToUniversalTime();
            utcRepo.CreatedAt = pinnedRepo.CreatedAt.ToUniversalTime();
            utcRepo.UpdatedAt = pinnedRepo.UpdatedAt.ToUniversalTime();
            return utcRepo;
        }
    }
}