using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
	public class AuthenticationRepository : RepositoryBase, IAuthenticationRepository
	{
		public AuthenticationRepository( IConfiguration configuration ) : base(configuration) { }

		public async Task<User> GetAdmin( string firstName, string lastName )
		{
			var adminQuery = Db.Collection("users")
				.WhereEqualTo("firstName", firstName)
				.WhereEqualTo("lastName", lastName);

			var snapshot = await adminQuery.GetSnapshotAsync();

			var admin = snapshot.FirstOrDefault(documentSnapshot => documentSnapshot.Exists)
				?.ConvertTo<User>();

			return admin;
		}
	}
}