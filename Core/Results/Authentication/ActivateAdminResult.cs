using Core.Types;

namespace Core.Results.Authentication
{
    public class ActivateAdminResult
    {
        public enum ResultReason
        {
            NoAdminRecord,
            BadCreationCode,
            AdminAlreadyExists,
            AdminCreated
        }

        public User Admin { get; set; }
        public ResultReason Reason { get; set; }
    }
}