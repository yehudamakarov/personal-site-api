using System;
using System.Collections.Generic;
using Core.Enums.ResultReasons;
using Core.Interfaces;
using Core.Results.GithubRepo;
using Core.Types;

namespace Core.Responses.GithubRepos
{
	public class PinnedReposResponse : IResponse<IEnumerable<Repo>>
	{
		public string Message { get; set; }
		public IEnumerable<Repo> Data { get; set; }
	}
}