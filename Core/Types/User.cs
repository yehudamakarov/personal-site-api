using Google.Cloud.Firestore;

namespace Core.Types
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty] public string Id { get; set; }
        [FirestoreProperty] public string FirstName { get; set; }
        [FirestoreProperty] public string LastName { get; set; }
        [FirestoreProperty] public string CreationCode { get; set; }
        [FirestoreProperty] public bool IsAdmin { get; set; }
        [FirestoreProperty] public string PasswordHash { get; set; }
    }
}