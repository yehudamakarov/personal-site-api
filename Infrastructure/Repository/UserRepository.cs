using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public User GetUser(string firstName, string lastName)
        {
            var users = Db.Collection("users");
            var me = users.WhereEqualTo("firstName", firstName).WhereEqualTo("lastName", lastName);
            me.GetSnapshotAsync();
        }
    }
}