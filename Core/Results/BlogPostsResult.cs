using System.Collections.Generic;
using Core.Types;

namespace Core.Results
{
    public class BlogPostsResult : IResult<IList<BlogPost>>
    {
        public BlogPostsResult()
        {
            Data = new List<BlogPost>();
        }

        public IList<BlogPost> Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}