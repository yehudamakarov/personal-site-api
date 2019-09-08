using System;
using Core.Enums.ResultReasons;
using Core.Interfaces;
using Core.Results;

namespace Core.Responses.Authentication
{
    public class LoginResponse : IResponse<string>
    {
        public LoginResponse(AdminLoginResult loginResult)
        {
            Message = ComputeMessage(loginResult.Details);
            Data = loginResult.Data;
        }

        public string Message { get; set; }
        public string Data { get; set; }

        private string ComputeMessage(LoginResultReason loginResultReason)
        {
            switch (loginResultReason)
            {
                case LoginResultReason.UserNotFound:
                    return "A user with this name was not found.";
                case LoginResultReason.PasswordIncorrect:
                    return
                        "The password associated with this user was is not the same as you provided.";
                case LoginResultReason.SuccessfulLogin:
                    return "Successfully logged in.";
                case LoginResultReason.PasswordNotProvided:
                    return "A password was not provided";
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(loginResultReason),
                        loginResultReason,
                        null
                    );
            }
        }
    }
}