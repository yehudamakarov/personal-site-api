using System.Threading.Tasks;
using Core.Enums.Authentication;
using Core.Requests.Authentication;
using Core.Results.Authentication;
using Core.Types;

namespace Core.Interfaces
{
	public interface IAuthenticationBL
	{
		Task<CreateAdminResult> HandleCreateAdmin(
			CreateAdminRequest createAdminRequest );

		Task<LoginResult> HandleAdminLogin( AdminLoginRequest adminLoginRequest );
	}
}