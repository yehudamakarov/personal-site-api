using System.Threading.Tasks;
using Core.Interfaces;
using Core.Results;
using Core.Types;

namespace Core.BL
{
    public class BlogPostBL : IBlogPostBL
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IProjectBL _projectBL;
        private readonly ITagBL _tagBL;

        public BlogPostBL(IBlogPostRepository blogPostRepository, IProjectBL projectBL, ITagBL tagBL)
        {
            _blogPostRepository = blogPostRepository;
            _projectBL = projectBL;
            _tagBL = tagBL;
        }


        public async Task<BlogPostsResult> GetAllBlogPosts()
        {
            var results = await _blogPostRepository.GetAllBlogPosts();
            if (results.Count == 0)
                return new BlogPostsResult
                {
                    Data = results,
                    Details = new ResultDetails
                    {
                        Message = "None were found.",
                        ResultStatus = ResultStatus.Warning
                    }
                };

            return new BlogPostsResult
            {
                Data = results,
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }

        public async Task<BlogPostsResult> GetBlogPostsByProjectId(string projectId)
        {
            var results = await _blogPostRepository.GetBlogPostsByProjectId(projectId);
            if (results.Count == 0)
                return new BlogPostsResult
                {
                    Data = results,
                    Details = new ResultDetails
                    {
                        Message = $"No blog posts for project with id of {projectId} were found.",
                        ResultStatus = ResultStatus.Warning
                    }
                };

            return new BlogPostsResult
            {
                Data = results,
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }

        public async Task<BlogPostResult> AddBlogPost(string title, string description, string content,
            string projectId)
        {
            var project = await _projectBL.GetProjectById(projectId);
            if (project.Details.ResultStatus == ResultStatus.Failure)
                return new BlogPostResult
                {
                    Data = null,
                    Details = new ResultDetails
                    {
                        Message = $"A project with id of {projectId} was not found.",
                        ResultStatus = ResultStatus.Failure
                    }
                };

            var blogPost = new BlogPost
            {
                Title = title.Trim(),
                Description = description.Trim(),
                Content = content.Trim(),
                ProjectId = projectId.Trim()
            };
            var persistedBlogPost = await _blogPostRepository.AddBlogPost(blogPost);
            return new BlogPostResult
            {
                Data = persistedBlogPost,
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }

        public Task<BlogPostsResult> GetBlogPostsByTagId(string tagId)
        {
            throw new System.NotImplementedException();
        }

        public Task<BlogPostResult> GetBlogPostById(string blogPostId)
        {
            throw new System.NotImplementedException();
        }

        public Task<BlogPostResult> UpdateBlogPost(BlogPost blogPost)
        {
            throw new System.NotImplementedException();
        }
    }
}