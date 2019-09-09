using Core.Types;

namespace Core.Results
{
    public class BlogPostResult : IResult<BlogPost>
    {
        public BlogPost Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}