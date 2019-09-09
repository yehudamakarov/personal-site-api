using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IJob
    {
        Task BeginJobAsync();
    }
}