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
        /// Gets or sets the default provider.
        /// </summary>
        /// <value>The default provider.</value>
        [ConfigurationProperty("defaultProvider", DefaultValue = "HttpCacheProvider")]
        [StringValidator(MinLength = 1)]
        public string DefaultProvider
        {
            get { return base["defaultProvider"] as string; }
            set { base["defaultProvider"] = value; }
        }

        /// <summary>
        /// Gets the providers to configure jobs.
        /// </summary>
        /// <value>The job configuration providers.</value>
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
