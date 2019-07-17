using System;
using Google.Cloud.Firestore;

namespace Core.Types
{
    [FirestoreData] public class Repo
    {
        [FirestoreProperty] public string DatabaseId { get; set; }
        [FirestoreProperty] public DateTime TimeFetched { get; set; }
        [FirestoreProperty] public bool Current { get; set; }
        [FirestoreProperty] public string Name { get; set; }
        [FirestoreProperty] public string Description { get; set; }
        [FirestoreProperty] public string Url { get; set; }
        [FirestoreProperty] public DateTime CreatedAt { get; set; }
        [FirestoreProperty] public DateTime UpdatedAt { get; set; }
    }
}