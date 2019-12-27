using Core.Types;

namespace Core.Results
{
    public class ActivateAdminResult : IResult<User>
    {
        public User Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}