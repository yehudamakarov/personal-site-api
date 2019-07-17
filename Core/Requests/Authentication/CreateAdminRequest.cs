using System.ComponentModel.DataAnnotations;

namespace Core.Requests.Authentication
{
    public class CreateAdminRequest
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string CreationCode { get; set; }
        [Required] public string Password { get; set; }
    }
}