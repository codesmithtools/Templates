using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Linq;

namespace CodeSmith.Data.Caching
{
    /// <summary>
    /// An interface defining a cache provider.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Saves the specified key to the cache provider.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        void Set<T>(string key, T data);

        /// <summary>
        /// Saves the specified key to the cache provider.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <param name="settings">The <see cref="CacheSettings"/> to be used when storing in the provider.</param>
        void Set<T>(string key, T data, CacheSettings settings);

        /// <summary>
        /// Removes the specified key from the cache provider.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        void Remove(string key);

        /// <summary>
        /// Removes the combined key and group name from the cache provider.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="groupName">Name of the group.</param>
        void Remove(string key, string groupName);

        /// <summary>
        /// Gets the data cached for specified key.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <returns>An instance of T if the item exists in the cache, otherwise <see langword="null"/>.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Gets the data cached for the combined key and group name.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>
        /// An instance of T if the item exists in the cache, otherwise <see langword="null"/>.
        /// </returns>
        T Get<T>(string key, string groupName);

        /// <summary>
        /// Gets the group value from the cache provider.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>The value of the group name to append to a key.</returns>
        /// <remarks>
        /// The group value is used to combine with the key to aid in expiring keys based on group.
        /// </remarks>
        int GetGroupVersion(string groupName);

        /// <summary>
        /// Gets the combined key based on the specified key and group name.
        /// </summary>
        /// <param name="key">The key to be combined.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>The combined key based on the specified key and group name.</returns>
        string GetGroupKey(string key, string groupName);

        /// <summary>
        /// Invalidates the cache items for the specified group name.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        void InvalidateGroup(string groupName);
    }
}
