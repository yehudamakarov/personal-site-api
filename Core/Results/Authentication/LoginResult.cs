using Core.Enums.ResultReasons;
using Core.Interfaces;

namespace Core.Results.Authentication
{
    public class LoginResult : IResult<string, LoginResultReason>
    {
        public string Data { get; set; }
        public LoginResultReason Reason { get; set; }
    }
}