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

        public TagManager(IProjectBL projectBL, IBlogPostBL blogPostBL, ITagBL tagBL, ILogger<TagManager> logger,
            IJobStatusNotifier jobStatusNotifier)
        {
            _projectBL = projectBL;
            _blogPostBL = blogPostBL;
            _tagBL = tagBL;
            _logger = logger;
            _jobStatusNotifier = jobStatusNotifier;
        }

        #endregion

        #region Public Methods

        public async void MapTag(IEnumerable<Facade> facadesToMap, string tagId)
        {
            try
            {
                // await _jobStatusNotifier.PushMapTagJobStatusUpdate(new TagResult(), JobStage.InProgress);
                var toMap = facadesToMap.ToList();
                await MapTagToProjects(toMap, tagId);
                await MapTagToBlogPosts(toMap, tagId);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "There was a problem mapping {tagId}", tagId);
            }
        }

        public async void RenameTagById(string currentTagId, string newTagId)
        {
            try
            {
                // below is only needed if the current tag is null (not in DB) for some reason. update Projects / Blog Posts createsOrFinds any NEW tag
                var currentTagResult = await _tagBL.CreateOrFindByTagId(currentTagId);
                var currentTagCount = currentTagResult.Data.TagId;

                var projectsChangedCount = await RenameTagInProjects(currentTagId, newTagId);
                var blogPostsChangedCount = await RenameTagInBlogPosts(currentTagId, newTagId);
                var newTagResult = await _tagBL.CreateOrFindByTagId(newTagId);
                newTagResult.Details.Message =
                    $@"{currentTagId} with {currentTagCount} articles was renamed to {newTagId}. It now has {newTagResult.Data.ArticleCount} articles. Projects count: {projectsChangedCount}. Blog Posts count: {blogPostsChangedCount}.";
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "There was a problem renaming {tagId}", currentTagId);
                var result = new TagResult
                {
                    Details = new ResultDetails
                    {
                        ResultStatus = ResultStatus.Failure,
                        Message = $"There was a problem renaming {currentTagId} to {newTagId}."
                    }
                };
            }
        }

        #endregion

        #region Properties

        private readonly ILogger<TagManager> _logger;
        private readonly IJobStatusNotifier _jobStatusNotifier;
        private readonly IProjectBL _projectBL;
        private readonly IBlogPostBL _blogPostBL;
        private readonly ITagBL _tagBL;

        #endregion

        #region Private Methods

        private async Task<int> RenameTagInBlogPosts(string currentTagId, string newTagId)
        {
            var blogPostsResult = await _blogPostBL.GetBlogPostsByTagId(currentTagId);
            if (blogPostsResult.Details.ResultStatus == ResultStatus.Failure) return 0;
            foreach (var blogPost in blogPostsResult.Data)
            {
                blogPost.TagIds.Remove(currentTagId);
                blogPost.TagIds.Add(newTagId);
                var updatedBlogPost = await _blogPostBL.UpdateBlogPost(blogPost);
            }

            return blogPostsResult.Data.Count;
        }

        private async Task<int> RenameTagInProjects(string currentTagId, string newTagId)
        {
            var projectsResult = await _projectBL.GetProjectsByTagId(currentTagId);
            if (projectsResult.Details.ResultStatus == ResultStatus.Failure) return 0;
            foreach (var project in projectsResult.Data)
            {
                project.TagIds.Remove(currentTagId);
                project.TagIds.Add(newTagId);
                var updatedProject = await _projectBL.UpdateProject(project);
            }

            return projectsResult.Data.Count;
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