using System.Threading.Tasks;
using Core.Enums.Authentication;
using Core.Types;

namespace Core.Interfaces
{
    public interface IAuthenticationBL
    {
        Task<CreateAdminResult> HandleCreateAdmin(
            string firstName, string lastName, string creationCode, string password );
    }
}