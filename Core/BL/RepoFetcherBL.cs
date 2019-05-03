using Core.Interfaces;

namespace Core.BL
{
	public class RepoFetcherBL : IRepoFetcherBL
	{
		private readonly IRepoRepository _repoRepository;

		public RepoFetcherBL(IRepoRepository repoRepository)
		{
			_repoRepository = repoRepository;
		}

		public void BeginJob()
		{
			throw new System.NotImplementedException();
		}
	}
}