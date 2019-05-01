namespace Core.Requests.Authentication
{
	public class AdminLoginRequest
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Password { get; set; }
	}
}