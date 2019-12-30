using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Results;

namespace Core.Interfaces
{
    public interface ITagBL
    {
        Task<AddTagResult> CreateOrFindByTagId(string tagId);
        Task<TagsResult> GetAllTags();
        Task UpdateTagCounts(IReadOnlyCollection<string> currentTagIds, IReadOnlyCollection<string> newTagIds);
        Task CreateOrFindTags(IEnumerable<string> tagIds);
    }
}