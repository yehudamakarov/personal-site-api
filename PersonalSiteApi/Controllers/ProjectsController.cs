using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PersonalSiteApi.Controllers
{
    [ApiController] [Route("[controller]/[action]")] [ProducesResponseType(typeof(string), 500)]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectBL _projectBL;

        public ProjectsController(IProjectBL projectBL)
        {
            _projectBL = projectBL;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<ActionResult> UpdateProject(Project project)
        {
            try
            {
                var result = await _projectBL.UpdateProject(project);
                return Ok(result);
            }
            catch (Exception exception)
            {
                var message = exception.StackTrace;
                return StatusCode(500, message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllProjects()
        {
            var result = await _projectBL.GetAllProjects();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ProjectById(string projectId)
        {
            var result = await _projectBL.GetProjectById(projectId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ProjectByName(string projectName)
        {
            var result = await _projectBL.GetProjectByName(projectName);
            return Ok(result);
        }
    }
}