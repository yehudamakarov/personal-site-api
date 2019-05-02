using System;
using Core.Results.Authentication;
using Core.Types;

namespace Core.Responses.Authentication
{
    public class CreateAdminResponse
    {
        public string Message { get; }
        public User Data { get; }

        public CreateAdminResponse(CreateAdminResult result)
        {
            Message = ComputeMessage(result.Reason);
            Data = result.Admin;
        }


        private string ComputeMessage(CreateAdminResult.ResultReason resultReason)
        {
            switch (resultReason)
            {
                case CreateAdminResult.ResultReason.NoAdminRecord:
                    return "There is no admin record to work with.";
                case CreateAdminResult.ResultReason.BadCreationCode:
                    return "The creation code was wrong. Can't access the admin record.";
                case CreateAdminResult.ResultReason.AdminAlreadyExists:
                    return "An Admin is already active.";
                case CreateAdminResult.ResultReason.AdminCreated:
                    return "Admin is now active";
                default:
                    throw new ArgumentOutOfRangeException(nameof(resultReason), resultReason, null);
            }
        }
    }
}