using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
	public class RepoRepository : RepositoryBase, IRepoRepository
	{
		public RepoRepository( IConfiguration configuration ) : base(configuration) { }
	}
}