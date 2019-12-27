using Core.Results;

namespace Core.Interfaces
{
    public class AdminLoginResult : IResult<string>
    {
        public string Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}