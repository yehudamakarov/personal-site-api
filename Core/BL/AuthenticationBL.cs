using System.Threading.Tasks;
using Core.Enums.Authentication;
using Core.Interfaces;
using Core.Types;

namespace Core.BL
{
	public class AuthenticationBL : IAuthenticationBL
	{
		private readonly IAuthenticationRepository _authenticationRepository;

		public AuthenticationBL( IAuthenticationRepository authenticationRepository )
		{
			_authenticationRepository = authenticationRepository;
		}

		public async Task<CreateAdminResult> HandleCreateAdmin(
			string firstName, string lastName, string creationCode, string password )
		{
			var admin = await _authenticationRepository.GetAdmin(firstName, lastName);

			if (admin == null)
			{
				return CreateAdminResult.NoAdminRecord;
			}

			if (creationCode != admin.CreationCode)
			{
				return CreateAdminResult.BadCreationCode;
			}

			if (admin.PasswordHash != null)
			{
				return CreateAdminResult.AdminAlreadyExists;
			}

			CreateAdmin(admin, password);
			return CreateAdminResult.AdminCreated;
		}

		private void CreateAdmin( User admin, string password )
		{
			admin.PasswordHash = CreatePasswordHash(password);
			var updatedAdmin = _authenticationRepository.CreateAdmin(admin);
		}

		private string CreatePasswordHash( string password )
		{
			throw new System.NotImplementedException();
		}
	}
}