using System.Collections.Generic;
using Core.Interfaces;
using Core.Types;

namespace Core.Responses.GithubRepos
{
    public class PinnedReposResponse : IResponse<IEnumerable<PinnedRepo>>
    {
        public string Message { get; set; }
        public IEnumerable<PinnedRepo> Data { get; set; }
    }
}