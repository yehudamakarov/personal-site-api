using System;
using System.Threading.Tasks;
using Core.Enums.ResultReasons;
using Core.Interfaces;
using Core.Responses.GithubRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PersonalSiteApi.Controllers
{
	[Route("[controller]/[action]")]
	public class ReposController : ControllerBase
	{
		private readonly IGithubRepoBL _githubRepoBL;

		public ReposController(IGithubRepoBL githubRepoBL) { _githubRepoBL = githubRepoBL; }

		[HttpGet]
		public async Task<IActionResult> PinnedRepos()
		{
			try
			{
				var pinnedRepos = await _githubRepoBL.GetPinnedRepos();
				return Ok(pinnedRepos);
			}
			catch (Exception exception)
			{
				return StatusCode(500, exception);
			}
		}
	}
}