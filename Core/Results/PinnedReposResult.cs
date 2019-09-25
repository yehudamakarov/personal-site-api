using System.Collections.Generic;
using Core.Types;

namespace Core.Results
{
    public class PinnedReposResult : IResult<IList<PinnedRepo>>
    {
        public IList<PinnedRepo> Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}