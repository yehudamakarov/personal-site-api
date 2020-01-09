using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Results;
using Core.Types;

namespace Core.Interfaces
{
    public interface ITagBL
    {
        Task<TagResult> CreateOrFindByTagId(string tagId);
        Task<TagsResult> GetAllTags();
        Task AdjustTagCounts(IReadOnlyCollection<string> currentTagIds, IReadOnlyCollection<string> newTagIds);
        Task CreateOrFindTags(IEnumerable<string> tagIds);
        Task<TagResult> UpdateTag(Tag tag);
        Task<DeleteTagResult> DeleteTagByTagId(string tagId);
    }
}