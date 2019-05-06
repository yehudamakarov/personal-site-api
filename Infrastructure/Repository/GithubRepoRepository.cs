using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Repository
{
	public class GithubRepoRepository : RepositoryBase, IGithubRepoRepository
	{
		private readonly ILogger<GithubRepoRepository> _logger;

		public GithubRepoRepository(
			IConfiguration configuration, ILogger<GithubRepoRepository> logger ) : base(
			configuration
		)
		{
			_logger = logger;
		}

		public async Task<IEnumerable<Repo>> GetCurrentPinnedRepos()
		{
			_logger.LogInformation("Retrieving 'current' repositories from Firestore.");
			var pinnedReposRef = Db.Collection("pinned-repositories");
			var pinnedCurrentReposSnapshot = await pinnedReposRef.WhereEqualTo("current", true)
				.GetSnapshotAsync();
			_logger.LogInformation("Retrieved 'current' repositories from Firestore.");
			_logger.LogInformation("Converting 'current' repository snapshot to 'Repo' objects.");
			var pinnedCurrentRepos = pinnedCurrentReposSnapshot
				.Documents
				.Select(docSnapshot => docSnapshot.ConvertTo<Repo>());
			_logger.LogInformation("Converted 'current' repository snapshot to 'Repo' objects.");
			return pinnedCurrentRepos;
		}

		public async Task<Repo> UploadRepoAsync( Repo repo )
		{
			_logger.LogInformation("Begin upload single repo {repo}", repo);
			var repoWithUtc = ConvertTimesToUtc(repo);

			// Get collection ref
			var pinnedReposRef = Db.Collection("pinned-repositories");

			// Make document ref in collection
			var pinnedRepoRef = pinnedReposRef.Document(repoWithUtc.DatabaseId.ToString());

			// write to Db
			// todo convert all dates to UTC
			_logger.LogInformation("Begin writing repo to Db with info: {@repo}", repo);
			var writeResult = await pinnedRepoRef.SetAsync(repoWithUtc);
			_logger.LogInformation(
				"Completed setting repo to Db with info: {@repo} with {writeResult}",
				repo,
				writeResult
			);

			// get result of write
			var snapshot = await pinnedRepoRef.GetSnapshotAsync();

			// convert to model
			var updatedRepo = snapshot.ConvertTo<Repo>();
			
			_logger.LogInformation("Completed uploading single repo {@updatedRepo}", updatedRepo);
			return updatedRepo;
		}

		private static Repo ConvertTimesToUtc( Repo repo )
		{
			var utcRepo = repo;
			utcRepo.TimeFetched = repo.TimeFetched.ToUniversalTime();
			utcRepo.CreatedAt = repo.CreatedAt.ToUniversalTime();
			utcRepo.UpdatedAt = repo.UpdatedAt.ToUniversalTime();
			return utcRepo;
		}

		public async Task<IEnumerable<Repo>> UploadReposAsync( IEnumerable<Repo> repos )
		{
			_logger.LogInformation("Beginning upload async.");
			var uploadTasks = repos.Select(UploadRepoAsync)
				.ToArray();
			var uploadedRepos = await Task.WhenAll(uploadTasks);
			_logger.LogInformation(
				"Completed uploading repos. {uploadedRepos}",
				JsonConvert.SerializeObject(uploadedRepos)
			);
			return uploadedRepos;
		}
	}
}