namespace Core.Types
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CreationCode { get; set; }
        public bool IsAdmin { get; set; }
        public string PasswordHash { get; set; }
    }
}