using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Provides for defering the execution to a batch of queries.
    /// </summary>
    /// <typeparam name="T">The type for the future query.</typeparam>
    public class FutureQuery<T> : FutureQueryBase<T>, IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FutureQuery&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="query">The query source to use when materializing.</param>
        /// <param name="loadAction">The action to execute when the query is accessed.</param>
        public FutureQuery(IQueryable<T> query, Action loadAction)
            : base(query, loadAction)
        { }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            var result = GetResult();

            return result == null 
                ? new List<T>().GetEnumerator() 
                : result.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}