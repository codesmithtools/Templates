using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeSmith.Data.Caching
{
    /// <summary>
    /// A base class for cache providers.
    /// </summary>
    public abstract class CacheProvider : ProviderBase, ICacheProvider
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Saves the specified key to the cache provider.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        public virtual void Set<T>(string key, T data)
        {
            Set(key, data, CacheManager.GetProfile());
        }

        /// <summary>
        /// Saves the specified key to the cache provider.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <param name="settings">The <see cref="CacheSettings"/> to be used when storing in the provider.</param>
        public abstract void Set<T>(string key, T data, CacheSettings settings);

        /// <summary>
        /// Removes the specified key from the cache provider.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        public abstract void Remove(string key);

        /// <summary>
        /// Removes the combined key and group name from the cache provider.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="groupName">Name of the group.</param>
        public virtual void Remove(string key, string groupName)
        {
            string groupKey = GetGroupKey(key, groupName);
            Remove(groupKey);
        }

        /// <summary>
        /// Invalidates the cache items for the specified group name.
        /// </summary>
        /// <param name="groupName">The group used to store the data in the cache provider.</param>
        public virtual void InvalidateGroup(string groupName)
        {
            string key = GetGroupVersionKey(groupName);
            var value = Get<int>(key);

            if (value == 0)
                return;

            // invalidate group by incrementing value
            value++;
            Set(key, value, new CacheSettings());
        }

        /// <summary>
        /// Gets the data cached for specified key.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <returns>
        /// An instance of T if the item exists in the cache, otherwise <see langword="null"/>.
        /// </returns>
        public abstract T Get<T>(string key);

        /// <summary>
        /// Gets the data cached for the combined key and group name.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>
        /// An instance of T if the item exists in the cache, otherwise <see langword="null"/>.
        /// </returns>
        public virtual T Get<T>(string key, string groupName)
        {
            string groupKey = GetGroupKey(key, groupName);
            return Get<T>(groupKey);
        }

        /// <summary>
        /// Gets the group version from the cache provider.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        /// <returns>The value of the group name to append to a key.</returns>
        /// <remarks>
        /// The group version is used to combine with the key to aid in expiring keys based on group.
        /// </remarks>
        public virtual int GetGroupVersion(string groupName)
        {
            string key = GetGroupVersionKey(groupName);

            var version = Get<int>(key);
            if (version != 0)
                return version;

            version = _random.Next(1, 10000);
            Set(key, version, new CacheSettings());

            return version;
        }


        /// <summary>
        /// Gets the combined key based on the specified key and group name.
        /// </summary>
        /// <param name="key">The key to be combined.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>
        /// The combined key based on the specified key and group name.
        /// </returns>
        public virtual string GetGroupKey(string key, string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return key;

            var groupVersion = GetGroupVersion(groupName);
            string cleanName = Regex.Replace(groupName, @"\W+", "");

            // build key {prefix}_{groupName}_{groupVersion}_{originalKey}
            return string.Format("p_{0}_{1}_{2}", cleanName, groupVersion, key);
        }

        private static string GetGroupVersionKey(string groupName)
        {
            string cleanName = Regex.Replace(groupName, @"\W+", "");
            return "g_" + cleanName;
        }

    }
}
