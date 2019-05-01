namespace Core.Requests.Authentication
{
    public class CreateAdminRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CreationCode { get; set; }
        public string Password { get; set; }
    }
}