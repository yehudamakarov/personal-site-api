using Google.Cloud.Firestore;

namespace Core.Types
{
    [FirestoreData] public class BlogPost
    {
        [FirestoreDocumentId] public string Id { get; set; }
        [FirestoreProperty] public string Title { get; set; }
        [FirestoreProperty] public string Description { get; set; }
        [FirestoreProperty] public string Content { get; set; }
    }
}