using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Manager;
using Core.Requests.Tags;
using Core.Results;

namespace Core.Interfaces
{
    public interface ITagManager
    {
        Task<MapTagJobStatus> MapTagProcess(string uniqueKey, IEnumerable<Facade> facadesToMap, string tagId);
        Task<RenameTagJobStatus> RenameTagByIdProcess(string uniqueKey, string currentTagId, string newTagId);
        Task<DeleteTagJobStatus> DeleteTagProcess(string uniqueKey, string tagId);
    }
}