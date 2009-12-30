using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Web.Caching;
using System.Web.UI.WebControls;

namespace CodeSmith.Data.Web
{
    /// <summary>
    /// A LinqDataSource that provides caching for Linq queries.
    /// </summary>
    public class LinqCacheDataSource : LinqDataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinqCacheDataSource"/> class.
        /// </summary>
        public LinqCacheDataSource()
        {
            Selecting += OnSelecting;
            Selected += OnSelected;
        }

        /// <summary>
        /// Gets or sets a value indicating whether query caching is enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Cache")]
        [Description("Enable caching the linq query result.")]
        public bool EnableCache
        {
            get
            {
                object result = ViewState["EnableCache"];
                if (result != null)
                    return (bool) result;

                return true;
            }
            set { ViewState["EnableCache"] = value; }
        }

        /// <summary>
        /// Gets or sets the time, in seconds, that the query is cached.
        /// </summary>
        [DefaultValue(30)]
        [Category("Cache")]
        [Description("The time, in seconds, that the query is cached.")]
        public int Duration
        {
            get
            {
                object result = ViewState["Duration"];
                if (result != null)
                    return (int) result;

                return 30;
            }
            set { ViewState["Duration"] = value; }
        }

        private void OnSelecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (!EnableCache)
                return;

            var provider = Caching.CacheManager.GetProvider();
            string key = GetKey();
            object source = provider.Get<object>(key);
            if (source == null)
                return;

            Debug.WriteLine("Cache Hit: " + key);
            e.Result = source;
        }

        private void OnSelected(object sender, LinqDataSourceStatusEventArgs e)
        {
            if (!EnableCache)
                return;

            if (e.Exception != null || e.Result == null)
                return;

            var provider = Caching.CacheManager.GetProvider();
            string key = GetKey();
            object source = provider.Get<object>(key);
            if (source != null)
                return;

            Debug.WriteLine("Cache Insert: " + key);
            provider.Set(key, source);
        }

        private string GetKey()
        {
            var sb = new StringBuilder();
            sb.Append(ContextTypeName);
            sb.Append(" from ");
            sb.Append(TableName);

            if (!string.IsNullOrEmpty(Select))
            {
                sb.Append(" select ");
                sb.Append(Select);
            }
            if (!string.IsNullOrEmpty(Where))
            {
                sb.Append(" where ");
                sb.Append(Where);
            }
            if (!string.IsNullOrEmpty(OrderBy))
            {
                sb.Append(" OrderBy ");
                sb.Append(OrderBy);
            }
            return sb.ToString();
        }
    }
}