using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using CodeSmith.Data.Caching;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Base class for future queries.
    /// </summary>
    /// <typeparam name="T">The type for the future query.</typeparam>
    public class FutureQueryBase<T> : IFutureQuery
    {
        private readonly Action _loadAction;
        private readonly IQueryable _query;
        private IEnumerable<T> _result;
        private bool _isLoaded;
        private CacheSettings _cacheSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureQuery&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="query">The query source to use when materializing.</param>
        /// <param name="loadAction">The action to execute when the query is accessed.</param>
        public FutureQueryBase(IQueryable query, Action loadAction)
            : this(query, loadAction, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureQuery&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="query">The query source to use when materializing.</param>
        /// <param name="loadAction">The action to execute when the query is accessed.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        public FutureQueryBase(IQueryable query, Action loadAction, CacheSettings cacheSettings)
        {
            _query = query;
            _loadAction = loadAction;
            _cacheSettings = cacheSettings;
            _result = null;
        }

        /// <summary>
        /// Checks the cache for the results.
        /// </summary>
        private void CheckCache()
        {
            if (_cacheSettings == null)
                return;

            string key = GetKey();
            ICollection<T> cached = QueryResultCache.GetResultCache<T>(key, _cacheSettings);

            if (cached == null)
                return;

            _isLoaded = true;
            _result = cached;
        }

        /// <summary>
        /// Gets the key used when caching the results.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetKey()
        {
            return _query.GetHashKey();
        }

        /// <summary>
        /// Gets the action to execute when the query is accessed.
        /// </summary>
        /// <value>The load action.</value>
        protected Action LoadAction
        {
            get { return _loadAction; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        public bool IsLoaded
        {
            get
            {
                if (_isLoaded)
                    return _isLoaded;

                CheckCache();
                return _isLoaded;
            }
        }

        /// <summary>
        /// Gets or sets the query execute exception. 
        /// </summary>
        /// <value>The query execute exception.</value>      
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets the query source to use when materializing.
        /// </summary>
        /// <value>The query source to use when materializing.</value>
        IQueryable IFutureQuery.Query
        {
            get { return _query; }
        }

        /// <summary>
        /// Gets the result by invoking the <see cref="LoadAction"/> if not already loaded.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that can be used to iterate through the collection.
        /// </returns>
        protected virtual IEnumerable<T> GetResult()
        {
            if (IsLoaded)
                return _result;

            // no load action, run query directly
            if (LoadAction == null)
            {
                _isLoaded = true;
                _result = _query as IEnumerable<T>;
                return _result;
            }

            // invoke the load action on the datacontext
            // result will be set with a callback to SetResult
            LoadAction.Invoke();
            return _result;
        }

        /// <summary>
        /// Gets the data command for this query.
        /// </summary>
        /// <param name="dataContext">The data context to get the command from.</param>
        /// <returns>The requested command object.</returns>
        DbCommand IFutureQuery.GetCommand(DataContext dataContext)
        {
            return GetCommand(dataContext);
        }

        /// <summary>
        /// Gets the data command for this query.
        /// </summary>
        /// <param name="dataContext">The data context to get the command from.</param>
        /// <returns>The requested command object.</returns>
        protected virtual DbCommand GetCommand(DataContext dataContext)
        {
            return dataContext.GetCommand(_query, true);
        }

        /// <summary>
        /// Sets the underling value after the query has been executed.
        /// </summary>
        /// <param name="result">The <see cref="IMultipleResults"/> to get the result from.</param>
        void IFutureQuery.SetResult(IMultipleResults result)
        {
            SetResult(result);
        }

        /// <summary>
        /// Sets the underling value after the query has been executed.
        /// </summary>
        /// <param name="result">The <see cref="IMultipleResults"/> to get the result from.</param>
        protected virtual void SetResult(IMultipleResults result)
        {
            _isLoaded = true;
            List<T> resultList = null;

            try
            {
                var resultSet = result.GetResult<T>();
                resultList = resultSet != null ? resultSet.ToList() : new List<T>();
                _result = resultList;

            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            if (_cacheSettings == null || resultList == null)
                return;

            // cache the result 
            string key = GetKey();
            QueryResultCache.SetResultCache(key, _cacheSettings, resultList);
        }
    }
}