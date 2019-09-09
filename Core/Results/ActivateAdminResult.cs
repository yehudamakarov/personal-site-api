using Core.Enums.ResultReasons;
using Core.Types;

namespace Core.Results
{
    public class ActivateAdminResult : IResult<User, ResultDetails<ActivateAdminResultReason>>
    {
        public User Data { get; set; }
        public ResultDetails<ActivateAdminResultReason> Details { get; set; }
    }
}