using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class AuthenticationRepository : RepositoryBase, IAuthenticationRepository
    {
        private readonly CollectionReference _usersCollection;

        public AuthenticationRepository(IConfiguration configuration) : base(configuration)
        {
            _usersCollection = Db.Collection("users");
        }

        public async Task<User> GetAdmin(string firstName, string lastName)
        {
            var adminSnapshot = await GetAdminSnapshot(firstName, lastName);

            var admin = adminSnapshot?.ConvertTo<User>();
            return admin;
        }

        public async Task<User> UpdateAdminPasswordHash(User admin, string passwordHash)
        {
            var adminSnapshot = await GetAdminSnapshot(admin.FirstName, admin.LastName);
            var adminRef = adminSnapshot.Reference;

            var updates = new Dictionary<FieldPath, object>
            {
                { new FieldPath("PasswordHash"), passwordHash },
                { new FieldPath("IsAdmin"), true }
            };
            var unused = await adminRef.UpdateAsync(updates);
            adminSnapshot = await adminRef.GetSnapshotAsync();
            var user = adminSnapshot.ConvertTo<User>();
            return user;
        }

        private async Task<DocumentSnapshot> GetAdminSnapshot(string firstName, string lastName)
        {
            const string firstNameField = nameof(User.FirstName);
            const string lastNameField = nameof(User.LastName);
            var adminQuery = _usersCollection
                .WhereEqualTo(firstNameField, firstName)
                .WhereEqualTo(lastNameField, lastName);

            var querySnapshot = await adminQuery.GetSnapshotAsync();

            return querySnapshot.FirstOrDefault(documentSnapshot => documentSnapshot.Exists);
        }
    }
}