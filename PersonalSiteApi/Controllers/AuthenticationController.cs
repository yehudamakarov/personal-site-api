using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Requests.Authentication;
using Core.Results;
using Core.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PersonalSiteApi.Controllers
{
    [ApiController] [Route("[controller]/[action]")] [ProducesResponseType(typeof(string), 500)]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationBL _authenticationBL;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationBL authenticationBL, ILogger<AuthenticationController> logger)
        {
            _authenticationBL = authenticationBL;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = Roles.Administrator)]
        [ProducesResponseType(200)]
        public IActionResult TestAuthentication()
        {
            return Ok("Authenticated as Admin");
        }

        [HttpPost]
        [ProducesResponseType(typeof(ActivateAdminResult), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> ActivateAdmin(CreateAdminRequest createAdminRequest)
        {
            var createAdminResult = await _authenticationBL.ActivateAdmin(createAdminRequest);

            return StatusCode(
                createAdminResult.Details.ResultStatus == ResultStatus.Success
                    ? StatusCodes.Status201Created
                    : StatusCodes.Status403Forbidden,
                createAdminResult
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(AdminLoginResult), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> Login(AdminLoginRequest adminLoginRequest)
        {
            try
            {
                var loginResult = await _authenticationBL.HandleAdminLoginRequest(adminLoginRequest);
                return Ok(loginResult);
            }
            catch (Exception exception)
            {
                const string message = "There was an error during login.";
                _logger.LogError(exception, message);
                return StatusCode(500, message);
            }
        }
    }
}