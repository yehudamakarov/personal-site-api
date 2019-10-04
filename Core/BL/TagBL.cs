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

        public async Task<AddTagResult> AddTag(string tagName)
        {
            var tag = await _tagRepository.AddTag(new Tag() { TagId = tagName });
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
    }
}