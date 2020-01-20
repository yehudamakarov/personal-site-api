using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Core;
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
        private const string UpdatesWillBeBroadcastOverAWebsocket = "Updates will be broadcast over a websocket.";

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
            try
            {
                var mapTagJobStatus = await _tagManager.MapTag(mapTagRequest.FacadesToMap, mapTagRequest.TagId);
                return Ok(mapTagJobStatus);
            }
            catch (Exception exception)
            {
                var mapTagJobStatus = new MapTagJobStatus(mapTagRequest.TagId, JobStage.Error);
                _logger.LogError(exception, "There was a problem mapping {tagId}", mapTagRequest.TagId);
                return StatusCode((int) HttpStatusCode.InternalServerError, mapTagJobStatus);
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> RenameTag(string existingTagId, string newTagId)
        {
            try
            {
                var renameTagStatus = await _tagManager.RenameTagById(existingTagId, newTagId);
                return Ok(renameTagStatus);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "There was a problem renaming {tagId}", existingTagId);
                var renameTagStatus = new TagResult
                {
                    Details = new ResultDetails
                    {
                        ResultStatus = ResultStatus.Failure,
                        Message = $"There was a problem renaming {existingTagId} to {newTagId}."
                    }
                };
                return StatusCode((int) HttpStatusCode.InternalServerError, renameTagStatus);
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> DeleteTag()
        {
            return Ok();
        }
    }
}