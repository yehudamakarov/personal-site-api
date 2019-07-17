using System.Threading.Tasks;
using Core.Requests.Authentication;
using Core.Results.Authentication;

namespace Core.Interfaces
{
    public interface IAuthenticationBL
    {
        Task<ActivateAdminResult> ActivateAdmin(
            CreateAdminRequest createAdminRequest);

        Task<LoginResult> HandleAdminLoginRequest(AdminLoginRequest adminLoginRequest);
    }
}