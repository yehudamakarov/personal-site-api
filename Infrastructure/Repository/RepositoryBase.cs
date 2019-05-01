using Core.Interfaces;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class RepositoryBase : IRepositoryBase
    {
        public FirestoreDb Db { get; }
        public RepositoryBase(IConfiguration configuration)
        {
            Db = FirestoreDb.Create(configuration["GOOGLE_PROJECT_ID"]);
        }
    }
}