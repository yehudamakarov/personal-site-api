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
		public PinnedReposResponse( PinnedReposResult pinnedReposResult )
		{
			Message = ComputeMessage(pinnedReposResult.Reason);
			Data = pinnedReposResult.Data;
		}

		public string Message { get; }
		public IEnumerable<Repo> Data { get; }

		private string ComputeMessage( PinnedReposResultReason reason )
		{
			switch (reason)
			{
				case PinnedReposResultReason.Success:
					return "Fetching pinned repositories succeeded";
				case PinnedReposResultReason.Error:
					return "There was an error fetching pinned repositories";
				default:
					throw new ArgumentOutOfRangeException(nameof(reason), reason, null);
			}
		}
	}
}