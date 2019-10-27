using System.Threading.Tasks;
using Core.Results;

namespace Core.Interfaces
{
    public interface ITagBL
    {
        Task<AddTagResult> CreateOrFindByTagId(string tagId);
        Task<TagsResult> GetAllTags();
    }
}