using Google.Cloud.Firestore;

namespace Core.Types
{
    [FirestoreData] public class Tag
    {
        [FirestoreDocumentId] public string TagId { get; set; }
        [FirestoreProperty] public int? ArticleCount { get; set; }
    }
}