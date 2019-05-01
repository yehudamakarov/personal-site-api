using System;
using System.Security.Cryptography;
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

			var newAdmin = await CreateAdmin(admin, password);
			return CreateAdminResult.AdminCreated;
		}

		private async Task<User> CreateAdmin( User admin, string password )
		{
			var passwordHash = CreatePasswordHash(password);
			return await _authenticationRepository.UpdateAdminPasswordHash(admin, passwordHash);
		}

		private static string CreatePasswordHash( string password )
		{
			// Create the salt value with a cryptographic PRNG
			var salt = new byte[16];
			new RNGCryptoServiceProvider().GetBytes(salt);

			// Create the Rfc2898DeriveBytes and get the hash value
			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
			var passwordHash = pbkdf2.GetBytes(20);

			// Combine the salt and password bytes for later use
			var hashAndSaltBytes = new byte[36];
			Array.Copy(salt, 0, hashAndSaltBytes, 0, 16);
			Array.Copy(passwordHash, 0, hashAndSaltBytes, 16, 20);

			// Turn the combined salt+hash into a string for storage
			var passwordHashString = Convert.ToBase64String(hashAndSaltBytes);
			return passwordHashString;
		}
	}
}