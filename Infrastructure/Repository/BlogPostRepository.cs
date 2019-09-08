using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class BlogPostRepository : RepositoryBase, IBlogPostRepository
    {
        public BlogPostRepository(IConfiguration configuration) : base(configuration) { }

        public Task<List<BlogPost>> GetAllBlogPosts()
        {
            throw new NotImplementedException();
        }

        public Task<BlogPost> AddBlogPost(string title, string description, string content, string projectName)
        {
            throw new NotImplementedException();
        }

        public Task<List<BlogPost>> GetBlogPostsByProject(string projectName)
        {
            throw new NotImplementedException();
        }
    }
}