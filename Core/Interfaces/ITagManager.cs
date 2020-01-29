using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Requests.Tags;
using Core.Results;

namespace Core.Interfaces
{
    public interface ITagManager
    {
        Task<MapTagJobStatus> MapTag(IEnumerable<Facade> facadesToMap, string tagId);
        Task<RenameTagJobStatus> RenameTagById(string currentTagId, string newTagId);
    }
}