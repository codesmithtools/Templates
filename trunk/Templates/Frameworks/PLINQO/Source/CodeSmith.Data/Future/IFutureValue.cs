namespace CodeSmith.Data.Future
{
    public interface IFutureValue<out T> : IFutureQuery
    {
        T Value { get; }
    }
}