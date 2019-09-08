namespace Core.Results
{
    public interface IResult<T, T1>
    {
        T Data { get; set; }
        T1 Details { get; set; }
    }

    public class ResultDetails<T>
    {
        public T ResultStatus { get; set; }
        public string Message { get; set; }
    }
}