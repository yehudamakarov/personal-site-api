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
            return await Task.FromResult(Ok("all blog posts"));
        }

        [HttpGet]
        public async Task<IActionResult> BlogPostsByProject(string projectName)
        {
            return await Task.FromResult(Ok($"blog posts by project name of {projectName}"));
        }

        [HttpPost]
        public async Task<IActionResult> AddBlogPost(string title, string description, string content,
            string projectName)
        {
            return await Task.FromResult(Ok(
                $"adding blog post with title: {title} description: {description} content: {content} and projectName: {projectName}"));
        }
    }
}