using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Future
{
    public class LoadedFutureValue<T> : IFutureValue<T>
    {
        public LoadedFutureValue(T value, IQueryable query)
        {
            Value = value;
            Query = query;
        }

        public bool IsLoaded
        {
            get { return true; }
        }

        public IQueryable Query { get; private set; }

        public T Value { get; private set; }

        public void LoadValue(object o)
        {
        }
    }
}
