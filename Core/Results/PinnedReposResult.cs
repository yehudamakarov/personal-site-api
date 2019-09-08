using System.Collections.Generic;
using Core.Types;

namespace Core.Results
{
    public class PinnedReposResult : IResult<List<Repo>>
    {
        public List<Repo> Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}