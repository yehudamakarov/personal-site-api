using System.Collections.Generic;
using Core.Enums.ResultReasons;
using Core.Interfaces;
using Core.Types;

namespace Core.Results.GithubRepo
{
    public class PinnedReposResult : IResult<IEnumerable<Repo>, PinnedReposResultReason>
    {
        public IEnumerable<Repo> Data { get; set; }
        public PinnedReposResultReason Reason { get; set; }
    }
}