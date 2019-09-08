using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<List<BlogPost>> GetBlogPostsByProject(string projectName);
        Task<List<BlogPost>> GetAllBlogPosts();
        Task<BlogPost> AddBlogPost(string title, string description, string content, string projectName);
    }
}