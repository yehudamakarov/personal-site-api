using System;
using System.Threading.Tasks;
using Core.Enums.ResultReasons;
using Core.Interfaces;
using Core.Requests.Authentication;
using Core.Responses.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PersonalSiteApi.Controllers
{
	[Route("[controller]/[action]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IAuthenticationBL _authenticationBL;
		private readonly ILogger<AuthenticationController> _logger;

		public AuthenticationController(
			IAuthenticationBL authenticationBL,
			ILogger<AuthenticationController> logger
		)
		{
			_authenticationBL = authenticationBL;
			_logger = logger;
		}

		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public IActionResult TestAuthentication() { return Ok("Authenticated as Admin"); }

		[HttpPost]
		public async Task<IActionResult> ActivateAdmin(CreateAdminRequest createAdminRequest)
		{
			var createAdminResult = await _authenticationBL.ActivateAdmin(createAdminRequest);
			var createAdminResponse = new ActivateAdminResponse(createAdminResult);

			return StatusCode(
				createAdminResult.Reason == ActivateAdminResultReason.AdminCreated
					? StatusCodes.Status201Created
					: StatusCodes.Status403Forbidden,
				createAdminResponse
			);
		}

		[HttpPost]
		public async Task<IActionResult> Login(AdminLoginRequest adminLoginRequest)
		{
			try
			{
				var loginResult = await _authenticationBL.HandleAdminLoginRequest(adminLoginRequest);
				var loginResponse = new LoginResponse(loginResult);
				return Ok(loginResponse);
			}
			catch (Exception exception)
			{
				_logger.LogError("There was an error during login.", exception);
				return StatusCode(500);
			}
		}
	}
}