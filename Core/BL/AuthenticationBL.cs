using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Core.Enums.Authentication;
using Core.Interfaces;
using Core.Requests.Authentication;
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
			CreateAdminRequest createAdminRequest )
		{
			var admin = await _authenticationRepository.GetAdmin(createAdminRequest.FirstName, createAdminRequest.LastName);

			if (admin == null)
			{
				return CreateAdminResult.NoAdminRecord;
			}

			if (createAdminRequest.CreationCode != admin.CreationCode)
			{
				return CreateAdminResult.BadCreationCode;
			}

			if (admin.PasswordHash != "")
			{
				return CreateAdminResult.AdminAlreadyExists;
			}

			var newAdmin = await CreateAdmin(admin, createAdminRequest.Password);
			return CreateAdminResult.AdminCreated;
		}

		public async Task<LoginResult> HandleAdminLogin( AdminLoginRequest adminLoginRequest )
		{
			var admin = await _authenticationRepository.GetAdmin(
				adminLoginRequest.FirstName,
				adminLoginRequest.LastName
			);

			var correctPassword = ValidatePassword(adminLoginRequest.Password, admin.PasswordHash);
			// todo if password is correct, give JWT, if not, give forbidden
		}

		private bool ValidatePassword( string password, string passwordHash )
		{
			// Extract the bytes
			var hashAndSaltFromDb = Convert.FromBase64String(passwordHash);
			
			// Get the salt we stored
			var saltFromDb = new byte[16];
			Array.Copy(hashAndSaltFromDb, 0, saltFromDb, 0, 16);
			
			// compute a hash from entered password
			var pbkdf2 = new Rfc2898DeriveBytes(password, saltFromDb, 10000);
			var hashThatResultsFromEnteredPassword = pbkdf2.GetBytes(20);

			// compare this hash to the hash in the Db
			for (var i = 0; i < hashAndSaltFromDb.Length; i++)
			{
				if (hashThatResultsFromEnteredPassword[i] != hashAndSaltFromDb[i + 16])
				{
					return false;
				}
			}
			return true;
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