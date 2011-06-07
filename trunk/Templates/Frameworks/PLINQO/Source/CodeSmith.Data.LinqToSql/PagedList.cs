using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// A paged collection.
    /// </summary>
    /// <typeparam name="T">The type of the items in the list.</typeparam>
    /// <remarks>
    /// When this collection is created, <see cref="IQueryable"/> Skip and Take is
    /// calculated and called on the source list. Also, if total count 
    /// is not specified, <see cref="IQueryable"/> Count is called.
    /// </remarks>
    public class PagedList<T> : List<T>, IPageList<T>
    {
        private IQueryable<T> _originalSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.PagedList`1"/> class.
        /// </summary>
        /// <param name="source">The source list of items.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize)
            : this(source, pageIndex, pageSize, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.PagedList`1"/> class.
        /// </summary>
        /// <param name="source">The source list of items.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="totalCount">The total count.</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int? totalCount)
        {
            Initialize(source.AsQueryable(), pageIndex, pageSize, totalCount);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.PagedList`1"/> class.
        /// </summary>
        /// <param name="source">The source list of items.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
            : this(source, pageIndex, pageSize, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CodeSmith.Data.Linq.PagedList`1"/> class.
        /// </summary>
        /// <param name="source">The source list of items.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="totalCount">The total count.</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize, int? totalCount)
        {
            Initialize(source, pageIndex, pageSize, totalCount);
        }

        #region IPagedList Members

        /// <summary>
        /// Gets a value indicating whether this instance has previous page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has previous page; otherwise, <c>false</c>.
        /// </value>
        public bool HasPreviousPage { get { return (PageIndex > 0); } }
        /// <summary>
        /// Gets a value indicating whether this instance has next page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has next page; otherwise, <c>false</c>.
        /// </value>
        public bool HasNextPage { get { return (PageIndex < (PageCount - 1)); } }
        /// <summary>
        /// Gets a value indicating whether this instance is first page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first page; otherwise, <c>false</c>.
        /// </value>
        public bool IsFirstPage { get { return (PageIndex <= 0); } }
        /// <summary>
        /// Gets a value indicating whether this instance is last page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is last page; otherwise, <c>false</c>.
        /// </value>
        public bool IsLastPage { get { return (PageIndex >= (PageCount - 1)); } }
        /// <summary>
        /// Gets the total page count.
        /// </summary>
        /// <value>The total page count.</value>
        public int PageCount { get { return TotalItemCount > 0 ? (int)Math.Ceiling(TotalItemCount / (double)PageSize) : 0; } }
        /// <summary>
        /// Gets the zero based index of the page.
        /// </summary>
        /// <value>The index of the page.</value>
        public int PageIndex { get; private set; }
        /// <summary>
        /// Gets the page number. Page number is PageIndex + 1.
        /// </summary>
        /// <value>The page number.</value>
        public int PageNumber { get { return PageIndex + 1; } }
        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; private set; }
        /// <summary>
        /// Gets the total item count.
        /// </summary>
        /// <value>The total item count.</value>
        public int TotalItemCount { get; private set; }

        #endregion

        /// <summary>
        /// Initializes the specified source.
        /// </summary>
        /// <param name="source">The source list of items.</param>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="totalCount">The total item count.</param>
        protected void Initialize(IQueryable<T> source, int pageIndex, int pageSize, int? totalCount)
        {
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException("pageIndex", "PageIndex cannot be below 0.");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", "PageSize cannot be less than 1.");

            if (source == null)
                source = new List<T>().AsQueryable();

            _originalSource = source;

            PageSize = pageSize;
            PageIndex = pageIndex;

            int skip = (pageIndex) * pageSize;

            if (totalCount.HasValue)
            {
                TotalItemCount = totalCount.Value;
                AddPage(source, skip, pageSize);
                return;
            }

            if (!source.SupportsFutureQuery())
            {
                // runs 2 queries
                TotalItemCount = source.Count();
                AddPage(source, skip, pageSize);
                return;
            }

            // use Future to batch the queries
            var q1 = source.FutureCount();
            var q2 = source.Skip(skip).Take(pageSize).Future();

            // runs batch query
            TotalItemCount = q1.Value;
            if (TotalItemCount == 0)
                return;

            AddRange(q2);
        }

        private void AddPage(IQueryable<T> source, int skip, int pageSize)
        {
            if (TotalItemCount == 0)
                return;

            IQueryable<T> page = (source).Skip(skip).Take(pageSize);
            AddRange(page.ToList());
        }

        /// <summary>
        /// Gets the next page of data using the original source.
        /// </summary>
        /// <returns>
        /// A new PageList with the current page index incremented. 
        /// </returns>
        /// <remarks>
        /// The source, page size and total will be the same as the current PageList.
        /// </remarks>
        public PagedList<T> NextPage()
        {
            if (_originalSource == null || !HasNextPage)
                return null;

            return new PagedList<T>(_originalSource, PageIndex + 1, PageSize, TotalItemCount);
        }

        /// <summary>
        /// Gets the previous page of data using the original source.
        /// </summary>
        /// <returns>
        /// A new PageList with the current page index decremented. 
        /// </returns>
        /// <remarks>
        /// The source, page size and total will be the same as the current PageList.
        /// </remarks>
        public PagedList<T> PreviousPage()
        {
            if (_originalSource == null || !HasPreviousPage)
                return null;

            return new PagedList<T>(_originalSource, PageIndex - 1, PageSize, TotalItemCount);
        }

        /// <summary>
        /// Gets the paged list of data from the specified page index.
        /// </summary>
        /// <param name="pageIndex">The zero based index of the page.</param>
        /// <returns>
        /// A new PageList with data from the specified page index.
        /// </returns>
        /// <remarks>
        /// The source, page size and total will be the same as the current PageList.
        /// </remarks>
        public PagedList<T> GotoPage(int pageIndex)
        {
            if (_originalSource == null)
                return null;

            return new PagedList<T>(_originalSource, pageIndex, PageSize, TotalItemCount);
        }

    }
}
