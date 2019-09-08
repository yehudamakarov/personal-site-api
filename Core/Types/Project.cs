using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace Core.Types
{
    [FirestoreData] public class Project
    {
        [FirestoreDocumentId] public string GithubRepoDatabaseId { get; set; }
        [FirestoreProperty] public string Name { get; set; }
        [FirestoreProperty] public string Description { get; set; }
        [FirestoreProperty] public List<ProjectHighlight> ProjectHighlights { get; set; }
        [FirestoreProperty] public List<ProjectPicture> ProjectPictures { get; set; }
    }
}