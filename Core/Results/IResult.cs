namespace Core.Results
{
    public interface IResult<T>
    {
        T Data { get; set; }
        ResultDetails Details { get; set; }
    }
}