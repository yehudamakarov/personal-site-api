using System.Collections.Generic;
using System.Linq;
using Google.Cloud.Firestore;

namespace Core.Types
{
    [FirestoreData] public class Project
    {
        [FirestoreDocumentId] public string GithubRepoDatabaseId { get; set; }
        [FirestoreProperty] public string ProjectName { get; set; }

        [FirestoreProperty] public string ProjectTitle
        {
            get
            {
                return string.Join(" ", ProjectName.Split('-', ' ')
                    .Select(word => word.First().ToString().ToUpper() + word.Substring(1)));
            }
            set { }
        }

        [FirestoreProperty] public string ProjectDescription { get; set; }
        [FirestoreProperty] public string ProjectOverview { get; set; }
        [FirestoreProperty] public List<ProjectHighlight> ProjectHighlights { get; set; }
        [FirestoreProperty] public List<ProjectPicture> ProjectPictures { get; set; }
        [FirestoreProperty] public List<string> TagIds { get; set; }
        [FirestoreProperty] public string Slug => string.Join("-", ProjectTitle.ToLower().Split(" "));
        [FirestoreProperty] public bool IsPinnedRepo { get; set; }
        [FirestoreProperty] public string GithubUrl { get; set; }
    }
}