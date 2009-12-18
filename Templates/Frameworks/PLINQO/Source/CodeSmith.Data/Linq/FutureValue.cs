using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Provides for defering the execution of a query to a batch of queries.
    /// </summary>
    /// <typeparam name="T">The type for the future query.</typeparam>
    [DebuggerDisplay("IsLoaded={IsLoaded}, Value={ValueForDebugDisplay}")]
    public class FutureValue<T> : FutureQueryBase<T>
    {
        private T _underlyingValue = default(T);
        private bool _hasValue = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> class.
        /// </summary>
        /// <param name="query">The query source to use when materializing.</param>
        /// <param name="loadAction">The action to execute when the query is accessed.</param>
        public FutureValue(IQueryable query, Action loadAction)
            : base(query, loadAction, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> class.
        /// </summary>
        /// <param name="query">The query source to use when materializing.</param>
        /// <param name="loadAction">The action to execute when the query is accessed.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        public FutureValue(IQueryable query, Action loadAction, CacheSettings cacheSettings)
            : base(query, loadAction, cacheSettings)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.FutureValue`1"/> class.
        /// </summary>
        /// <param name="underlyingValue">The underlying value.</param>
        public FutureValue(T underlyingValue)
            : base(null, null, null)
        {
            _underlyingValue = underlyingValue;
            _hasValue = true;
        }

        /// <summary>
        /// Gets or sets the value assigned to or loaded by the query.
        /// </summary>
        /// <returns>
        /// The value of this deferred property.
        /// </returns>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value
        {
            get
            {
                if (!_hasValue)
                {
                    _hasValue = true;
                    var result = GetResult();

                    if (result != null)
                        _underlyingValue = result.FirstOrDefault();
                }

                if (Exception != null)
                    throw new FutureException("An error occurred executing the future query.", Exception);

                return _underlyingValue;
            }
            set
            {
                _underlyingValue = value;
                _hasValue = true;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="FutureValue{T}"/> to <see cref="T"/>.
        /// </summary>
        /// <param name="futureValue">The future value.</param>
        /// <returns>The result of forcing this lazy value.</returns>
        public static implicit operator T(FutureValue<T> futureValue)
        {
            return futureValue.Value;
        }

        /// <summary>
        /// Gets the value for debug display.
        /// </summary>
        /// <value>The value for debug display.</value>
        internal T ValueForDebugDisplay
        {
            get { return _underlyingValue; }
        }
    }



}
