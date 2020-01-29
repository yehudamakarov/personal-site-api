using System.Collections.Generic;

namespace Core.Requests.Tags
{
    public class MapTagRequest
    {
        public IEnumerable<Facade> FacadesToMap { get; set; }
        public string TagId { get; set; }
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