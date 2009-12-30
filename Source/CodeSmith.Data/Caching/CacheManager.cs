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
    /// // get the default provider
    /// var provider = CacheManager.GetProvider();
    /// provider.Set("key", "some cached data");
    /// var data = provider.Get<string>("key");
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

            Register<HttpCacheProvider>(true);
            _defaultProfile = new CacheSettings();
            Initialize();
        }

        private static void Initialize()
        {
            var cacheSection = ConfigurationManager.GetSection("cacheManager") as CacheManagerSection;
            if (cacheSection == null)
                return;

            //load providers
            if (cacheSection.Providers.Count > 0)
            {
                var cacheProviders = new CacheProviderCollection();
                ProvidersHelper.InstantiateProviders(cacheSection.Providers, cacheProviders, typeof(CacheProvider));
                foreach (CacheProvider provider in cacheProviders)
                    _providers.TryAdd(provider.Name, provider);

                ICacheProvider cacheProvider;
                if (!string.IsNullOrEmpty(cacheSection.DefaultProvider))
                    if (_providers.TryGetValue(cacheSection.DefaultProvider, out cacheProvider))
                        _defaultProvider = cacheProvider;
            }

            //load profiles
            if (cacheSection.Profiles.Count > 0)
            {
                foreach (ProfileElement profile in cacheSection.Profiles)
                    _profiles.TryAdd(profile.Name, profile.ToCacheSettings());

                CacheSettings cacheSettings;
                if (!string.IsNullOrEmpty(cacheSection.DefaultProfile))
                    if (_profiles.TryGetValue(cacheSection.DefaultProfile, out cacheSettings))
                        _defaultProfile = cacheSettings;
            }
        }

        /// <summary>
        /// Registers the specified provider name.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="defaultProvider">if set to <c>true</c> this provider will be set as default.</param>
        /// <returns>A new instance of the provider.</returns>
        public static ICacheProvider Register<T>(string providerName, bool defaultProvider) where T : ICacheProvider, new()
        {
            return Register<T>(providerName, defaultProvider, k => new T());
        }

        /// <summary>
        /// Registers the specified provider name.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="defaultProvider">if set to <c>true</c> this provider will be set as default.</param>
        /// <param name="createFactory">The factory to create a new provider.</param>
        /// <returns>A new instance of the provider.</returns>
        public static ICacheProvider Register<T>(string providerName, bool defaultProvider, Func<string, ICacheProvider> createFactory) where T : ICacheProvider, new()
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
        public static ICacheProvider Register<T>() where T : ICacheProvider, new()
        {
            return Register<T>(false);
        }

        /// <summary>
        /// Registers the specified provider name.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <param name="defaultProvider">if set to <c>true</c> this provider will be set as default.</param>
        /// <returns>A new instance of the provider.</returns>
        public static ICacheProvider Register<T>(bool defaultProvider) where T : ICacheProvider, new()
        {
            return Register<T>(typeof(T).Name, defaultProvider);
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
        /// Gets an instance of <see cref="CacheSettings"/> based the default profile.
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

    }
}
