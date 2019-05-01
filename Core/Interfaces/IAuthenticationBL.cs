using Core.Enums.Authentication;
using Core.Types;

namespace Core.Interfaces
{
    public interface IAuthenticationBL
    {
        CreateAdminResult HandleCreateAdmin(string firstName, string lastName, string creationCode, string password);
    }
}