using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<User> GetAdmin( string firstName, string lastName );
        User CreateAdmin( User admin );
    }
}