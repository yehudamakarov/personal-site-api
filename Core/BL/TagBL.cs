using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Enums;
using Core.Interfaces;
using Core.Results;
using Core.Types;
using Microsoft.Extensions.Logging;

namespace Core.BL
{
    public class TagBL : ITagBL
    {
        #region Properties

        private readonly ITagRepository _tagRepository;
        private readonly ILogger<TagBL> _logger;

        #endregion

        #region Constructors

        public TagBL(ITagRepository tagRepository, ILogger<TagBL> logger)
        {
            _tagRepository = tagRepository;
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public async Task<TagResult> CreateOrFindByTagId(string tagId)
        {
            var tag = await _tagRepository.CreateOrFindByTagId(new Tag { TagId = tagId });
            return new TagResult()
            {
                Data = tag,
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }

        public async Task<TagsResult> GetAllTags()
        {
            var tags = await _tagRepository.GetAllTags();
            if (tags.Count != 0)
                return new TagsResult
                {
                    Data = tags,
                    Details = new ResultDetails { ResultStatus = ResultStatus.Success }
                };

            return new TagsResult
            {
                Data = tags,
                Details = new ResultDetails
                {
                    Message = "None were found",
                    ResultStatus = ResultStatus.Warning
                }
            };
        }

        public async Task UpdateTagCounts(
            IReadOnlyCollection<string> currentTagIds,
            IReadOnlyCollection<string> newTagIds
        )
        {
            await AdjustTagCounts(currentTagIds, newTagIds);
        }

        public async Task CreateOrFindTags(IEnumerable<string> tagIds)
        {
            var createOrFindTagTasks = tagIds.Select(CreateOrFindByTagId);
            var initiatedCreateOrFindTagsTasks =
                (from findTagsTask in createOrFindTagTasks select AwaitTask(findTagsTask)).ToArray();
            await Task.WhenAll(initiatedCreateOrFindTagsTasks);
        }

        public async Task<TagResult> UpdateTag(Tag tag)
        {
            try
            {
                var updatedTag = await _tagRepository.UpdateTag(tag);
                return new TagResult
                {
                    Data = updatedTag,
                    Details = new ResultDetails { ResultStatus = ResultStatus.Success }
                };
            }
            catch (Exception exception)
            {
                const string message = "This tag may have not been updated.";
                _logger.LogError(exception, message + " {@tag}", tag);
                return new TagResult
                {
                    Data = tag,
                    Details = new ResultDetails
                    {
                        Message = message,
                        ResultStatus = ResultStatus.Failure
                    }
                };
            }
        }

        public async Task<DeleteTagResult> DeleteTagByTagId(string tagId)
        {
            try
            {
                var deletedTagId = await _tagRepository.DeleteTag(tagId);
                return new DeleteTagResult
                {
                    Data = deletedTagId,
                    Details = new ResultDetails { ResultStatus = ResultStatus.Success }
                };
            }
            catch (Exception exception)
            {
                const string message = "There was a problem deleting this Tag.";
                _logger.LogError(exception, message + " {tagId}", tagId);
                return new DeleteTagResult
                {
                    Data = tagId,
                    Details = new ResultDetails
                    {
                        Message = message,
                        ResultStatus = ResultStatus.Failure
                    }
                };
            }
        }

        #endregion

        #region Private Methods

        private async Task UpdateTagCount(IEnumerable<string> tagIds, TagCountUpdates direction, int amount)
        {
            switch (direction)
            {
                case TagCountUpdates.Increment:
                    await IncrementTagAmounts(tagIds, amount);
                    break;
                case TagCountUpdates.Decrement:
                    await DecrementTagAmounts(tagIds, amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private async Task AdjustTagCounts(
            IReadOnlyCollection<string> currentProjectTagIds,
            IReadOnlyCollection<string> newProjectTagIds
        )
        {
            var toIncrement = newProjectTagIds.Except(currentProjectTagIds).ToList();
            var toDecrement = currentProjectTagIds.Except(newProjectTagIds).ToList();
            await UpdateTagCount(toIncrement, TagCountUpdates.Increment, 1);
            await UpdateTagCount(toDecrement, TagCountUpdates.Decrement, 1);
        }

        private static async Task<T> AwaitTask<T>(Task<T> task)
        {
            return await task;
        }

        private async Task<TagResult> GetTagById(string tagId)
        {
            var tag = await _tagRepository.GetTagById(tagId);
            if (tag == null)
                return new TagResult
                {
                    Data = null,
                    Details = new ResultDetails
                    {
                        Message = $"No tag with id {tagId} was found.",
                        ResultStatus = ResultStatus.Failure
                    }
                };

            return new TagResult
            {
                Data = tag,
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }

        private async Task DecrementTagAmounts(IEnumerable<string> tagIds, int amount)
        {
            foreach (var tagId in tagIds) await _tagRepository.DecrementTagCountById(tagId, amount);
        }

        private async Task IncrementTagAmounts(IEnumerable<string> tagIds, int amount)
        {
            foreach (var tagId in tagIds) await _tagRepository.IncrementTagCountById(tagId, amount);
        }

        #endregion
    }
}