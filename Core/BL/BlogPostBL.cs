using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Results;

namespace Core.BL
{
    public class BlogPostBL : IBlogPostBL
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogPostBL(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
        }


        public Task<BlogPostsResult> GetAllBlogPosts()
        {
            throw new NotImplementedException();
        }

        public Task<BlogPostsResult> GetBlogPostsByProject(string projectName)
        {
            throw new NotImplementedException();
        }

        public Task<BlogPostResult> AddBlogPost(string title, string description, string content, string projectName)
        {
            throw new NotImplementedException();
        }
    }
}