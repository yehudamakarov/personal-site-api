using System.ComponentModel.DataAnnotations;

namespace Core.Requests.Authentication
{
    public class AdminLoginRequest
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Password { get; set; }
    }
}