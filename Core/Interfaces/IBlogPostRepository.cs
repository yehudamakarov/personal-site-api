using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<IList<BlogPost>> GetBlogPostsByProjectId(string projectId);
        Task<IList<BlogPost>> GetAllBlogPosts();
        Task<BlogPost> AddBlogPost(BlogPost blogPost);
    }
}