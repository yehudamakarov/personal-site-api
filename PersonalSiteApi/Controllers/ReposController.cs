using System.Threading.Tasks;
using Core.Interfaces;
using Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PersonalSiteApi.Controllers
{
    [ApiController] [Route("[controller]/[action]")]
    public class ReposController : ControllerBase
    {
        private readonly IGithubRepoBL _githubRepoBL;
        private readonly ILogger<ReposController> _logger;

        public ReposController(IGithubRepoBL githubRepoBL, ILogger<ReposController> logger)
        {
            _githubRepoBL = githubRepoBL;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PinnedReposResult), 200)]
        public async Task<IActionResult> PinnedRepos()
        {
            var pinnedRepos = await _githubRepoBL.GetPinnedRepos(true);
            return Ok(pinnedRepos);
        }
    }
}