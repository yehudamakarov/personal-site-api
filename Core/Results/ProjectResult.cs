using Core.Types;

namespace Core.Results
{
    public class ProjectResult : IResult<Project>
    {
        public Project Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}