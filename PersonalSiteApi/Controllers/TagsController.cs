using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Results;
using Core.Types;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        [ProducesResponseType(typeof(TagsResult), 200)]
        public async Task<IActionResult> AllTags()
        {
            var result = await _tagBL.GetAllTags();
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(AddTagResult), 200)]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> CreateOrFindByTagId([Required] string tagName)
        {
            var addTagResult = await _tagBL.CreateOrFindByTagId(tagName);
            return Ok(addTagResult);
        }
    }
}