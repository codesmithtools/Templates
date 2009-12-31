using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using CodeSmith.Data.Collections;

namespace CodeSmith.Data.Caching
{
    /// <summary>
    /// A class to manage cached items via a provider.
    /// </summary>
    /// <example>The following example gets an item to the default cache provider.
    /// <code><![CDATA[
    /// CacheManager.Set("key", "some cached data");
    /// var data = CacheManager.Get<string>("key");
    /// ]]>
    /// </code>
    /// </example>
    /// <example>The following example uses CacheManager to expire a cache group.
    /// <code><![CDATA[
    /// var db = new TrackerDataContext { Log = Console.Out };
    /// // get a CacheSettings instance using the default profile with a group of 'Role'.
    /// var cache = CacheManager.GetProfile().WithGroup("Role");
    /// 
    /// // queries that can be cached
    /// var roles = db.Role
    ///     .Where(r => r.Name == "Test Role")
    ///     .FromCache(cache);
    /// var role = db.Role
    ///     .ByName("Duck Roll")
    ///     .FromCacheFirstOrDefault(cache);
    /// 
    /// // after you make some update, expire group using InvalidateGroup
    /// CacheManager.GetProvider().InvalidateGroup("Role");
    /// ]]>
    /// </code>
    /// </example>
    public class CacheManager
    {
        private static readonly ConcurrentDictionary<string, ICacheProvider> _providers;
        private static readonly ConcurrentDictionary<string, CacheSettings> _profiles;
        private static CacheSettings _defaultProfile;
        private static ICacheProvider _defaultProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheManager"/> class.
        /// </summary>
        static CacheManager()
        {
            _providers = new ConcurrentDictionary<string, ICacheProvider>(StringComparer.OrdinalIgnoreCase);
            _profiles = new ConcurrentDictionary<string, CacheSettings>(StringComparer.OrdinalIgnoreCase);

            RegisterProvider<HttpCacheProvider>(true);
            Initialize();
            _defaultProfile = new CacheSettings();
        }

        private static void Initialize()
        {
            var cacheSection = ConfigurationManager.GetSection("cacheManager") as CacheManagerSection;
            if (cacheSection == null)
                return;

            // load providers
            if (cacheSection.Providers.Count > 0)
            {
                var cacheProviders = new CacheProviderCollection();
                ProvidersHelper.InstantiateProviders(cacheSection.Providers, cacheProviders, typeof(ICacheProvider));
                foreach (ICacheProvider provider in cacheProviders)
                    _providers.TryAdd(provider.Name, provider);

                ICacheProvider cacheProvider;
                if (!string.IsNullOrEmpty(cacheSection.DefaultProvider))
                    if (_providers.TryGetValue(cacheSection.DefaultProvider, out cacheProvider))
                        _defaultProvider = cacheProvider;
            }

            // load profiles
            if (cacheSection.Profiles.Count > 0)
            {
                foreach (ProfileElement profile in cacheSection.Profiles)
                    _profiles.TryAdd(profile.Name, profile.ToCacheSettings());

                CacheSettings cacheSettings;
                if (!string.IsNullOrEmpty(cacheSection.DefaultProfile))
                    if (_profiles.TryGetValue(cacheSection.DefaultProfile, out cacheSettings))
                        _defaultProfile = cacheSettings;
            }

            if (!String.IsNullOrEmpty(cacheSection.DefaultGroup))
                DefaultGroup = cacheSection.DefaultGroup;
        }

        /// <summary>
        /// Registers the specified provider name.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="defaultProvider">if set to <c>true</c> this provider will be set as default.</param>
        /// <returns>A new instance of the provider.</returns>
        public static ICacheProvider RegisterProvider<T>(string providerName, bool defaultProvider) where T : ICacheProvider, new()
        {
            return RegisterProvider(providerName, defaultProvider, k => new T());
        }

        /// <summary>
        /// Registers the specified provider name.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="defaultProvider">if set to <c>true</c> this provider will be set as default.</param>
        /// <param name="createFactory">The factory to create a new provider.</param>
        /// <returns>A new instance of the provider.</returns>
        public static ICacheProvider RegisterProvider(string providerName, bool defaultProvider, Func<string, ICacheProvider> createFactory)
        {
            var provider = _providers.GetOrAdd(providerName, createFactory);
            if (defaultProvider)
                _defaultProvider = provider;

            return provider;
        }

        /// <summary>
        /// Registers the specified provider name.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <returns>A new instance of the provider.</returns>
        public static ICacheProvider RegisterProvider<T>() where T : ICacheProvider, new()
        {
            return RegisterProvider<T>(false);
        }

        /// <summary>
        /// Registers the specified provider name.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <param name="defaultProvider">if set to <c>true</c> this provider will be set as default.</param>
        /// <returns>A new instance of the provider.</returns>
        public static ICacheProvider RegisterProvider<T>(bool defaultProvider) where T : ICacheProvider, new()
        {
            return RegisterProvider<T>(typeof(T).Name, defaultProvider);
        }

        /// <summary>
        /// Gets the default cache provider.
        /// </summary>
        /// <returns>An instance of the provider.</returns>
        public static ICacheProvider GetProvider()
        {
            return GetProvider(null);
        }

