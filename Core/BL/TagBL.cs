using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Enums;
using Core.Interfaces;
using Core.Requests.Tags;
using Core.Results;
using Core.Types;

namespace Core.BL
{
    public class TagBL : ITagBL
    {
        private readonly ITagRepository _tagRepository;

        public TagBL(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<AddTagResult> CreateOrFindByTagId(string tagId)
        {
            var tag = await _tagRepository.CreateOrFindByTagId(new Tag() { TagId = tagId });
            return new AddTagResult()
            {
                Data = tag,
                Details = new ResultDetails() { ResultStatus = ResultStatus.Success }
            };
        }

        private async Task<TagResult> GetTagById(string tagId)
        {
            var tag = await _tagRepository.GetTagById(tagId);
            if (tag == null)
            {
                return new TagResult
                {
                    Data = null,
                    Details = new ResultDetails
                    {
                        Message = $"No tag with id {tagId} was found.",
                        ResultStatus = ResultStatus.Failure
                    }
                };
            }

            return new TagResult
            {
                Data = tag,
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }

        public async Task<TagsResult> GetAllTags()
        {
            var tags = await _tagRepository.GetAllTags();
            if (tags.Count != 0)
            {
                return new TagsResult()
                {
                    Data = tags,
                    Details = new ResultDetails { ResultStatus = ResultStatus.Success }
                };
            }

            return new TagsResult()
            {
                Data = tags,
                Details = new ResultDetails()
                {
                    Message = "None were found",
                    ResultStatus = ResultStatus.Warning
                }
            };
        }

        public async Task UpdateTagCounts(IEnumerable<string> tagIds, TagCountUpdates direction, int amount)
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

        public bool MapTag(IEnumerable<Facade> facadesToMap, string tagId)
        {
            throw new NotImplementedException();
        }

        private async Task DecrementTagAmounts(IEnumerable<string> tagIds, int amount)
        {
            foreach (var tagId in tagIds)

            {
                await _tagRepository.DecrementTagCountById(tagId, amount);
            }
        }

        private async Task IncrementTagAmounts(IEnumerable<string> tagIds, int amount)
        {
            foreach (var tagId in tagIds)
            {
                await _tagRepository.IncrementTagCountById(tagId, amount);
            }
        }
    }
}