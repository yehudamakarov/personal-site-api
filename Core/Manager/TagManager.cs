using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Requests.Tags;
using Microsoft.Extensions.Logging;

namespace Core.Manager
{
    public class TagManager : ITagManager
    {
        #region Properties

        private readonly ILogger<TagManager> _logger;
        private readonly IProjectBL _projectBL;
        private readonly IBlogPostBL _blogPostBL;
        private readonly ITagBL _tagBL;

        #endregion

        #region Constructors

        public TagManager(IProjectBL projectBL, IBlogPostBL blogPostBL, ITagBL tagBL, ILogger<TagManager> logger)
        {
            _projectBL = projectBL;
            _blogPostBL = blogPostBL;
            _tagBL = tagBL;
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public async Task<bool> MapTag(IEnumerable<Facade> facadesToMap, string tagId)
        {
            try
            {
                await MapTagToProjects(facadesToMap, tagId);
                await MapTagToBlogPosts(facadesToMap, tagId);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "There was a problem mapping {tagId}", tagId);
                return false;
            }
        }

        #endregion

        #region Private Methods

        private async Task MapTagToBlogPosts(IEnumerable<Facade> facadesToMap, string tagId)
        {
            var (blogPostIdsToTag, blogPostIdsToUntag) = await BlogPostIdsToTag(facadesToMap, tagId);
            await TagBlogPosts(tagId, blogPostIdsToTag);
            await UntagBlogPosts(tagId, blogPostIdsToUntag);
        }

        private async Task MapTagToProjects(IEnumerable<Facade> facadesToMap, string tagId)
        {
            var (projectIdsToTag, projectIdsToUntag) = await ProjectIdsToTagAndToUntag(facadesToMap, tagId);
            await TagProjects(tagId, projectIdsToTag);
            await UntagProjects(tagId, projectIdsToUntag);
        }

        private async Task UntagBlogPosts(string tagId, IEnumerable<string> blogPostIdsToUntag)
        {
            foreach (var blogPostId in blogPostIdsToUntag)
            {
                var blogPostResult = await _blogPostBL.GetBlogPostById(blogPostId);
                var blogPost = blogPostResult.Data;
                var removed = blogPost.TagIds.Remove(tagId);
                if (removed)
                {
                    var updatedBlogPost = await _blogPostBL.UpdateBlogPost(blogPost);
                }
            }
        }

        private async Task TagBlogPosts(string tagId, IEnumerable<string> blogPostIdsToTag)
        {
            foreach (var blogPostId in blogPostIdsToTag)
            {
                var blogPostResult = await _blogPostBL.GetBlogPostById(blogPostId);
                var blogPost = blogPostResult.Data;
                blogPost.TagIds.Add(tagId);
                var updatedBlogPost = await _blogPostBL.UpdateBlogPost(blogPost);
            }
        }

        private async Task<(IEnumerable<string> blogPostIdsToTag, IEnumerable<string> blogPostIdsToUntag)>
            BlogPostIdsToTag(IEnumerable<Facade> facadesToMap, string tagId)
        {
            var blogPostIdsToMap = facadesToMap.Where(facade => facade.Type == FacadeType.BlogPost)
                .Select(facade => facade.Id);
            var blogPostsWithTag = await _blogPostBL.GetBlogPostsByTagId(tagId);
            var blogPostIdsWithTag = blogPostsWithTag.Data.Select(blogPost => blogPost.Id);

            var blogPostIdsToTag = blogPostIdsToMap.Except(blogPostIdsWithTag);
            var blogPostIdsToUntag = blogPostIdsWithTag.Except(blogPostIdsToMap);
            return (blogPostIdsToTag, blogPostIdsToUntag);
        }

        private async Task UntagProjects(string tagId, IEnumerable<string> projectIdsToUntag)
        {
            foreach (var projectId in projectIdsToUntag)
            {
                var projectResult = await _projectBL.GetProjectById(projectId);
                var project = projectResult.Data;
                var removed = project.TagIds.Remove(tagId);
                if (removed)
                {
                    var updatedProject = await _projectBL.UpdateProject(project);
                }
            }
        }

        private async Task TagProjects(string tagId, IEnumerable<string> projectIdsToTag)
        {
            foreach (var projectId in projectIdsToTag)
            {
                var projectResult = await _projectBL.GetProjectById(projectId);
                var project = projectResult.Data;
                project.TagIds.Add(tagId);
                var updatedProject = await _projectBL.UpdateProject(project);
            }
        }

        private async Task<(IEnumerable<string> projectIdsToTag, IEnumerable<string> projectIdsToUntag)>
            ProjectIdsToTagAndToUntag(IEnumerable<Facade> facadesToMap, string tagId)
        {
            var projectIdsToMap = facadesToMap.Where(facade => facade.Type == FacadeType.Project)
                .Select(facade => facade.Id);
            var projectsWithTag = await _projectBL.GetProjectsByTagId(tagId);
            var projectIdsWithTag = projectsWithTag.Data.Select(project => project.GithubRepoDatabaseId);

            var projectIdsToTag = projectIdsToMap.Except(projectIdsWithTag);
            var projectIdsToUntag = projectIdsWithTag.Except(projectIdsToMap);
            return (projectIdsToTag, projectIdsToUntag);
        }

        #endregion
    }
}