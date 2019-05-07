namespace Core.Results.Authentication
{
    public class LoginResult
    {
        public enum ResultReason
        {
            UserNotFound,
            PasswordIncorrect,
            SuccessfulLogin,
            PasswordNotProvided
        }

        public string Token { get; set; }
        public ResultReason Reason { get; set; }
    }
}