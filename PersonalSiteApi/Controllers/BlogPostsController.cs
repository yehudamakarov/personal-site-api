using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PersonalSiteApi.Controllers
{
    [ApiController] [Route("[controller]/[action]")]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostBL _blogPostBL;

        public BlogPostsController(IBlogPostBL blogPostBL)
        {
            _blogPostBL = blogPostBL;
        }

        [HttpGet]
        public async Task<IActionResult> AllBlogPosts()
        {
            var result = await _blogPostBL.GetAllBlogPosts();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> BlogPostsByProjectId(string projectId)
        {
            var result = await _blogPostBL.GetBlogPostsByProjectId(projectId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> AddBlogPost(string title, string description, string content,
            string projectId)
        {
            var result = await _blogPostBL.AddBlogPost(title, description, content, projectId);
            return Ok(result);
        }
    }
}