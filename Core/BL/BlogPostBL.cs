using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Results;
using Core.Types;
using Microsoft.Extensions.Logging;

namespace Core.BL
{
    public class BlogPostBL : IBlogPostBL
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ILogger<BlogPostBL> _logger;
        private readonly IProjectBL _projectBL;
        private readonly ITagBL _tagBL;

        public BlogPostBL(
            IBlogPostRepository blogPostRepository,
            IProjectBL projectBL,
            ITagBL tagBL,
            ILogger<BlogPostBL> logger
        )
        {
            _blogPostRepository = blogPostRepository;
            _projectBL = projectBL;
            _tagBL = tagBL;
            _logger = logger;
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

        public async Task<BlogPostResult> AddBlogPost(
            string title,
            string description,
            string content,
            string projectId
        )
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

        public async Task<BlogPostsResult> GetBlogPostsByTagId(string tagId)
        {
            var results = await _blogPostRepository.GetBlogPostsByTagId(tagId);
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

        public async Task<BlogPostResult> GetBlogPostById(string blogPostId)
        {
            var blogPost = await _blogPostRepository.GetBlogPostById(blogPostId);
            if (blogPost == null)
                return new BlogPostResult
                {
                    Data = null,
                    Details = new ResultDetails
                    {
                        Message = $"No blogPost with {blogPostId} was found.",
                        ResultStatus = ResultStatus.Failure
                    }
                };

            return new BlogPostResult
            {
                Data = blogPost,
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }

        public async Task<BlogPostResult> UpdateBlogPost(BlogPost blogPost)
        {
            try
            {
                await UpdateTagIdsOfBlogPost(blogPost);
                var updatedBlogPost = await _blogPostRepository.UpdateBlogPost(blogPost);
                return new BlogPostResult
                {
                    Data = updatedBlogPost,
                    Details = new ResultDetails
                    {
                        Message = "Successfully updated.",
                        ResultStatus = ResultStatus.Success
                    }
                };
            }
            catch (Exception exception)
            {
                const string message = "The BlogPost may not have been saved.";
                _logger.LogError(exception, message);
                return new BlogPostResult
                {
                    Data = blogPost,
                    Details = new ResultDetails
                    {
                        Message = message,
                        ResultStatus = ResultStatus.Failure
                    }
                };
            }
        }

        private async Task UpdateTagIdsOfBlogPost(BlogPost newBlogPost)
        {
            var currentBlogPost = await _blogPostRepository.GetBlogPostById(newBlogPost.Id);
            await _tagBL.CreateOrFindTags(newBlogPost.TagIds);
            await _tagBL.AdjustTagCounts(currentBlogPost.TagIds, newBlogPost.TagIds);
        }
    }
}