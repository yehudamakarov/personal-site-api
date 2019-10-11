using System.Threading.Tasks;
using Core.Interfaces;
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
                Details = new ResultDetails()
                {
                    Message = "Tag added successfully.",
                    ResultStatus = ResultStatus.Success
                }
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
            if (tags.Count == 0)
            {
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

            return new TagsResult()
            {
                Data = tags,
                Details = new ResultDetails { ResultStatus = ResultStatus.Success }
            };
        }
    }
}