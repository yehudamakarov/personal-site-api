using Core.Types;

namespace Core.Results.Authentication
{
    public class CreateAdminResult
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