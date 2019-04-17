using Microsoft.AspNetCore.Mvc;

namespace PersonalSiteApi.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest)
        {
            
        }
    }
}