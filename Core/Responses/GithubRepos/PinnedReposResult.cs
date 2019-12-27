using System.Collections.Generic;
using Core.Results;
using Core.Types;

namespace Core.Responses.GithubRepos
{
    public class PinnedReposResult : IResult<IList<PinnedRepo>>
    {
        public IList<PinnedRepo> Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}