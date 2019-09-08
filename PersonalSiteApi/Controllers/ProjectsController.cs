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
    }
}