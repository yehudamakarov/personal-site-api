using System.Collections.Generic;
using Core.Types;

namespace Core.Results
{
    public class TagsResult : IResult<IList<Tag>>
    {
        // ammend
        public IList<Tag> Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}