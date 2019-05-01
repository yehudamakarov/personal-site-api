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
		public AuthenticationRepository( IConfiguration configuration ) : base(configuration) { }

		public async Task<User> GetAdmin( string firstName, string lastName )
		{
			var adminSnapshot = await GetAdminSnapshot(firstName, lastName);

			return adminSnapshot.ConvertTo<User>();
		}

		private async Task<DocumentSnapshot> GetAdminSnapshot( string firstName, string lastName )
		{
			var adminQuery = Db.Collection("users")
				.WhereEqualTo("firstName", firstName)
				.WhereEqualTo("lastName", lastName);

			var querySnapshot = await adminQuery.GetSnapshotAsync();

			return querySnapshot.FirstOrDefault(documentSnapshot => documentSnapshot.Exists);
		}

		public async Task<User> UpdateAdminPasswordHash( User admin, string passwordHash )
		{
			var adminSnapshot = await GetAdminSnapshot(admin.FirstName, admin.LastName);
			var adminRef = adminSnapshot.Reference;

			var updates = new Dictionary<FieldPath, object>
			{
				{
					new FieldPath("passwordHash"), passwordHash
				}
			};
			await adminRef.UpdateAsync(updates);
			adminSnapshot = await adminRef.GetSnapshotAsync();
			return adminSnapshot.ConvertTo<User>();
		}
	}
}