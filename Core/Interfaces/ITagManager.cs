using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Requests.Tags;
using Core.Results;

namespace Core.Interfaces
{
    public interface ITagManager
    {
        void MapTag(IEnumerable<Facade> facadesToMap, string tagId);
        void RenameTagById(string currentTagId, string newTagId);
    }
}