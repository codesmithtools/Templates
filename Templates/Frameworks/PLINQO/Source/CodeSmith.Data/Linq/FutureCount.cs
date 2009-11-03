using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Provides for defering the execution of a count query to a batch of queries.
    /// </summary>
    [DebuggerDisplay("IsLoaded={IsLoaded}, Value={ValueForDebugDisplay}")]
    [DebuggerTypeProxy(typeof(FutureCountDebugView))]
    public class FutureCount : FutureQueryBase<int>
    {
        private int _underlyingValue = 0;
        private bool _hasValue = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureCount"/> class.
        /// </summary>
        /// <param name="query">The query source to use when materializing.</param>
        /// <param name="loadAction">The action to execute when the query is accessed.</param>
        public FutureCount(IQueryable query, Action loadAction)
            : base(query, loadAction)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureCount"/> class.
        /// </summary>
        /// <param name="underlyingValue">The underlying value.</param>
        public FutureCount(int underlyingValue)
            : base(null, null)
        {
            _underlyingValue = underlyingValue;
            _hasValue = true;
        }

        /// <summary>
        /// Gets the data command for this query.
        /// </summary>
        /// <param name="dataContext">The data context to get the command from.</param>
        /// <returns>The requested command object.</returns>
        protected override DbCommand GetCommand(System.Data.Linq.DataContext dataContext)
        {
            var command = base.GetCommand(dataContext);
            if (command == null)
                return command;

            // rewrite the comand text to count(*)
            string commandText = command.CommandText;

            // hard coded search, matches pattern in SqlFormatter
            string fromText = commandText.Substring(commandText.IndexOf("\r\nFROM "));
            string countText = "SELECT COUNT(*) AS [value] " + fromText;

            command.CommandText = countText;

            return command;
        }

        /// <summary>
        /// Gets or sets the value assigned to or loaded by the query.
        /// </summary>
        /// <value>The value.</value>
        /// <returns>
        /// The value of this deferred property.
        /// </returns>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int Value
        {
            get
            {
                if (!_hasValue)
                {
                    _hasValue = true;
                    var result = GetResult();
                    if (result == null)
                        return _underlyingValue;

                    _underlyingValue = result.FirstOrDefault();
                }

                return _underlyingValue;
            }
            set
            {
                _underlyingValue = value;
                _hasValue = true;
            }
        }

        #region Debug Proxy
        internal int ValueForDebugDisplay
        {
            get { return _underlyingValue; }
        }

        internal sealed class FutureCountDebugView
        {
            private readonly FutureCount _futureCount;

            public FutureCountDebugView(FutureCount futureCount)
            {
                _futureCount = futureCount;
            }

            public int Value
            {
                get { return _futureCount.ValueForDebugDisplay; }
            }

            public bool IsLoaded
            {
                get { return _futureCount.IsLoaded; }
            }

            public IQueryable Query
            {
                get { return ((IFutureQuery)_futureCount).Query; }
            }
        }
        #endregion

    }


}
