using System.Threading.Tasks;
using Core.Results;
using Core.Types;

namespace Core.Interfaces
{
    public interface IBlogPostBL
    {
        Task<BlogPostsResult> GetAllBlogPosts();
        Task<BlogPostsResult> GetBlogPostsByProjectId(string projectId);
        Task<BlogPostResult> AddBlogPost(string title, string description, string content, string projectId);
        Task<BlogPostsResult> GetBlogPostsByTagId(string tagId);
        Task<BlogPostResult> GetBlogPostById(string blogPostId);
        Task<BlogPostResult> UpdateBlogPost(BlogPost blogPost);
    }
}