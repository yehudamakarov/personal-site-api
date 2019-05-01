using System;
using Core.Interfaces;
using Core.Requests.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace PersonalSiteApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationBL _authenticationBL;

        public AuthenticationController(IConfiguration configuration, IAuthenticationBL authenticationBL)
        {
            _configuration = configuration;
            _authenticationBL = authenticationBL;
        }

        [HttpPost]
        public IActionResult CreateAdmin([FromBody] CreateAdminRequest createAdminRequest)
        {
            var user = _authenticationBL.CreateAdmin(
                createAdminRequest.FirstName,
                createAdminRequest.LastName,
                createAdminRequest.CreationCode,
                createAdminRequest.Password
            );
        }
    }
}