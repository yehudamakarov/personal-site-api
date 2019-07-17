namespace Core.Interfaces
{
    public interface IResponse<out T>
    {
        string Message { get; }
        T Data { get; }
    }
}