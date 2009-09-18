using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace CodeSmith.Data.Linq
{
    public class CacheSettings
    {
        #region Private Members

        private TimeSpan _slidingExpiration = Cache.NoSlidingExpiration;
        private CacheItemPriority _priority = CacheItemPriority.Normal;
        private DateTime _absoluteExpiration = Cache.NoAbsoluteExpiration;
        private bool _cacheEmptyResult = true;
        private CacheDependency _cacheDependency = null;
        private CacheItemRemovedCallback _cacheItemRemovedCallback = null;

        #endregion

        #region Constructors

        public CacheSettings() { }

        public CacheSettings(DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, bool cacheEmptyResult, CacheDependency cacheDependency, CacheItemRemovedCallback cacheItemRemovedCallback)
            : this(slidingExpiration, priority, cacheEmptyResult, cacheDependency, cacheItemRemovedCallback)
        {
            AbsoluteExpiration = absoluteExpiration;
        }

        public CacheSettings(int duration, TimeSpan slidingExpiration, CacheItemPriority priority, bool cacheEmptyResult, CacheDependency cacheDependency, CacheItemRemovedCallback cacheItemRemovedCallback)
            : this(slidingExpiration, priority, cacheEmptyResult, cacheDependency, cacheItemRemovedCallback)
        {
            Duration = duration;
        }

        private CacheSettings(TimeSpan slidingExpiration, CacheItemPriority priority, bool cacheEmptyResult, CacheDependency cacheDependency, CacheItemRemovedCallback cacheItemRemovedCallback)
        {
            SlidingExpiration = slidingExpiration;
            Priority = priority;
            CacheEmptyResult = cacheEmptyResult;
            CacheDependency = cacheDependency;
            CacheItemRemovedCallback = cacheItemRemovedCallback;
        }

        public CacheSettings(TimeSpan slidingExpiration, bool cacheEmptyResult)
        {
            SlidingExpiration = slidingExpiration;
            CacheEmptyResult = cacheEmptyResult;
        }

        public CacheSettings(TimeSpan slidingExpiration, CacheItemPriority priority)
        {
            SlidingExpiration = slidingExpiration;
            Priority = priority;
        }

        public CacheSettings(DateTime absoluteExpiration, bool cacheEmptyResult)
        {
            AbsoluteExpiration = absoluteExpiration;
            CacheEmptyResult = cacheEmptyResult;
        }

        public CacheSettings(DateTime absoluteExpiration, CacheItemPriority priority)
        {
            AbsoluteExpiration = absoluteExpiration;
            Priority = priority;
        }

        public CacheSettings(int duration, bool cacheEmptyResult)
        {
            Duration = duration;
            CacheEmptyResult = cacheEmptyResult;
        }

        public CacheSettings(int duration, CacheItemPriority priority)
        {
            Duration = duration;
            Priority = priority;
        }

        #endregion

        #region Public Properties

        public int Duration
        {
            get
            {
                var timeSpan = DateTime.UtcNow.Subtract(_absoluteExpiration);
                return Convert.ToInt32(timeSpan.TotalSeconds);
            }
            set
            {
                _absoluteExpiration = DateTime.UtcNow.AddSeconds(value);
            }
        }

        public TimeSpan SlidingExpiration
        {
            get { return _slidingExpiration; }
            set { _slidingExpiration = value; }
        }

        public CacheItemPriority Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public DateTime AbsoluteExpiration
        {
            get { return _absoluteExpiration; }
            set { _absoluteExpiration = value; }
        }

        public bool CacheEmptyResult
        {
            get { return _cacheEmptyResult; }
            set { _cacheEmptyResult = value; }
        }

        public CacheDependency CacheDependency
        {
            get { return _cacheDependency; }
            set { _cacheDependency = value; }
        }

        public CacheItemRemovedCallback CacheItemRemovedCallback
        {
            get { return _cacheItemRemovedCallback; }
            set { _cacheItemRemovedCallback = value; }
        }

        #endregion
    }
}
