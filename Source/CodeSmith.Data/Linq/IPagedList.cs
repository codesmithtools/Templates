using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Interface defining a paged list.
    /// </summary>
    public interface IPagedList
    {
        /// <summary>
        /// Gets a value indicating whether this instance has previous page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has previous page; otherwise, <c>false</c>.
        /// </value>
        bool HasPreviousPage { get; }
        /// <summary>
        /// Gets a value indicating whether this instance has next page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has next page; otherwise, <c>false</c>.
        /// </value>
        bool HasNextPage { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is first page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first page; otherwise, <c>false</c>.
        /// </value>
        bool IsFirstPage { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is last page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is last page; otherwise, <c>false</c>.
        /// </value>
        bool IsLastPage { get; }
        /// <summary>
        /// Gets the total page count.
        /// </summary>
        /// <value>The total page count.</value>
        int PageCount { get; }
        /// <summary>
        /// Gets the zero based index of the page.
        /// </summary>
        /// <value>The index of the page.</value>
        int PageIndex { get; }
        /// <summary>
        /// Gets the page number. Page number is PageIndex + 1.
        /// </summary>
        /// <value>The page number.</value>
        int PageNumber { get; }
        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        int PageSize { get; }
        /// <summary>
        /// Gets the total item count.
        /// </summary>
        /// <value>The total item count.</value>
        int TotalItemCount { get; }
    }

    /// <summary>
    /// Interface defining a paged list.
    /// </summary>
    /// <typeparam name="T">The type of the items in the list.</typeparam>
    public interface IPageList<T> : IList<T>, IPagedList
    {

    }
}
