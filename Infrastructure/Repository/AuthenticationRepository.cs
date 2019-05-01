using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
	public class AuthenticationRepository : RepositoryBase, IAuthenticationRepository
	{
		public AuthenticationRepository( IConfiguration configuration ) : base(configuration) { }

		public User GetAdmin( string firstName, string lastName )
		{
			var users = Db.Collection("users");

			var me = users
				.WhereEqualTo("firstName", firstName)
				.WhereEqualTo("lastName", lastName);
			me.GetSnapshotAsync();
			return new User();
		}
	}
}