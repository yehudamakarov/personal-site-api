namespace Core.Results.Authentication
{
	public class LoginResult
	{
		public string Token { get; set; }
		public ResultReason Reason { get; set; }
		public enum ResultReason
		{
			UserNotFound,
			PasswordIncorrect,
			SuccessfulLogin,
			PasswordNotProvided
		}
		
	}
}