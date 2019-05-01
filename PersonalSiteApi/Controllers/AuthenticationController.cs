using System;
using System.Threading.Tasks;
using Core.Enums.Authentication;
using Core.Interfaces;
using Core.Requests.Authentication;
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

		[HttpPost]
		public async Task<IActionResult> CreateAdmin( CreateAdminRequest createAdminRequest )
		{
			var createAdminResult = await _authenticationBL.HandleCreateAdmin(createAdminRequest);

			switch (createAdminResult)
			{
				case CreateAdminResult.NoAdminRecord:
					return StatusCode(
						StatusCodes.Status403Forbidden,
						"There is no admin record to work with."
					);
				case CreateAdminResult.BadCreationCode:
					return StatusCode(
						StatusCodes.Status403Forbidden,
						"There was a problem with your creation code."
					);
				case CreateAdminResult.AdminAlreadyExists:
					return StatusCode(
						StatusCodes.Status403Forbidden,
						"There was a problem with your creation code."
					);
				case CreateAdminResult.AdminCreated:
					return Ok("The admin is now active.");
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		[HttpPost]
		public Task<IActionResult> Login( AdminLoginRequest adminLoginRequest )
		{
			var loginResult = _authenticationBL.HandleAdminLogin(adminLoginRequest);
		}
	}
}