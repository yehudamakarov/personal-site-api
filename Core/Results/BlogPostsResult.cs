using System.Collections.Generic;
using Core.Types;

namespace Core.Results
{
    public class BlogPostsResult : IResult<List<BlogPost>>
    {
        public BlogPostsResult()
        {
            Data = new List<BlogPost>();
        }

        public List<BlogPost> Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}