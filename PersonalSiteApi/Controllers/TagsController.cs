using System.Threading.Tasks;
using Core.Interfaces;
using Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PersonalSiteApi.Controllers
{
    [ApiController] [Route("[controller]/[action]")]
    public class TagsController : ControllerBase
    {
        private readonly ILogger<TagsController> _logger;
        private readonly ITagBL _tagBL;

        public TagsController(ILogger<TagsController> logger, ITagBL tagBL)
        {
            _logger = logger;
            _tagBL = tagBL;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AddTagResult), 200)]
        public async Task<IActionResult> AddTag(string tagName)
        {
            var addTagResult = await _tagBL.AddTag(tagName);
            return Ok(addTagResult);
        }
    }
}