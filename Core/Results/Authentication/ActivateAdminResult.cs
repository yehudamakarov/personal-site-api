using Core.Enums.ResultReasons;
using Core.Interfaces;
using Core.Types;

namespace Core.Results.Authentication
{
    public class ActivateAdminResult : IResult<User, ActivateAdminResultReason>
    {
        public User Data { get; set; }
        public ActivateAdminResultReason Reason { get; set; }
    }
}