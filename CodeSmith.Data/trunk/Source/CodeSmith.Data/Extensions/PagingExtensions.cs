using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Paging extension methods.
    /// </summary>
    public static class PagingExtensions
    {
        /// <summary>
        /// Paginates the specified query.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="page">The zero based index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <returns>An <see cref="T:System.Linq.IQueryable`1"/> with Skip and Take set.</returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
        {
            int skip = Math.Max(pageSize * (page - 1), 0);
            return query.Skip(skip).Take(pageSize);
        }

        #region IQueryable<T> extensions

        /// <summary>
        /// Converts the source to a <see cref="T:CodeSmith.Data.Linq.PagedList`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="source">The <see cref="T:System.Linq.IQueryable`1"/> source.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <returns>A new instance of <see cref="T:CodeSmith.Data.Linq.PagedList`1"/>.</returns>
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        /// <summary>
        /// Converts the source to a <see cref="T:CodeSmith.Data.Linq.PagedList`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="source">The <see cref="T:System.Linq.IQueryable`1"/> source.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns>A new instance of <see cref="T:CodeSmith.Data.Linq.PagedList`1"/>.</returns>
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion

        #region IEnumerable<T> extensions

        /// <summary>
        /// Converts the source to a <see cref="T:CodeSmith.Data.Linq.PagedList`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> source.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <returns>A new instance of <see cref="T:CodeSmith.Data.Linq.PagedList`1"/>.</returns>
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        /// <summary>
        /// Converts the source to a <see cref="T:CodeSmith.Data.Linq.PagedList`1"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> source.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns>A new instance of <see cref="T:CodeSmith.Data.Linq.PagedList`1"/>.</returns>
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion

    }
}
