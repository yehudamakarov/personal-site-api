namespace Core.Results
{
    public interface IResult<T>
    {
        // pretending a commit
        T Data { get; set; }
        ResultDetails Details { get; set; }
    }
}