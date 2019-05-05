using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class GithubRepoRepository : RepositoryBase, IGithubRepoRepository
    {
        public GithubRepoRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<Repo>> GetCurrentPinnedRepos()
        {
            var pinnedReposRef = Db.Collection("pinned-repositories");
            var pinnedCurrentReposSnapshot = await pinnedReposRef.WhereEqualTo("current", true)
                .GetSnapshotAsync();
            var pinnedCurrentRepos = pinnedCurrentReposSnapshot
                .Documents
                .Select(docSnapshot => docSnapshot.ConvertTo<Repo>());
            return pinnedCurrentRepos;
        }

        public async Task<Repo> UploadRepoAsync(Repo repo)
        {
            var repoWithUtc = ConvertTimesToUtc(repo);
            // Get collection ref
            var pinnedReposRef = Db.Collection("pinned-repositories");
            // Make document ref in collection
            var pinnedRepoRef = pinnedReposRef.Document(repoWithUtc.DatabaseId.ToString());
            // write to Db
            // todo convert all dates to UTC
            await pinnedRepoRef.SetAsync(repoWithUtc);
            // get result of write
            var snapshot = await pinnedRepoRef.GetSnapshotAsync();
            // convert to model
            return snapshot.ConvertTo<Repo>();
        }

        private Repo ConvertTimesToUtc(Repo repo)
        {
            var utcRepo = repo;
            utcRepo.TimeFetched = repo.TimeFetched.ToUniversalTime();
            utcRepo.CreatedAt = repo.CreatedAt.ToUniversalTime();
            utcRepo.UpdatedAt = repo.UpdatedAt.ToUniversalTime();
            return utcRepo;
        }

        public async Task<IEnumerable<Repo>> UploadReposAsync(IEnumerable<Repo> repos)
        {
            var uploadTasks = repos.Select(UploadRepoAsync)
                .ToArray();
            var uploadedRepos = await Task.WhenAll(uploadTasks);
            return uploadedRepos;
        }
    }
}