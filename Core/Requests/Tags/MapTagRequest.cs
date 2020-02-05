using System.Collections.Generic;

namespace Core.Requests.Tags
{
    public class MapTagRequest
    {
        public string UniqueKey { get; set; }
        public IEnumerable<Facade> FacadesToMap { get; set; }
        public string TagId { get; set; }
    }

    public class RenameTagRequest
    {
        public string UniqueKey { get; set; }
        public string ExistingTagId { get; set; }
        public string NewTagId { get; set; }
    }

    public class Facade
    {
        public Facade()
        {
            TagIds = new List<string>();
        }

        public string Id { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public List<string> TagIds { get; set; }
        public FacadeType Type { get; set; }
    }

    public enum FacadeType
    {
        Project,
        BlogPost
    }
}