using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PersonalSiteApi.Controllers
{
    [ApiController] [Route("[controller]/[action]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectBL _projectBL;

        public ProjectsController(IProjectBL projectBL)
        {
            _projectBL = projectBL;
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