using Core.Types;

namespace Core.Results
{
    public class AddTagResult : IResult<Tag>
    {
        public Tag Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}