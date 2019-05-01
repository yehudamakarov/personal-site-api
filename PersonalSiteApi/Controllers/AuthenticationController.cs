using System;
using System.Threading.Tasks;
using Core.Enums.Authentication;
using Core.Interfaces;
using Core.Requests.Authentication;
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
			var createAdminResult = await _authenticationBL.HandleCreateAdmin(
				createAdminRequest.FirstName,
				createAdminRequest.LastName,
				createAdminRequest.CreationCode,
				createAdminRequest.Password
			);

			switch (createAdminResult)
			{
				case CreateAdminResult.NoAdminRecord:
					return Forbid("There is no admin record to work with.");
				case CreateAdminResult.BadCreationCode:
					return Forbid("There was a problem with your creation code.");
				case CreateAdminResult.AdminAlreadyExists:
					return Forbid("There is already an admin active.");
				case CreateAdminResult.AdminCreated:
					return Ok("The admin is now active.");
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}