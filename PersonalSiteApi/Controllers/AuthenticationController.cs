using System.Threading.Tasks;
using Core.Enums.ResultReasons;
using Core.Interfaces;
using Core.Requests.Authentication;
using Core.Responses.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PersonalSiteApi.Controllers
{
	[Route("api/[controller]/[action]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IAuthenticationBL _authenticationBL;

		public AuthenticationController( IAuthenticationBL authenticationBL )
		{
			_authenticationBL = authenticationBL;
		}

		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public IActionResult TestAuthentication()
		{
			return Ok("Authenticated as Admin");
		}

		[HttpPost]
		public async Task<IActionResult> ActivateAdmin( CreateAdminRequest createAdminRequest )
		{
			var createAdminResult =
				await _authenticationBL.HandleActivateAdminRequest(createAdminRequest);
			var createAdminResponse = new ActivateAdminResponse(createAdminResult);

			return StatusCode(
				createAdminResult.Reason == ActivateAdminResultReason.AdminCreated
					? StatusCodes.Status201Created
					: StatusCodes.Status403Forbidden,
				createAdminResponse
			);
		}

		[HttpPost]
		public async Task<IActionResult> Login( AdminLoginRequest adminLoginRequest )
		{
			var loginResult = await _authenticationBL.HandleAdminLoginRequest(adminLoginRequest);
			var loginResponse = new LoginResponse(loginResult);

			return StatusCode(
				loginResult.Reason == LoginResultReason.SuccessfulLogin
					? StatusCodes.Status200OK
					: StatusCodes.Status403Forbidden,
				loginResponse
			);
		}
	}
}