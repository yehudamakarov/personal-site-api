using Core.Types;

namespace Core.Results.Authentication
{
    public class ActivateAdminResult
    {
        public User Admin { get; set; }
        public ResultReason Reason { get; set; }

        public enum ResultReason
        {
            NoAdminRecord,
            BadCreationCode,
            AdminAlreadyExists,
            AdminCreated,
        }
    }
}