        /// <summary>
        /// Gets the provider by name. If <paramref name="providerName"/> is <see langword="null"/>, the default provider is returned.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>An instance of the provider.</returns>
        public static ICacheProvider GetProvider(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
                return _defaultProvider;

            ICacheProvider provider;
            if (!_providers.TryGetValue(providerName, out provider))
                throw new ArgumentException(string.Format(
                    "Unable to locate cache provider '{0}'.", providerName), "providerName");

            return provider;
        }

        /// <summary>
        /// Gets the provider by the type.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <returns>An instance of the provider.</returns>
        public static ICacheProvider GetProvider<T>() where T : ICacheProvider
        {
            return GetProvider(typeof(T).Name);
        }

        /// <summary>
        /// Gets an instance of <see cref="CacheSettings"/> based on the default profile.
        /// </summary>
        /// <returns>An instance of <see cref="CacheSettings"/>.</returns>
        public static CacheSettings GetProfile()
        {
            return GetProfile(null);
        }

        /// <summary>
        /// Gets an instance of <see cref="CacheSettings"/> based on the profile name. 
        /// If the profile name is not found, the default profile is used.
        /// </summary>
        /// <param name="profileName">Name of the cache profile.</param>
        /// <returns>An instance of <see cref="CacheSettings"/>.</returns>
        public static CacheSettings GetProfile(string profileName)
        {
            if (string.IsNullOrEmpty(profileName))
                return _defaultProfile.Clone();

            CacheSettings cacheSettings;
            _profiles.TryGetValue(profileName, out cacheSettings);

            if (cacheSettings == null)
                cacheSettings = _defaultProfile;

            return (cacheSettings == null)
                ? new CacheSettings()
                : cacheSettings.Clone();
        }

        /// <summary>
        /// The default group for cache items to be inserted into.
        /// </summary>
        public static string DefaultGroup { get; set; }

        /// <summary>
        /// The default provider for cache items.
        /// </summary>
        public static ICacheProvider DefaultProvider
        {
            get
            {
                return _defaultProvider;
            }
            set
            {
                _defaultProvider = value;
            }
        }

        /// <summary>
        /// Saves the specified key to the default cache provider.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        public static void Set<T>(string key, T data)
        {
            DefaultProvider.Set(key, data);
        }

        /// <summary>
        /// Saves the specified key to the default cache provider for a specific duration.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <param name="duration">The duration to store the data in the cache.</param>
        public static void Set<T>(string key, T data, int duration)
        {
            DefaultProvider.Set(key, data, CacheSettings.FromDuration(duration));
        }

        /// <summary>
        /// Saves the specified key using the settings from the specified cache profile.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <param name="profile">The name of the cache profile to use.</param>
        public static void Set<T>(string key, T data, string profile)
        {
            CacheSettings settings = GetProfile(profile);
            GetProvider(settings.Provider).Set(key, data, settings);
        }

        /// <summary>
        /// Saves the specified key using the settings from the specified cache profile.
        /// </summary>
        /// <typeparam name="T">The type for data being saved,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="data">The data to be cached in the provider.</param>
        /// <param name="settings">The <see cref="CacheSettings"/> to be used when storing in the provider.</param>
        public static void Set<T>(string key, T data, CacheSettings settings)
        {
            GetProvider(settings.Provider).Set(key, data, settings);
        }

        /// <summary>
        /// Removes the specified key from the default cache provider.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        public static bool Remove(string key)
        {
            return DefaultProvider.Remove(key);
        }

        /// <summary>
        /// Removes the combined key and group name from the default cache provider.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="groupName">Name of the group.</param>
        public static bool Remove(string key, string groupName)
        {
            return DefaultProvider.Remove(key, groupName);
        }

        /// <summary>
        /// Gets the data cached for the specified key from the default cache provider.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <returns>An instance of T if the item exists in the cache, otherwise <see langword="null"/>.</returns>
        public static object Get(string key)
        {
            return DefaultProvider.Get(key);
        }

        /// <summary>
        /// Gets the data cached for the specified key from the default cache provider.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <returns>An instance of T if the item exists in the cache, otherwise <see langword="null"/>.</returns>
        public static T Get<T>(string key)
        {
            return DefaultProvider.Get<T>(key);
        }

        /// <summary>
        /// Gets the data cached for the combined key and group name from the default cache provider.
        /// </summary>
        /// <typeparam name="T">The type for data being retrieved from cache,</typeparam>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>
        /// An instance of T if the item exists in the cache, otherwise <see langword="null"/>.
        /// </returns>
        public static T Get<T>(string key, string groupName)
        {
            return DefaultProvider.Get<T>(key, groupName);
        }

        /// <summary>
        /// Checks to see if the specified key exists.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <returns>True if the item exists.</returns>
        public static bool Exists(string key)
        {
            return DefaultProvider.Exists(key);
        }

        /// <summary>
        /// Checks to see if the specified key exists.
        /// </summary>
        /// <param name="key">The key used to store the data in the cache provider.</param>
        /// <param name="group">The name of the cache group.</param>
        /// <returns>True if the item exists.</returns>
        public static bool Exists(string key, string group)
        {
            return DefaultProvider.Exists(key, group);
        }

        /// <summary>
        /// Invalidates the cache items for the default group from the default cache provider.
        /// </summary>
        public static void InvalidateGroup()
        {
            DefaultProvider.InvalidateGroup(DefaultGroup);
        }

        /// <summary>
        /// Invalidates the cache items for the specified group from the default cache provider.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        public static void InvalidateGroup(string groupName)
        {
            DefaultProvider.InvalidateGroup(groupName);
        }
    }
}
