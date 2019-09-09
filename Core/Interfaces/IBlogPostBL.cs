using System.Threading.Tasks;
using Core.Results;

namespace Core.Interfaces
{
    public interface IBlogPostBL
    {
        Task<BlogPostsResult> GetAllBlogPosts();
        Task<BlogPostsResult> GetBlogPostsByProjectId(string projectId);
        Task<BlogPostResult> AddBlogPost(string title, string description, string content, string projectId);
    }
}