using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Web.UI.WebControls;
using CodeSmith.Data.Caching;

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
        /// Gets or sets the name of the cache profile to use. This will override the Duration property.
        /// </summary>
        [Category("CacheProfile")]
        [Description("The name of the cache profile to use.")]
        public string CacheProfile
        {
            get
            {
                return ViewState["CacheProfile"] as string;
            }
            set { ViewState["CacheProfile"] = value; }
        }

        /// <summary>
        /// Gets or sets the time, in seconds, that the query is cached.
        /// </summary>
        [Category("Cache")]
        [Description("The time, in seconds, that the query is cached.")]
        public int Duration
        {
            get
            {
                object result = ViewState["Duration"];
                if (result != null)
                    return (int)result;

                return 0;
            }
            set { ViewState["Duration"] = value; }
        }

        private void OnSelecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (!EnableCache)
                return;

            string key = GetKey();
            object source = CacheManager.Get<object>(key);
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

            string key = GetKey();
            object source = CacheManager.Get<object>(key);
            if (source != null)
                return;

            Debug.WriteLine("Cache Insert: " + key);
            if (!String.IsNullOrEmpty(CacheProfile))
                CacheManager.Set(key, source, CacheManager.GetProfile(CacheProfile));
            else if (Duration > 0)
                CacheManager.Set(key, source, CacheSettings.FromDuration(Duration));
            else
                CacheManager.Set(key, source);
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