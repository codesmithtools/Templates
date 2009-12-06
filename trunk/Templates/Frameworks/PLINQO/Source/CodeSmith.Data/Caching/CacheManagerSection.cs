using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Caching
{
    public class CacheManagerSection : ConfigurationSection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheManagerSection"/> class.
        /// </summary>
        public CacheManagerSection()
        {
            DefaultProvider = "HttpCacheProvider";
        }

        /// <summary>
        /// Gets or sets the default provider name.
        /// </summary>
        /// <value>The default provider name.</value>
        [ConfigurationProperty("defaultProvider", DefaultValue = "HttpCacheProvider")]
        [StringValidator(MinLength = 1)]
        public string DefaultProvider
        {
            get { return base["defaultProvider"] as string; }
            set { base["defaultProvider"] = value; }
        }

        /// <summary>
        /// Gets or sets the default profile name.
        /// </summary>
        /// <value>The default profile name.</value>
        [ConfigurationProperty("defaultProfile")]
        public string DefaultProfile
        {
            get { return base["defaultProfile"] as string; }
            set { base["defaultProfile"] = value; }
        }

        /// <summary>
        /// Gets the cache profiles.
        /// </summary>
        /// <value>The cache profiles.</value>
        [ConfigurationProperty("profiles")]
        public ProfileElementCollection Profiles
        {
            get
            {
                return this["profiles"] as ProfileElementCollection;
            }
        }

        /// <summary>
        /// Gets the cache providers.
        /// </summary>
        /// <value>The cache providers.</value>
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get
            {
                return this["providers"] as ProviderSettingsCollection;
            }
        }
    }
}
