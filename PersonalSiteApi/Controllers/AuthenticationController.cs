using System.Threading.Tasks;
using Core.Interfaces;
using Core.Requests.Authentication;
using Core.Responses.Authentication;
using Core.Results.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace PersonalSiteApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationBL _authenticationBL;

        public AuthenticationController(IAuthenticationBL authenticationBL)
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
        public async Task<IActionResult> CreateAdmin(CreateAdminRequest createAdminRequest)
        {
            var createAdminResult = await _authenticationBL.HandleCreateAdmin(createAdminRequest);
            var createAdminResponse = new CreateAdminResponse(createAdminResult);

            return StatusCode(
                createAdminResult.Reason == ActivateAdminResult.ResultReason.AdminCreated
                    ? StatusCodes.Status201Created
                    : StatusCodes.Status403Forbidden,
                createAdminResponse
            );
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginRequest adminLoginRequest)
        {
            var loginResult = await _authenticationBL.HandleAdminLogin(adminLoginRequest);
            var loginResponse = new LoginResponse(loginResult);

            return StatusCode(
                loginResult.Reason == LoginResult.ResultReason.SuccessfulLogin
                    ? StatusCodes.Status200OK
                    : StatusCodes.Status403Forbidden,
                loginResponse
            );
        }
    }
}