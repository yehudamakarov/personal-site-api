namespace Core.Interfaces
{
    public interface IResult<T, TU>
    {
        T Data { get; set; }
        TU Reason { get; set; }
    }
}