using System;
using Core.Results.Authentication;
using Core.Types;

namespace Core.Responses.Authentication
{
    public class CreateAdminResponse
    {
        public CreateAdminResponse(ActivateAdminResult result)
        {
            Message = ComputeMessage(result.Reason);
            Data = result.Admin;
        }

        public string Message { get; }
        public User Data { get; }


        private string ComputeMessage(ActivateAdminResult.ResultReason resultReason)
        {
            switch (resultReason)
            {
                case ActivateAdminResult.ResultReason.NoAdminRecord:
                    return "There is no admin record to work with.";
                case ActivateAdminResult.ResultReason.BadCreationCode:
                    return "The creation code was wrong. Can't access the admin record.";
                case ActivateAdminResult.ResultReason.AdminAlreadyExists:
                    return "An Admin is already active.";
                case ActivateAdminResult.ResultReason.AdminCreated:
                    return "Admin is now active";
                default:
                    throw new ArgumentOutOfRangeException(nameof(resultReason), resultReason, null);
            }
        }
    }
}