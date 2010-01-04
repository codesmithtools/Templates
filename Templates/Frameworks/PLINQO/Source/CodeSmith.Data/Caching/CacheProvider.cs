using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeSmith.Data.Linq;

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
        /// Saves the specified key to the cache provider for a specific duration.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <param name="duration">The duration to store the data in the cache.</param>
        public void Set<T>(string key, T data, int duration)
        {
            Set(key, data, new CacheSettings(duration));
        }

        /// <summary>
        /// Saves the specified key to the cache provider using settings from a named cache profile.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <param name="profile">The name of the cache profile to use.</param>
        public void Set<T>(string key, T data, string profile)
        {
            Set(key, data, CacheManager.GetProfile(profile));
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
        public virtual bool Remove(string key)
        {
            return Remove(key, CacheManager.DefaultGroup);
        }

        /// <summary>
        /// Removes the combined key and group name from the cache provider.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">Name of the group.</param>
        public abstract bool Remove(string key, string group);

        /// <summary>
        /// Invalidates the cache items for the default cache group.
        /// </summary>
        public void InvalidateGroup()
        {
            InvalidateGroup(CacheManager.DefaultGroup);
        }

        /// <summary>
        /// Invalidates the cache items for the specified group name.
        /// </summary>
        /// <param name="group">The group used to store the data in the cache provider.</param>
        public virtual void InvalidateGroup(string group)
        {
            if (String.IsNullOrEmpty(group))
                return;

            string key = GetGroupVersionKey(group);
            var value = Get<int>(key, null);

            if (value == 0)
                return;

            // invalidate group by incrementing value
            value++;
            Set(key, value, new CacheSettings().WithGroup(null));
        }

        /// <summary>
        /// Gets the data cached for specified key.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <returns>
        /// The object if it exists in the cache, otherwise <see langword="null"/>.
        /// </returns>
        public virtual object Get(string key)
        {
            return Get(key, CacheManager.DefaultGroup);
        }

        /// <summary>
        /// Gets the data cached for specified key.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The name of the cache group.</param>
        /// <returns>
        /// The object if it exists in the cache, otherwise <see langword="null"/>.
        /// </returns>
        public abstract object Get(string key, string group);

        /// <summary>
        /// Gets the data cached for specified key.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <returns>
        /// An instance of T if it exists in the cache, otherwise <see langword="null"/>.
        /// </returns>
        public virtual T Get<T>(string key)
        {
            return Get<T>(key, CacheManager.DefaultGroup);
        }

        /// <summary>
        /// Gets the data cached for the combined key and group name.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">Name of the group.</param>
        /// <returns>
        /// An instance of T if the item exists in the cache, otherwise <see langword="null"/>.
        /// </returns>
        public virtual T Get<T>(string key, string group)
        {
            object data = Get(key, group);
            if (data == null)
                return default(T);

            Type dataType = data.GetType();

            if (typeof(T) == dataType || typeof(T).IsAssignableFrom(dataType))
                return (T)data;

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(dataType))
                return (T)converter.ConvertFrom(data);

            if (dataType == typeof(byte[]))
            {
                if (typeof(T).IsSubclassOf(typeof(IEnumerable<T>)))
                {
                    try
                    {
                        var converted = (T)((byte[])data).ToCollection<T>();
                        return converted;
                    }
                    catch {}
                }
                else
                {
                    try
                    {
                        var converted = ((byte[])data).ToObject<T>();
                        return converted;
                    }
                    catch { }
                }
            }

            return default(T);
        }

        /// <summary>
        /// Checks to see if the specified key exists.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <returns>True if the item exists.</returns>
        public bool Exists(string key)
        {
            return Exists(key, CacheManager.DefaultGroup);
        }

        /// <summary>
        /// Checks to see if the specified key exists.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The name of the cache group.</param>
        /// <returns>True if the item exists.</returns>
        public virtual bool Exists(string key, string group)
        {
            return Get(key, group) != null;
        }

        /// <summary>
        /// Gets the default group's current version from the cache provider.
        /// </summary>
        /// <returns>The group version to append to a key.</returns>
        /// <remarks>
        /// The group version is used to combine with the key to aid in expiring keys based on group.
        /// </remarks>
        public int GetGroupVersion()
        {
            return GetGroupVersion(CacheManager.DefaultGroup);
        }

        /// <summary>
        /// Gets the group version from the cache provider.
        /// </summary>
        /// <param name="group">The group name.</param>
        /// <returns>The value of the group name to append to a key.</returns>
        /// <remarks>
        /// The group version is used to combine with the key to aid in expiring keys based on group.
        /// </remarks>
        public virtual int GetGroupVersion(string group)
        {
            if (string.IsNullOrEmpty(group))
                return 0;

            string key = GetGroupVersionKey(group);

            var version = Get<int>(key, null);
            if (version != 0)
                return version;

            version = _random.Next(1, 10000);
            Set(key, version, new CacheSettings().WithGroup(null));

            return version;
        }

        /// <summary>
        /// Gets the combined key based on the specified key and default group name.
        /// </summary>
        /// <param name="key">The key to be combined.</param>
        /// <returns>The combined key based on the specified key and group name.</returns>
        public string GetGroupKey(string key)
        {
            return GetGroupKey(key, CacheManager.DefaultGroup);
        }

        /// <summary>
        /// Gets the combined key based on the specified key and group name.
        /// </summary>
        /// <param name="key">The key to be combined.</param>
        /// <param name="group">Name of the group.</param>
        /// <returns>
        /// The combined key based on the specified key and group name.
        /// </returns>
        public virtual string GetGroupKey(string key, string group)
        {
            if (string.IsNullOrEmpty(group))
                return key;

            var groupVersion = GetGroupVersion(group);
            string cleanName = Regex.Replace(group, @"\W+", "");

            // build key {prefix}_{groupName}_{groupVersion}_{originalKey}
            return string.Format("p_{0}_{1}_{2}", cleanName, groupVersion, key);
        }

        private static string GetGroupVersionKey(string group)
        {
            string cleanName = Regex.Replace(group, @"\W+", "");
            return "g_" + cleanName;
        }
    }
}
