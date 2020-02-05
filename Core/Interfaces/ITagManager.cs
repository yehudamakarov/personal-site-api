using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Requests.Tags;
using Core.Results;

namespace Core.Interfaces
{
    public interface ITagManager
    {
        Task<MapTagJobStatus> MapTag(string uniqueKey, IEnumerable<Facade> facadesToMap, string tagId);
        Task<RenameTagJobStatus> RenameTagById(string uniqueKey, string currentTagId, string newTagId);
    }
}