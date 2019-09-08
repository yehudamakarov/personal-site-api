using Core.Enums.ResultReasons;

namespace Core.Results
{
    public class AdminLoginResult : IResult<string, LoginResultReason>
    {
        public string Data { get; set; }
        public LoginResultReason Details { get; set; }
    }
}