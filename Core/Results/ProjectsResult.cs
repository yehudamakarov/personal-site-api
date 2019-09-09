using System.Collections.Generic;
using Core.Types;

namespace Core.Results
{
    public class ProjectsResult : IResult<IList<Project>>
    {
        public IList<Project> Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}