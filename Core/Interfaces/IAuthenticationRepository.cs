using Core.Types;

namespace Core.Interfaces
{
    public interface IAuthenticationRepository
    {
        User GetAdmin( string firstName, string lastName );
    }
}