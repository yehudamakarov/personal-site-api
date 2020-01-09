using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Requests.Tags;
using Core.Results;
using Microsoft.Extensions.Logging;

namespace Core.Manager
{
    public class TagManager : ITagManager
    {
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
                var toMap = facadesToMap.ToList();
                await MapTagToProjects(toMap, tagId);
                await MapTagToBlogPosts(toMap, tagId);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "There was a problem mapping {tagId}", tagId);
                return false;
            }
        }
        
        public async Task<TagResult> RenameTagById(string currentTagId,string newTagId )
        {
            try
            {
                var tag = await _tagBL.CreateOrFindByTagId(currentTagId);
                await RenameTagInProjects(currentTagId, newTagId);
                // todo continue
                var blogPosts = await _blogPostBL.GetBlogPostsByTagId(currentTagId);
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        #endregion

        #region Properties

        private readonly ILogger<TagManager> _logger;

        private readonly IProjectBL _projectBL;

        private readonly IBlogPostBL _blogPostBL;

        private readonly ITagBL _tagBL;

        #endregion

        #region Private Methods

        private async Task RenameTagInProjects(string currentTagId, string newTagId)
        {
            var projectsResult = await _projectBL.GetProjectsByTagId(currentTagId);
            if (projectsResult.Details.ResultStatus == ResultStatus.Failure)
            {
                foreach (var project in projectsResult.Data)
                {
                    project.TagIds.Remove(currentTagId);
                    project.TagIds.Add(newTagId);
                    var updatedProject = await _projectBL.UpdateProject(project);
                }
            }
        }

        private async Task MapTagToBlogPosts(IEnumerable<Facade> facadesToMap, string tagId)
        {
            var (blogPostIdsToTag, blogPostIdsToUntag) = await BlogPostIdsToTag(facadesToMap, tagId);
            await TagBlogPosts(tagId, blogPostIdsToTag);
            await UntagBlogPosts(tagId, blogPostIdsToUntag);
        }

        private async Task MapTagToProjects(IEnumerable<Facade> facadesToMap, string tagId)
        {
            var ( projectIdsToTag, projectIdsToUntag) = await ProjectIdsToTagAndToUntag(facadesToMap, tagId);
            await TagProjects(tagId, projectIdsToTag);
            await UntagProjects(tagId, projectIdsToUntag);
        }

        private async Task UntagBlogPosts(string tagId, IEnumerable<string> blogPostIdsToUntag)
        {
            foreach (var blogPostId in blogPostIdsToUntag)
            {
                var blogPostResult = await _blogPostBL.GetBlogPostById(blogPostId);
                var blogPost = blogPostResult.Data;
                blogPost.TagIds?.Remove(tagId);
                var updatedBlogPost = await _blogPostBL.UpdateBlogPost(blogPost);
            }
        }

        private async Task TagBlogPosts(string tagId, IEnumerable<string> blogPostIdsToTag)
        {
            foreach (var blogPostId in blogPostIdsToTag)
            {
                var blogPostResult = await _blogPostBL.GetBlogPostById(blogPostId);
                var blogPost = blogPostResult.Data;
                if (blogPost.TagIds == null)
                    blogPost.TagIds = new List<string> { tagId };
                else
                    blogPost.TagIds.Add(tagId);

                var updatedBlogPost = await _blogPostBL.UpdateBlogPost(blogPost);
            }
        }

        private async Task<(IEnumerable<string> blogPostIdsToTag, IEnumerable<string> blogPostIdsToUntag)>
            BlogPostIdsToTag(IEnumerable<Facade> facadesToMap, string tagId)
        {
            var blogPostIdsToMap = facadesToMap.Where(facade => facade.Type == FacadeType.BlogPost)
                .Select(facade => facade.Id).ToList();
            var blogPostsWithTag = await _blogPostBL.GetBlogPostsByTagId(tagId);
            var blogPostIdsWithTag = blogPostsWithTag.Data.Select(blogPost => blogPost.Id).ToList();

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
                project.TagIds?.Remove(tagId);
                var updatedProject = await _projectBL.UpdateProject(project);
            }
        }

        private async Task TagProjects(string tagId, IEnumerable<string> projectIdsToTag)
        {
            foreach (var projectId in projectIdsToTag)
            {
                var projectResult = await _projectBL.GetProjectById(projectId);
                var project = projectResult.Data;
                if (project.TagIds == null)
                    project.TagIds = new List<string> { tagId };
                else
                    project.TagIds.Add(tagId);

                var updatedProject = await _projectBL.UpdateProject(project);
            }
        }

        private async Task<(IEnumerable<string> projectIdsToTag, IEnumerable<string> projectIdsToUntag)>
            ProjectIdsToTagAndToUntag(IEnumerable<Facade> facadesToMap, string tagId)
        {
            var projectIdsToMap = facadesToMap.Where(facade => facade.Type == FacadeType.Project)
                .Select(facade => facade.Id).ToList();

            var projectsWithTag = await _projectBL.GetProjectsByTagId(tagId);
            var projectIdsWithTag = projectsWithTag.Data.Select(project => project.GithubRepoDatabaseId).ToList();

            var projectIdsToTag = projectIdsToMap.Except(projectIdsWithTag);
            var projectIdsToUntag = projectIdsWithTag.Except(projectIdsToMap);
            return (projectIdsToTag, projectIdsToUntag);
        }

        #endregion
    }
}