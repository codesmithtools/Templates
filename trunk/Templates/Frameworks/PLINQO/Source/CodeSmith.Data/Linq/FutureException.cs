using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CodeSmith.Data.Linq
{
    public class FutureException : Exception
    {
        public FutureException()
        {}

        public FutureException(string message)
            : base(message)
        {}

        public FutureException(string message, Exception innerException)
            : base(message, innerException)
        {}

        protected FutureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}
