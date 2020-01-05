using System.Threading.Tasks;
using Core.Interfaces;
using Core.Results;
using Core.Types;
using Microsoft.Extensions.Logging;

namespace Core.Job
{
    public class CalculateTagCountsJob : ICalculateTagCountsJob
    {
        private const string JobName = nameof(CalculateTagCountsJob);
        private readonly IBlogPostBL _blogPostBL;
        private readonly ILogger<CalculateTagCountsJob> _logger;
        private readonly IProjectBL _projectBL;
        private readonly ITagBL _tagBL;

        public CalculateTagCountsJob(ITagBL tagBL, IProjectBL projectBL, IBlogPostBL blogPostBL,
            ILogger<CalculateTagCountsJob> logger)
        {
            _tagBL = tagBL;
            _projectBL = projectBL;
            _blogPostBL = blogPostBL;
            _logger = logger;
        }

        public async Task BeginJobAsync()
        {
            _logger.LogInformation("Beginning {JobName}.", JobName);
            var tags = await _tagBL.GetAllTags();
            if (tags.Details.ResultStatus != ResultStatus.Success)
                _logger.LogWarning("There was a problem retrieving tags in order to count them.");
            else
                foreach (var tag in tags.Data)
                    await CalculateArticleCount(tag);

            _logger.LogInformation("Finished {JobName}.", JobName);
        }

        private async Task CalculateArticleCount(Tag tag)
        {
            var matchingProjects = await _projectBL.GetProjectsByTagId(tag.TagId);
            if (matchingProjects.Details.ResultStatus == ResultStatus.Failure)
            {
                _logger.LogWarning("There was a problem retrieving projects for {@tag}", tag);
                return;
            }

            var matchingBlogPosts = await _blogPostBL.GetBlogPostsByTagId(tag.TagId);
            if (matchingBlogPosts.Details.ResultStatus == ResultStatus.Failure)
            {
                _logger.LogWarning("There was a problem retrieving blogPosts for {@tag}", tag);
                return;
            }

            var totalCount = matchingProjects.Data.Count + matchingBlogPosts.Data.Count;
            if (totalCount == tag.ArticleCount)
            {
                _logger.LogInformation(
                    "{tagId} count hasn't changed. No update necessary. Current count is {articleCount}",
                    tag.TagId,
                    totalCount
                );
                return;
            }

            if (totalCount > 0)
                await UpdateTag(tag, totalCount);
            else
                await DeleteTag(tag);
        }

        private async Task DeleteTag(Tag tag)
        {
            var result = await _tagBL.DeleteTagByTagId(tag.TagId);
            if (result.Details.ResultStatus == ResultStatus.Success)
            {
                var deletedTagId = result.Data;
                _logger.LogInformation("Deleted tag {tagId}", deletedTagId);
            }
            else
            {
                _logger.LogWarning("Tried to delete tag {@tag} but there was a problem.", tag);
            }
        }

        private async Task UpdateTag(Tag tag, int totalCount)
        {
            tag.ArticleCount = totalCount;
            var updatedTag = await _tagBL.UpdateTag(tag);
            if (updatedTag.Details.ResultStatus == ResultStatus.Success)
                _logger.LogInformation(
                    "Updated {tagId} articleCount from {initialTagArticleCount} to {updatedTagArticleCount}",
                    tag.TagId,
                    tag.ArticleCount,
                    updatedTag.Data.ArticleCount);
            else
                _logger.LogWarning("There was a problem updating {@tag} with a count of {articleCount}",
                    tag,
                    totalCount);
        }
    }
}