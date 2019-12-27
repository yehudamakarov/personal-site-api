using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Enums;
using Core.Requests.Tags;
using Core.Results;

namespace Core.Interfaces
{
    public interface ITagBL
    {
        Task<AddTagResult> CreateOrFindByTagId(string tagId);
        Task<TagsResult> GetAllTags();
        Task UpdateTagCounts(IEnumerable<string> tagIds, TagCountUpdates direction, int amount);
        bool MapTag(IEnumerable<Facade> facadesToMap, string tagId);
    }
}