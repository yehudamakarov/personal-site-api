using System.Collections.Generic;
using Core.Types;

namespace Core.Results
{
    public class ProjectsResult : IResult<List<Project>>
    {
        public List<Project> Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}