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
        /// The name of the cache provider.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Saves the specified key to the cache provider.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        void Set<T>(string key, T data);

        /// <summary>
        /// Saves the specified key to the cache provider for a specific duration.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <param name="duration">The duration to store the data in the cache.</param>
        void Set<T>(string key, T data, int duration);

        /// <summary>
        /// Saves the specified key to the cache provider using settings from a named cache profile.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <param name="profile">The name of the cache profile to use.</param>
        void Set<T>(string key, T data, string profile);

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
        /// <returns>True if the item was successfully removed.</returns>
        bool Remove(string key);

        /// <summary>
        /// Removes the combined key and group name from the cache provider.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The name of the cache group.</param>
        /// <returns>True if the item was successfully removed.</returns>
        bool Remove(string key, string group);

        /// <summary>
        /// Gets the data cached for the specified key.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <returns>An object containing the cached data.</returns>
        object Get(string key);

        /// <summary>
        /// Gets the data cached for the specified key.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The name of the cache group.</param>
        /// <returns>An object containing the cached data.</returns>
        object Get(string key, string group);

        /// <summary>
        /// Gets the data cached for the specified key.
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
        /// <param name="group">The name of the cache group.</param>
        /// <returns>
        /// An instance of T if the item exists in the cache, otherwise <see langword="null"/>.
        /// </returns>
        T Get<T>(string key, string group);

        /// <summary>
        /// Saves a key/value pair to the cache if the key does not already exist.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <returns>
        /// An instance of T that will be either the existing value for the key if the key is already in the cache, 
        /// or the new value if the key was not in the cache.
        /// </returns>
        T GetOrSet<T>(string key, T data);

        /// <summary>
        /// Saves a key/value pair to the cache if the key does not already exist.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The name of the cache group.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <returns>
        /// An instance of T that will be either the existing value for the key if the key is already in the cache, 
        /// or the new value if the key was not in the cache.
        /// </returns>
        T GetOrSet<T>(string key, string group, T data);

        /// <summary>
        /// Saves a key/value pair to the cache if the key does not already exist.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <param name="settings">The <see cref="CacheSettings"/> to be used when storing in the provider.</param>
        /// <returns>
        /// An instance of T that will be either the existing value for the key if the key is already in the cache,
        /// or the new value if the key was not in the cache.
        /// </returns>
        T GetOrSet<T>(string key, T data, CacheSettings settings);

        /// <summary>
        /// Saves a key/value pair to the cache if the key does not already exist.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="valueFactory">The function used to generate a value for the key.</param>
        /// <returns>
        /// An instance of T that will be either the existing value for the key if the key is already in the cache, 
        /// or the new value if the key was not in the cache.
        /// </returns>
        T GetOrSet<T>(string key, Func<string, T> valueFactory);

        /// <summary>
        /// Saves a key/value pair to the cache if the key does not already exist.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The name of the cache group.</param>
        /// <param name="valueFactory">The function used to generate a value for the key.</param>
        /// <returns>
        /// An instance of T that will be either the existing value for the key if the key is already in the cache, 
        /// or the new value if the key was not in the cache.
        /// </returns>
        T GetOrSet<T>(string key, string group, Func<string, T> valueFactory);

        /// <summary>
        /// Saves a key/value pair to the cache if the key does not already exist.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="valueFactory">The function used to generate a value for the key.</param>
        /// <param name="settings">The <see cref="CacheSettings"/> to be used when storing in the provider.</param>
        /// <returns>
        /// An instance of T that will be either the existing value for the key if the key is already in the cache, 
        /// or the new value if the key was not in the cache.
        /// </returns>
        T GetOrSet<T>(string key, Func<string, T> valueFactory, CacheSettings settings);

        /// <summary>
        /// Checks to see if the specified key exists.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <returns>True if the item exists.</returns>
        bool Exists(string key);

        /// <summary>
        /// Checks to see if the specified key exists.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The name of the cache group.</param>
        /// <returns>True if the item exists.</returns>
        bool Exists(string key, string group);

        /// <summary>
        /// Gets the default group's current version from the cache provider.
        /// </summary>
        /// <returns>The group version to append to a key.</returns>
        /// <remarks>
        /// The group version is used to combine with the key to aid in expiring keys based on group.
        /// </remarks>
        int GetGroupVersion();

        /// <summary>
        /// Gets the specified group's current version from the cache provider.
        /// </summary>
        /// <param name="group">The name of the cache group.</param>
        /// <returns>The group version to append to a key.</returns>
        /// <remarks>
        /// The group version is used to combine with the key to aid in expiring keys based on group.
        /// </remarks>
        int GetGroupVersion(string group);

        /// <summary>
        /// Gets the combined key based on the specified key and default group name.
        /// </summary>
        /// <param name="key">The key to be combined.</param>
        /// <returns>The combined key based on the specified key and group name.</returns>
        string GetGroupKey(string key);

        /// <summary>
        /// Gets the combined key based on the specified key and group name.
        /// </summary>
        /// <param name="key">The key to be combined.</param>
        /// <param name="group">The name of the cache group.</param>
        /// <returns>The combined key based on the specified key and group name.</returns>
        string GetGroupKey(string key, string group);

        /// <summary>
        /// Invalidates the cache items for the default cache group.
        /// </summary>
        void InvalidateGroup();

        /// <summary>
        /// Invalidates the cache items for the specified cache group.
        /// </summary>
        /// <param name="group">The name of the cache group.</param>
        void InvalidateGroup(string group);

        /// <summary>
        /// Invalidates all cache items.
        /// </summary>
        void Clear();
    }
}
