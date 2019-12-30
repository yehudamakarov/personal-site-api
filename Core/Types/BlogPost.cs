using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace Core.Types
{
    [FirestoreData] public class BlogPost
    {
        public BlogPost()
        {
            TagIds = new List<string>();
        }
        [FirestoreDocumentId] public string Id { get; set; }
        [FirestoreProperty] public string Title { get; set; }
        [FirestoreProperty] public string Description { get; set; }
        [FirestoreProperty] public string Content { get; set; }
        [FirestoreProperty] public string ProjectId { get; set; }

        [FirestoreProperty] public string Slug => string.Join("-", Title.ToLower().Trim().Split(" "));


        [FirestoreProperty] public List<string> TagIds { get; set; }
    }
}