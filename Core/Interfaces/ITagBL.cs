using System.Threading.Tasks;
using Core.Results;

namespace Core.Interfaces
{
    public interface ITagBL
    {
        Task<AddTagResult> AddTag(string tagName);
    }
}