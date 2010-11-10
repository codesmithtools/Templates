using System;
using System.Configuration;

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
            Mode = CacheExpirationMode.Duration;
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
        /// Gets or sets the group key of the profile.
        /// </summary>
        /// <value>The name of the group key.</value>
        [ConfigurationProperty("group")]
        public string Group
        {
            get { return (string)this["group"]; }
            set { this["group"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating the cache expiration mode.
        /// </summary>
        /// <value>The cache expiration mode.</value>
        [ConfigurationProperty("mode", DefaultValue = CacheExpirationMode.Duration)]
        public CacheExpirationMode Mode
        {
            get { return (CacheExpirationMode)this["mode"]; }
            set { this["mode"] = value; }
        }

        /// <summary>
        /// Converts the profile element to a <see cref="CacheSettings"/> instance.
        /// </summary>
        /// <returns>An instance of <see cref="CacheSettings"/>.</returns>
        public CacheSettings ToCacheSettings()
        {
            var cache = new CacheSettings();
            cache.Duration = Duration;
            cache.Mode = Mode;

            if (!string.IsNullOrEmpty(Provider))
                cache.Provider = Provider;

            if (!string.IsNullOrEmpty(Group))
                cache.Group = Group;

            return cache;
        }
    }
}
