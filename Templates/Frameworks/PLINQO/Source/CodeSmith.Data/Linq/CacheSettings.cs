using System;
using System.Web.Caching;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Settings Object for QueryResultCache.FromCache Methods
    /// </summary>
    public class CacheSettings
    {

        #region Constructors

        /// <summary>
        /// Creates a CacheSettings object with default/empty values.
        /// </summary>
        public CacheSettings()
        {
            CacheEmptyResult = true;
            AbsoluteExpiration = Cache.NoAbsoluteExpiration;
            Priority = CacheItemPriority.Normal;
            SlidingExpiration = Cache.NoSlidingExpiration;
        }

        /// <summary>
        /// Creates a CacheSettings object with default values for Sliding Expiration.
        /// </summary>
        /// <param name="slidingExpiration">Sets Sliding Expiration</param>
        /// <param name="cacheEmptyResult">Sets Cache Empty Result</param>
        public CacheSettings(TimeSpan slidingExpiration, bool cacheEmptyResult)
        {
            AbsoluteExpiration = Cache.NoAbsoluteExpiration;
            Priority = CacheItemPriority.Normal;
            SlidingExpiration = slidingExpiration;
            CacheEmptyResult = cacheEmptyResult;
        }

        /// <summary>
        /// Creates a CacheSettings object with default values for Sliding Expiration.
        /// </summary>
        /// <param name="slidingExpiration">Sets Sliding Expiration</param>
        /// <param name="priority">Sets Priority</param>
        public CacheSettings(TimeSpan slidingExpiration, CacheItemPriority priority)
        {
            CacheEmptyResult = true;
            AbsoluteExpiration = Cache.NoAbsoluteExpiration;
            SlidingExpiration = slidingExpiration;
            Priority = priority;
        }

        /// <summary>
        /// Creates a CacheSettings object with default values for Absolute Expiration.
        /// </summary>
        /// <param name="absoluteExpiration">Sets Absolute Expiration</param>
        /// <param name="cacheEmptyResult">Sets Cache Empty Result</param>
        public CacheSettings(DateTime absoluteExpiration, bool cacheEmptyResult)
        {
            Priority = CacheItemPriority.Normal;
            SlidingExpiration = Cache.NoSlidingExpiration;
            AbsoluteExpiration = absoluteExpiration;
            CacheEmptyResult = cacheEmptyResult;
        }

        /// <summary>
        /// Creates a CacheSettings object with default values for Absolute Expiration.
        /// </summary>
        /// <param name="absoluteExpiration">Sets Absolute Expiration</param>
        /// <param name="priority">Sets Priority</param>
        public CacheSettings(DateTime absoluteExpiration, CacheItemPriority priority)
        {
            CacheEmptyResult = true;
            SlidingExpiration = Cache.NoSlidingExpiration;
            AbsoluteExpiration = absoluteExpiration;
            Priority = priority;
        }

        /// <summary>
        /// Creates a CacheSettings object with default values for Duration.
        /// </summary>
        /// <param name="duration">Sets Duration</param>
        /// <param name="cacheEmptyResult">Sets Cache Empty Result</param>
        public CacheSettings(int duration, bool cacheEmptyResult)
        {
            AbsoluteExpiration = Cache.NoAbsoluteExpiration;
            Priority = CacheItemPriority.Normal;
            SlidingExpiration = Cache.NoSlidingExpiration;
            Duration = duration;
            CacheEmptyResult = cacheEmptyResult;
        }

        /// <summary>
        /// Creates a CacheSettings object with default values for Duration.
        /// </summary>
        /// <param name="duration">Sets Duration</param>
        /// <param name="priority">Sets Priority</param>
        public CacheSettings(int duration, CacheItemPriority priority)
        {
            CacheEmptyResult = true;
            AbsoluteExpiration = Cache.NoAbsoluteExpiration;
            SlidingExpiration = Cache.NoSlidingExpiration;
            Duration = duration;
            Priority = priority;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The Absolute Expiration represented as (Duration) seconds from now.
        /// </summary>
        public int Duration
        {
            get
            {
                var timeSpan = DateTime.UtcNow.Subtract(AbsoluteExpiration);
                return Convert.ToInt32(timeSpan.TotalSeconds);
            }
            set
            {
                AbsoluteExpiration = DateTime.UtcNow.AddSeconds(value);
            }
        }

        /// <summary>
        /// Used for setting System.Web.Caching.Cache.Insert parameter slidingExpiration:
        /// The interval between the time the inserted object was last accessed and the
        /// time at which that object expires. If this value is the equivalent of 20
        /// minutes, the object will expire and be removed from the cache 20 minutes after
        /// it was last accessed. If you are using sliding expiration, the
        /// absoluteExpiration parameter must be System.Web.Caching.Cache.NoAbsoluteExpiration.
        /// </summary>
        public TimeSpan SlidingExpiration { get; set; }

        /// <summary>
        /// Used for setting System.Web.Caching.Cache.Insert parameter priority:
        /// The cost of the object relative to other items stored in the cache, as
        /// expressed by the System.Web.Caching.CacheItemPriority enumeration. This value is
        /// used by the cache when it evicts objects; objects with a lower cost are removed
        /// from the cache before objects with a higher cost.
        /// </summary>
        public CacheItemPriority Priority { get; set; }

        /// <summary>
        /// Used for setting System.Web.Caching.Cache.Insert parameter absoluteExpiration:
        /// The time at which the inserted object expires and is removed from the cache. To
        /// avoid possible issues with local time such as changes from standard time to
        /// daylight saving time, use System.DateTime.UtcNow rather than System.DateTime.Now
        /// for this parameter value. If you are using absolute expiration, the
        /// slidingExpiration parameter must be System.Web.Caching.Cache.NoSlidingExpiration.
        /// </summary>
        public DateTime AbsoluteExpiration { get; set; }

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

        #endregion
    }
}
