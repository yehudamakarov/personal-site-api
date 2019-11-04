using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Enums;
using Core.Results;

namespace Core.Interfaces
{
    public interface ITagBL
    {
        Task<AddTagResult> CreateOrFindByTagId(string tagId);
        Task<TagsResult> GetAllTags();
        Task<UpdateTagCountsResult> UpdateTagCountsByOne(IList<string> tagIds, TagCountUpdates direction);
    }
}