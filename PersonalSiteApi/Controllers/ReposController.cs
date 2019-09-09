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
        private readonly ILogger<ReposController> _logger;
        private readonly IRepoBL _repoBL;

        public ReposController(IRepoBL repoBL, ILogger<ReposController> logger)
        {
            _repoBL = repoBL;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PinnedReposResult), 200)]
        public async Task<IActionResult> PinnedRepos()
        {
            var pinnedRepos = await _repoBL.GetPinnedRepos(true);
            return Ok(pinnedRepos);
        }
    }
}