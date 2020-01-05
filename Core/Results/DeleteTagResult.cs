namespace Core.Results
{
    public class DeleteTagResult : IResult<string>
    {
        /// <summary>
        /// The tagId of the deleted Tag.
        /// </summary>
        public string Data { get; set; }
        public ResultDetails Details { get; set; }
    }
}