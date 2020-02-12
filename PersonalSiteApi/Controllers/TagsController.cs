using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.Interfaces;
using Core.Manager;
using Core.Requests.Tags;
using Core.Results;
using Core.Types;
using Google.Cloud.Diagnostics.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace PersonalSiteApi.Controllers
{
    [ApiController] [Route("[controller]/[action]")]
    public class TagsController : ControllerBase
    {
        private readonly ILogger<TagsController> _logger;
        private readonly ITagBL _tagBL;
        private readonly ITagManager _tagManager;
        private readonly IManagedTracer _managedTracer;

        public TagsController(ILogger<TagsController> logger, ITagBL tagBL, ITagManager tagManager,
            IManagedTracer managedTracer)
        {
            _logger = logger;
            _tagBL = tagBL;
            _tagManager = tagManager;
            _managedTracer = managedTracer;
        }

        [HttpGet]
        [ProducesResponseType(typeof(TagsResult), 200)]
        public async Task<IActionResult> AllTags()
        {
            // var handler = new TraceHeaderPropagatingHandler();
            var headers = HttpContext.Request.Headers;
            var spanIdBefore = _managedTracer.GetCurrentSpanId();
            var traceIdBefore = _managedTracer.GetCurrentTraceId();
            _logger.LogInformation(
                "headers: {@HeadersBefore}, spanIdBefore: {@SpanIdBefore}, traceIdBefore: {@TraceIdBefore}",
                headers, spanIdBefore, traceIdBefore);
            // var span = _managedTracer.StartSpan(nameof(AllTags))
            using (var span = _managedTracer.StartSpan(nameof(AllTags)))
            {
                var headersAfter = HttpContext.Request.Headers;
                var spanIdAfter = _managedTracer.GetCurrentSpanId();
                var traceIdAfter = _managedTracer.GetCurrentTraceId();

                _logger.LogInformation(
                    "span: {@Span}, headersAfter: {@HeadersAfter}, spanIdAfter: {@SpanIdAfter}, traceIdAfter: {@TraceIdAfter}",
                    span, headersAfter, spanIdAfter, traceIdAfter);
                // var traceHeaderHandler = new TraceHeaderPropagatingHandler(() => _managedTracer);
                var result = await _tagBL.GetAllTags();
                return Ok(result);
            }
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
                var mapTagJobStatus = await _tagManager.MapTagProcess(mapTagRequest.UniqueKey,
                    mapTagRequest.FacadesToMap, mapTagRequest.TagId);
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
        public async Task<IActionResult> RenameTag(RenameTagRequest renameTagRequest)
        {
            try
            {
                var renameTagStatus =
                    await _tagManager.RenameTagByIdProcess(renameTagRequest.UniqueKey, renameTagRequest.ExistingTagId,
                        renameTagRequest.NewTagId);
                return Ok(renameTagStatus);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "There was a problem renaming {tagId}", renameTagRequest.ExistingTagId);
                var renameTagStatus = new MapTagJobStatus()
                {
                    Item = new TagResult()
                    {
                        Details = new ResultDetails
                        {
                            ResultStatus = ResultStatus.Failure,
                            Message =
                                $"There was a problem renaming {renameTagRequest.ExistingTagId} to {renameTagRequest.NewTagId}."
                        }
                    },
                    JobStage = JobStage.Error,
                    UniqueKey = renameTagRequest.UniqueKey
                };
                return StatusCode((int) HttpStatusCode.InternalServerError, renameTagStatus);
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> DeleteTag(DeleteTagRequest deleteTagRequest)
        {
            try
            {
                var deleteTagStatus =
                    await _tagManager.DeleteTagProcess(deleteTagRequest.UniqueKey, deleteTagRequest.TagId);
                return Ok(deleteTagStatus);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "There was a problem deleting {tagId}", deleteTagRequest.TagId);
                var deleteTagStatus = new DeleteTagJobStatus()
                {
                    Item = deleteTagRequest.TagId,
                    JobStage = JobStage.Error
                };
                return StatusCode(500, deleteTagStatus);
            }
        }
    }

    public class DeleteTagRequest : IJobRequest
    {
        public string UniqueKey { get; set; }
        public string TagId { get; set; }
    }
}