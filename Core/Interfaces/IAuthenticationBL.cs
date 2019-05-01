using Core.Types;

namespace Core.Interfaces
{
    public interface IAuthenticationBL
    {
        User CreateAdmin(string firstName, string lastName, string creationCode, string password);
    }
}