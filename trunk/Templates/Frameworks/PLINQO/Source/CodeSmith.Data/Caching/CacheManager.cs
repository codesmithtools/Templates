using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using CodeSmith.Data.Collections;

namespace CodeSmith.Data.Caching
{
    public class CacheManager
    {
        private readonly ConcurrentDictionary<string, ICacheProvider> _providers;
        private readonly ConcurrentDictionary<string, CacheSettings> _profiles;

        #region Singleton

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheManager"/> class.
        /// </summary>
        public CacheManager()
        {
            _providers = new ConcurrentDictionary<string, ICacheProvider>(StringComparer.OrdinalIgnoreCase);
            _profiles = new ConcurrentDictionary<string, CacheSettings>(StringComparer.OrdinalIgnoreCase);

            Register<HttpCacheProvider>(true);
            DefaultProfile = new CacheSettings();
            Initialize();
        }

        /// <summary>
        /// Gets the current singleton instance of <see cref="CacheManager"/>.
        /// </summary>
        /// <value>The current singleton instance.</value>
        /// <remarks>
        /// An instance of <see cref="CacheManager"/> wont be created until the very first 
        /// call to the sealed class. This is a CLR optimization that
        /// provides a properly lazy-loading singleton. 
        /// </remarks>
        public static CacheManager Current
        {
            get { return Nested.Current; }
        }

        /// <summary>
        /// Nested class to lazy-load singleton.
        /// </summary>
        private class Nested
        {
            /// <summary>
            /// Current singleton instance.
            /// </summary>
            internal readonly static CacheManager Current = new CacheManager();
        }

        #endregion

        private void Initialize()
        {
            var cacheSection = ConfigurationManager.GetSection("cacheManager") as CacheManagerSection;
            if (cacheSection == null)
                return;

            //load providers
            if (cacheSection.Providers.Count > 0)
            {
                var cacheProviders = new CacheProviderCollection();
                ProvidersHelper.InstantiateProviders(cacheSection.Providers, cacheProviders, typeof (CacheProvider));
                foreach (CacheProvider provider in cacheProviders)
                    _providers.TryAdd(provider.Name, provider);

                ICacheProvider cacheProvider;
                if (!string.IsNullOrEmpty(cacheSection.DefaultProvider))
                    if (Providers.TryGetValue(cacheSection.DefaultProvider, out cacheProvider))
                        DefaultProvider = cacheProvider;
            }

            //load profiles
            if (cacheSection.Profiles.Count > 0)
            {
                foreach (ProfileElement profile in cacheSection.Profiles)
                    _profiles.TryAdd(profile.Name, profile.ToCacheSettings());

                CacheSettings cacheSettings;
                if (!string.IsNullOrEmpty(cacheSection.DefaultProfile))
                    if (Profiles.TryGetValue(cacheSection.DefaultProfile, out cacheSettings))
                        DefaultProfile = cacheSettings;
            }
        }

        /// <summary>
        /// Gets or sets the default provider.
        /// </summary>
        /// <value>The default provider.</value>
        public ICacheProvider DefaultProvider { get; private set; }

        public IDictionary<string, ICacheProvider> Providers
        {
            get { return _providers; }
        }

        public CacheSettings DefaultProfile { get; private set; }

        public IDictionary<string, CacheSettings> Profiles
        {
            get { return _profiles; }
        }

        /// <summary>
        /// Registers the specified provider name.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="defaultProvider">if set to <c>true</c> this provider will be set as default.</param>
        /// <returns>A new instance of the provider.</returns>
        public ICacheProvider Register<T>(string providerName, bool defaultProvider) where T : ICacheProvider, new()
        {
            ICacheProvider provider = _providers.GetOrAdd(providerName, k => new T());
            if (defaultProvider)
                DefaultProvider = provider;

            return provider;
        }

        /// <summary>
        /// Registers the specified provider name.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <returns>A new instance of the provider.</returns>
        public ICacheProvider Register<T>() where T : ICacheProvider, new()
        {
            return Register<T>(false);
        }

        /// <summary>
        /// Registers the specified provider name.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <param name="defaultProvider">if set to <c>true</c> this provider will be set as default.</param>
        /// <returns>A new instance of the provider.</returns>
        public ICacheProvider Register<T>(bool defaultProvider) where T : ICacheProvider, new()
        {
            return Register<T>(typeof(T).Name, defaultProvider);
        }

        /// <summary>
        /// Gets the provider by name. If <paramref name="providerName"/> is <see langword="null"/>, <see cref="DefaultProvider"/> is returned.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>An instance of the provider.</returns>
        public ICacheProvider GetProvider(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
                return DefaultProvider;

            ICacheProvider provider;
            if (!Providers.TryGetValue(providerName, out provider))
                throw new ArgumentException(string.Format(
                    "Unable to locate cache provider '{0}'.", providerName), "providerName");

            return provider;
        }

        /// <summary>
        /// Gets the provider by the type.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <returns>An instance of the provider.</returns>
        public ICacheProvider GetProvider<T>() where T : ICacheProvider
        {
            return GetProvider(typeof(T).Name);
        }

        /// <summary>
        /// Gets an instance of <see cref="CacheSettings"/> based on the profile name. 
        /// If the profile name is not found, the <see cref="DefaultProfile"/> is used.
        /// </summary>
        /// <param name="profileName">Name of the cache profile.</param>
        /// <returns>An instance of <see cref="CacheSettings"/>.</returns>
        public CacheSettings GetProfile(string profileName)
        {
            CacheSettings cacheSettings;
            Profiles.TryGetValue(profileName, out cacheSettings);

            // throw error?
            if (cacheSettings == null)
                cacheSettings = DefaultProfile ?? new CacheSettings();

            return cacheSettings;
        }

    }
}
