using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> CreateOrFindByTagId(Tag tag);
        Task<IList<Tag>> GetAllTags();
        Task<Tag> GetTagById(string tagId);
    }
}