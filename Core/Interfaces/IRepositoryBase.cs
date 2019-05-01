using Google.Cloud.Firestore;

namespace Core.Interfaces
{
    public interface IRepositoryBase
    {
        FirestoreDb Db { get; }
    }
}