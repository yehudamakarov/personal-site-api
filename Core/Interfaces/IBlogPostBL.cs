using System.Threading.Tasks;
using Core.Results;

namespace Core.Interfaces
{
    public interface IBlogPostBL
    {
        Task<BlogPostsResult> GetAllBlogPosts();
        Task<BlogPostsResult> GetBlogPostsByProject(string projectName);
        Task<BlogPostResult> AddBlogPost(string title, string description, string content, string projectName);
    }
}