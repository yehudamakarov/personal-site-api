using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> AddTag(Tag tag);
    }
}