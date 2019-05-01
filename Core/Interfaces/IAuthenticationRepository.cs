using System.Threading.Tasks;
using Core.Types;

namespace Core.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<User> GetAdmin( string firstName, string lastName );
        Task<User> UpdateAdminPasswordHash( User admin, string passwordHash );
    }
}