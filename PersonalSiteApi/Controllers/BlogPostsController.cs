using System.Threading.Tasks;
using Core.Interfaces;
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
        public async Task<IActionResult> BlogPostsByProject(string projectName)
        {
            var result = await _blogPostBL.GetBlogPostsByProjectId(projectName);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddBlogPost(string title, string description, string content,
            string projectId)
        {
            var result = await _blogPostBL.AddBlogPost(title, description, content, projectId);
            return Ok(result);
        }
    }
}