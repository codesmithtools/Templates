using NHibernate;

namespace CodeSmith.Data.NHibernate
{
    internal class FutureValue<T> : IFutureValue<T>
    {
        private readonly T _value;

        public FutureValue(T value)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }
    }
}