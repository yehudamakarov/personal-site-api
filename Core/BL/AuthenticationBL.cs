using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Core.Enums.ResultReasons;
using Core.Interfaces;
using Core.Requests.Authentication;
using Core.Results;
using Core.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.BL
{
    public class AuthenticationBL : IAuthenticationBL
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationBL(IAuthenticationRepository authenticationRepository, IConfiguration configuration)
        {
            _authenticationRepository = authenticationRepository;
            _configuration = configuration;
        }

        public async Task<ActivateAdminResult> ActivateAdmin(CreateAdminRequest createAdminRequest)
        {
            var admin = await _authenticationRepository.GetAdmin(
                createAdminRequest.FirstName,
                createAdminRequest.LastName
            );

            if (admin == null)
                return new ActivateAdminResult
                {
                    Details = new ResultDetails<ActivateAdminResultReason>
                    {
                        ResultStatus = ActivateAdminResultReason.NoAdminRecord,
                        Message = ""
                    }
                };

            if (createAdminRequest.CreationCode != admin.CreationCode)
                return new ActivateAdminResult
                {
                    Details = new ResultDetails<ActivateAdminResultReason>
                    {
                        ResultStatus = ActivateAdminResultReason.BadCreationCode,
                        Message = ""
                    }
                };

            if (admin.PasswordHash != null)
                return new ActivateAdminResult
                {
                    Details = new ResultDetails<ActivateAdminResultReason>
                    {
                        ResultStatus = ActivateAdminResultReason.AdminAlreadyExists,
                        Message = ""
                    }
                };

            var activatedAdmin = await ActivateAdmin(admin, createAdminRequest.Password);

            return new ActivateAdminResult
            {
                Details = new ResultDetails<ActivateAdminResultReason>
                {
                    ResultStatus = ActivateAdminResultReason.AdminCreated,
                    Message = ""
                },
                Data = activatedAdmin
            };
        }

        public async Task<AdminLoginResult> HandleAdminLoginRequest(AdminLoginRequest adminLoginRequest)
        {
            var admin = await _authenticationRepository.GetAdmin(
                adminLoginRequest.FirstName,
                adminLoginRequest.LastName
            );

            if (admin == null)
                return new AdminLoginResult { Details = LoginResultReason.UserNotFound };

            if (adminLoginRequest.Password == null)
                return new AdminLoginResult { Details = LoginResultReason.PasswordNotProvided };

            var correctPassword = ValidatePassword(adminLoginRequest.Password, admin.PasswordHash);
            if (!correctPassword)
                return new AdminLoginResult { Details = LoginResultReason.PasswordIncorrect };

            var token = GenerateToken(admin);
            return new AdminLoginResult
            {
                Data = token,
                Details = LoginResultReason.SuccessfulLogin
            };
        }

        private string GenerateToken(User admin)
        {
            var signingKeyBytes = Convert.FromBase64String(_configuration["JWT_SIGNING_KEY"]);
            var expiryDurationMinutes = int.Parse(_configuration["JWT_TOKEN_EXPIRY_DURATION_IN_MINUTES"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(expiryDurationMinutes),
                Subject = new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim("role", "Administrator"),
                        new Claim("firstName", admin.FirstName),
                        new Claim("lastName", admin.LastName)
                    }
                ),
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(signingKeyBytes),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenObject = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(tokenObject);
        }

        private static bool ValidatePassword(string password, string passwordHash)
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
            for (var i = 0; i < hashThatResultsFromEnteredPassword.Length; i++)
                if (hashThatResultsFromEnteredPassword[i] != hashAndSaltFromDb[i + 16])
                    return false;

            return true;
        }

        private async Task<User> ActivateAdmin(User admin, string password)
        {
            var passwordHash = CreatePasswordHash(password);
            return await _authenticationRepository.UpdateAdminPasswordHash(admin, passwordHash);
        }

        private static string CreatePasswordHash(string password)
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