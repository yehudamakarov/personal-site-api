using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Google.Cloud.Firestore;

namespace Core.Types
{
    [FirestoreData] public class Project
    {
        [FirestoreProperty] public DateTime UpdatedAt { get; set; }
        [FirestoreDocumentId] public string GithubRepoDatabaseId { get; set; }
        [FirestoreProperty] public string ProjectName { get; set; }

        private string _projectTitle;

        [FirestoreProperty] public string ProjectTitle
        {
            get => _projectTitle ?? ProjectName;
            set => _projectTitle = value;
        }

        [FirestoreProperty] public string ProjectDescription { get; set; }
        [FirestoreProperty] public string ProjectOverview { get; set; }
        [FirestoreProperty] public List<ProjectHighlight> ProjectHighlights { get; set; }
        [FirestoreProperty] public List<ProjectPicture> ProjectPictures { get; set; }
        [FirestoreProperty] public List<string> TagIds { get; set; }
        [FirestoreProperty] public string Slug => string.Join("-", ProjectTitle.ToLower().Trim().Split(" "));
        [FirestoreProperty] public bool IsPinnedRepo { get; set; }
        [FirestoreProperty] public string GithubUrl { get; set; }
        [FirestoreProperty] public DateTime CreatedAt { get; set; }
    }
}