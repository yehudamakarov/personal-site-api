using System;
using Core.Results.Authentication;

namespace Core.Responses.Authentication
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public string Data { get; set; }

        public LoginResponse(LoginResult loginResult)
        {
            Message = ComputeMessage(loginResult.Reason);
            Data = loginResult.Token;
        }

        private string ComputeMessage(LoginResult.ResultReason loginResultReason)
        {
            switch (loginResultReason)
            {
                case LoginResult.ResultReason.UserNotFound:
                    return "A user with this name was not found.";
                case LoginResult.ResultReason.PasswordIncorrect:
                    return "The password associated with this user was is not the same as you provided.";
                case LoginResult.ResultReason.SuccessfulLogin:
                    return "Successfully logged in.";
                case LoginResult.ResultReason.PasswordNotProvided:
                    return "A password was not provided";
                default:
                    throw new ArgumentOutOfRangeException(nameof(loginResultReason), loginResultReason, null);
            }
        }
    }
}