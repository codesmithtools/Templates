using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Caching
{
    /// <summary>
    /// Profile configuration element.
    /// </summary>
    public class ProfileElement : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileElement"/> class.
        /// </summary>
        public ProfileElement()
        {
            IsSlidingExpiration = false;
            Description = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the profile.
        /// </summary>
        /// <value>The name of the profile.</value>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the description for the profile.
        /// </summary>
        /// <value>The description for the profile.</value>
        [ConfigurationProperty("description", DefaultValue = "")]
        public string Description
        {
            get { return this["description"] as string; }
            set { this["description"] = value; }
        }

        /// <summary>
        /// Gets or sets the expiration duration.
        /// </summary>
        /// <value>The expiration duration.</value>
        [ConfigurationProperty("duration", IsRequired = true)]
        public TimeSpan Duration
        {
            get { return (TimeSpan)this["duration"]; }
            set { this["duration"] = value; }
        }

        /// <summary>
        /// Gets or sets the cache provider name.
        /// </summary>
        /// <value>The cache provider name.</value>
        [ConfigurationProperty("provider")]
        public string Provider
        {
            get { return this["provider"] as string; }
            set { this["provider"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cache is sliding expiration.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the cache is sliding expiration; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("isSlidingExpiration", DefaultValue = false)]
        public bool IsSlidingExpiration
        {
            get { return (bool)this["isSlidingExpiration"]; }
            set { this["isSlidingExpiration"] = value; }
        }

        public CacheSettings ToCacheSettings()
        {
            var cache = new CacheSettings();
            cache.Duration = Duration;
            cache.Mode = IsSlidingExpiration ? CacheExpirationMode.Sliding : CacheExpirationMode.Duration;

            if (!string.IsNullOrEmpty(Provider))
                cache.Provider = Provider;

            return cache;
        }
    }
}
