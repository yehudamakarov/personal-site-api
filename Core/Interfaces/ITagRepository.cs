using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> CreateOrFindByTagId(string tag);
        Task<IList<Tag>> GetAllTags();
        Task<Tag> GetTagById(string tagId);
        Task<Tag> IncrementTagCountById(string tagId, int amount);
        Task<Tag> DecrementTagCountById(string tagId, int amount);
        Task<Tag> UpdateTag(Tag tag);
        Task<string> DeleteTag(string tagId);
    }
}