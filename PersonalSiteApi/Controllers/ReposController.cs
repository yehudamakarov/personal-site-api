using System.Threading.Tasks;
using Core.Enums.ResultReasons;
using Core.Interfaces;
using Core.Responses.GithubRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PersonalSiteApi.Controllers
{
	[Route("api/[controller]/[action]")]
	public class ReposController : ControllerBase
	{
		private readonly IGithubRepoBL _githubRepoBL;

		public ReposController( IGithubRepoBL githubRepoBL )
		{
			_githubRepoBL = githubRepoBL;
		}

		[HttpGet]
		public async Task<IActionResult> PinnedRepos()
		{
			var pinnedReposResult = await _githubRepoBL.HandlePinnedRepos();
			var pinnedReposResponse = new PinnedReposResponse(pinnedReposResult);
			return StatusCode(
				pinnedReposResult.Reason == PinnedReposResultReason.Success
					? StatusCodes.Status200OK
					: StatusCodes.Status500InternalServerError,
				pinnedReposResponse
			);
		}
	}
}