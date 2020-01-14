using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Requests.Tags;
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
        private readonly ITagManager _tagManager;

        public TagsController(ILogger<TagsController> logger, ITagBL tagBL, ITagManager tagManager)
        {
            _logger = logger;
            _tagBL = tagBL;
            _tagManager = tagManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(TagsResult), 200)]
        public async Task<IActionResult> AllTags()
        {
            var result = await _tagBL.GetAllTags();
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TagResult), 200)]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> CreateOrFindByTagId([Required] string tagId)
        {
            var addTagResult = await _tagBL.CreateOrFindByTagId(tagId);
            return Ok(addTagResult);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> MapTag(MapTagRequest mapTagRequest)
        {
            var result = await _tagManager.MapTag(mapTagRequest.FacadesToMap, mapTagRequest.TagId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public IActionResult RenameTag(string existingTagId, string newTagId)
        {
            _tagManager.RenameTagById(existingTagId, newTagId);
            return new StatusCodeResult((int) HttpStatusCode.Accepted);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> DeleteTag()
        {
            return Ok();
        }
    }
}