using System.Threading.Tasks;
using Core.Requests.Authentication;
using Core.Results;

namespace Core.Interfaces
{
    public interface IAuthenticationBL
    {
        Task<ActivateAdminResult> ActivateAdmin(CreateAdminRequest createAdminRequest);

        Task<AdminLoginResult> HandleAdminLoginRequest(AdminLoginRequest adminLoginRequest);
    }
}