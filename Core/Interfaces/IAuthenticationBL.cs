using System.Threading.Tasks;
using Core.Requests.Authentication;
using Core.Results.Authentication;

namespace Core.Interfaces
{
    public interface IAuthenticationBL
    {
        Task<ActivateAdminResult> HandleCreateAdmin(
            CreateAdminRequest createAdminRequest);

        Task<LoginResult> HandleAdminLogin(AdminLoginRequest adminLoginRequest);
    }
}