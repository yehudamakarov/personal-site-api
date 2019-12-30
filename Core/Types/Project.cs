using System;
using System.Collections.Generic;
using System.ComponentModel;
using Google.Cloud.Firestore;

namespace Core.Types
{
    [FirestoreData] public class Project
    {
        public Project()
        {
            ProjectHighlights = new List<ProjectHighlight>();
            ProjectPictures = new List<ProjectPicture>();
            TagIds = new List<string>();
        }

        private string _projectTitle;
        [FirestoreDocumentId] public string GithubRepoDatabaseId { get; set; }

        [Description("Controlled by Github")] [FirestoreProperty]
        public string ProjectName { get; set; }

        [Description("Controlled by Github")] [FirestoreProperty]
        public string ProjectDescription { get; set; }

        [Description("Controlled by Github")] [FirestoreProperty]
        public bool IsPinnedRepo { get; set; }

        [Description("Controlled by Github")] [FirestoreProperty]
        public string GithubUrl { get; set; }


        [Description("Controlled by Github")] [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        [Description("Controlled by Github")] [FirestoreProperty]
        public DateTime UpdatedAt { get; set; }

        [FirestoreProperty] public string ProjectTitle
        {
            get => _projectTitle ?? ProjectName;
            set => _projectTitle = value;
        }

        [Description("Editable")] [FirestoreProperty]
        public string DeploymentUrl { get; set; }

        [Description("Editable")] [FirestoreProperty]
        public string ProjectOverview { get; set; }

        [Description("Editable")] [FirestoreProperty]
        public List<ProjectHighlight> ProjectHighlights { get; set; }

        [Description("Editable")] [FirestoreProperty]
        public List<ProjectPicture> ProjectPictures { get; set; }

        [Description("Editable")] [FirestoreProperty]
        public List<string> TagIds { get; set; }

        [FirestoreProperty] public string Slug => string.Join("-", ProjectTitle.ToLower().Trim().Split(" "));
    }
}