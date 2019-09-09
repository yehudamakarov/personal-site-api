using System;
using Core.Enums.ResultReasons;
using Core.Interfaces;
using Core.Results;
using Core.Types;

namespace Core.Responses.Authentication
{
    public class ActivateAdminResponse : IResponse<User>
    {
        public ActivateAdminResponse(ActivateAdminResult result)
        {
            Message = ComputeMessage(result.Details.ResultStatus);
            Data = result.Data;
        }

        public string Message { get; }
        public User Data { get; }

        private string ComputeMessage(ActivateAdminResultReason resultReason)
        {
            switch (resultReason)
            {
                case ActivateAdminResultReason.NoAdminRecord:
                    return "There is no admin record to work with.";
                case ActivateAdminResultReason.BadCreationCode:
                    return "The creation code was wrong. Can't access the admin record.";
                case ActivateAdminResultReason.AdminAlreadyExists:
                    return "An Admin is already active.";
                case ActivateAdminResultReason.AdminCreated:
                    return "Admin is now active";
                default:
                    throw new ArgumentOutOfRangeException(nameof(resultReason), resultReason, null);
            }
        }
    }
}