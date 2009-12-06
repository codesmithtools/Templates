using System;
using System.Web.Caching;

namespace CodeSmith.Data.Caching
{
    public enum CacheExpirationMode
    {
        None,
        Duration,
        Sliding,
        Absolute
    }

    /// <summary>
    /// Settings Object for QueryResultCache.FromCache Methods
    /// </summary>
    public class CacheSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSettings"/> class.
        /// </summary>
        public CacheSettings()
        {
            CacheEmptyResult = true;
            Priority = CacheItemPriority.Normal;
            Mode = CacheExpirationMode.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSettings"/> class with the specified duration.
        /// </summary>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        public CacheSettings(int duration)
            : this()
        {
            Duration = TimeSpan.FromSeconds(duration);
            Mode = CacheExpirationMode.Duration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSettings"/> class with the specified absolute expiration.
        /// </summary>
        /// <param name="absoluteExpiration">The absolute DateTime the cache will expire.</param>
        public CacheSettings(DateTime absoluteExpiration)
        {
            AbsoluteExpiration = absoluteExpiration;
            Mode = CacheExpirationMode.Absolute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSettings"/> class with the specified sliding expiration.
        /// </summary>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        public CacheSettings(TimeSpan slidingExpiration)
        {
            Duration = slidingExpiration;
            Mode = CacheExpirationMode.Sliding;
        }

        #endregion

        ///// <summary>
        ///// The Absolute Expiration represented as a Duration from DateTime.UtcNow.
        ///// </summary>
        //public TimeSpan AbsoluteExpirationDuration { get; set; }

        //private DateTime? _absoluteExpiration;

        ///// <summary>
        ///// Used for setting System.Web.Caching.Cache.Insert parameter absoluteExpiration:
        ///// The time at which the inserted object expires and is removed from the cache. To
        ///// avoid possible issues with local time such as changes from standard time to
        ///// daylight saving time, use System.DateTime.UtcNow rather than System.DateTime.Now
        ///// for this parameter value. If you are using absolute expiration, the
        ///// slidingExpiration parameter must be System.Web.Caching.Cache.NoSlidingExpiration.
        ///// </summary>
        //public DateTime AbsoluteExpiration
        //{
        //    get
        //    {
        //        if (_absoluteExpiration.HasValue)
        //            return _absoluteExpiration.Value;

        //        return AbsoluteExpirationDuration == TimeSpan.Zero
        //            ? Cache.NoAbsoluteExpiration
        //            : DateTime.UtcNow.Add(AbsoluteExpirationDuration);
        //    }
        //    set
        //    {
        //        _absoluteExpiration = value;
        //    }
        //}

        ///// <summary>
        ///// Used for setting System.Web.Caching.Cache.Insert parameter slidingExpiration:
        ///// The interval between the time the inserted object was last accessed and the
        ///// time at which that object expires. If this value is the equivalent of 20
        ///// minutes, the object will expire and be removed from the cache 20 minutes after
        ///// it was last accessed. If you are using sliding expiration, the
        ///// absoluteExpiration parameter must be System.Web.Caching.Cache.NoAbsoluteExpiration.
        ///// </summary>
        //public TimeSpan SlidingExpiration { get; set; }

        public CacheExpirationMode Mode { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime AbsoluteExpiration { get; set; }

        /// <summary>
        /// Used for setting System.Web.Caching.Cache.Insert parameter priority:
        /// The cost of the object relative to other items stored in the cache, as
        /// expressed by the System.Web.Caching.CacheItemPriority enumeration. This value is
        /// used by the cache when it evicts objects; objects with a lower cost are removed
        /// from the cache before objects with a higher cost.
        /// </summary>
        public CacheItemPriority Priority { get; set; }

        /// <summary>
        /// Used to determine if an empty result should be cached.
        /// </summary>
        public bool CacheEmptyResult { get; set; }

        /// <summary>
        /// Used for setting System.Web.Caching.Cache.Insert parameter dependencies:
        /// The file or cache key dependencies for the item. When any dependency changes, the
        /// object becomes invalid and is removed from the cache. If there are no
        /// dependencies, this parameter contains null.
        /// </summary>
        public CacheDependency CacheDependency { get; set; }

        /// <summary>
        /// Used for setting System.Web.Caching.Cache.Insert parameter onRemoveCallback:
        /// A delegate that, if provided, will be called when an object is removed from the
        /// cache. You can use this to notify applications when their objects are deleted from
        /// the cache.
        /// </summary>
        public CacheItemRemovedCallback CacheItemRemovedCallback { get; set; }

        /// <summary>
        /// Gets or sets the cache provider name.
        /// </summary>
        /// <value>The cache provider name.</value>
        public string Provider { get; set; }
    }
}
