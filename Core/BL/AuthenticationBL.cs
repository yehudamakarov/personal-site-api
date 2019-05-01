using Core.Enums.Authentication;
using Core.Interfaces;
using Core.Types;

namespace Core.BL
{
    public class AuthenticationBL : IAuthenticationBL
    {
        private readonly IAuthenticationRepository _authenticationRepository;

        public AuthenticationBL(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }
        public CreateAdminResult HandleCreateAdmin( string firstName, string lastName, string creationCode, string password )
        {
            
        }
    }
}