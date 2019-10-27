using Core.Types;

namespace Core.Results
{
    public class TagResult : IResult<Tag>
    {
        public Tag Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}