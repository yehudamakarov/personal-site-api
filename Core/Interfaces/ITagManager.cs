using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Requests.Tags;

namespace Core.Interfaces
{
    public interface ITagManager
    {
        Task<bool> MapTag(IEnumerable<Facade> facadesToMap, string tagId);
    }
}