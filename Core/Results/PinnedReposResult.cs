using System.Collections.Generic;
using Core.Types;

namespace Core.Results
{
    public class PinnedReposResult : IResult<IList<Repo>>
    {
        public IList<Repo> Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}