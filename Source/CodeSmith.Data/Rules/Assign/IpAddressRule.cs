using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace CodeSmith.Data.Rules.Assign
{
    /// <summary>
    /// Assigns the current users IP address when the entity is committed from the <see cref="System.Data.Linq.DataContext"/>.
    /// </summary>
    /// <example>
    /// <para>Add rule using the rule manager directly.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new IpAddressRule("IpAddress", EntityState.Dirty));
    /// }
    /// ]]></code>
    /// <para>Add rule using the Metadata class and attribute.</para>
    /// <code><![CDATA[
    /// private class Metadata
    /// {
    ///     // fragment of the metadata class
    /// 
    ///     [IpAddress(EntityState.Dirty)]
    ///     public string IpAddress { get; set; }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="T:CodeSmith.Data.Attributes.IpAddressAttribute"/>
    /// <remarks>
    /// If the <see cref="HttpContext"/>.Current is not null, then the 
    /// <see cref="HttpContext"/>.Current.Request.UserHostAddress is used as the IP address.
    /// Otherwise, the computers current IP address is used.
    /// </remarks>
    public class IpAddressRule : PropertyRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IpAddressRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public IpAddressRule(string property)
            : base(property)
        {
            // lower priority because we need to assign before validate
            Priority = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IpAddressRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="assignState">State of the object that can be assigned.</param>
        public IpAddressRule(string property, EntityState assignState)
            : base(property, assignState)
        {
            // lower priority because we need to assign before validate
            Priority = 10;
        }

        /// <summary>
        /// Runs this rule.
        /// </summary>
        /// <param name="context">The rule context.</param>
        public override void Run(RuleContext context)
        {
            context.Message = ErrorMessage;
            context.Success = true;

            // Only set if CanRun and if the value has not been manually changed.
            if (CanRun(context.TrackedObject) && !IsPropertyValueModified(context.TrackedObject.Original, context.TrackedObject.Current))
                SetPropertyValue(context.TrackedObject.Current, GetCurrentIpAddress());
        }

        private static string GetCurrentIpAddress()
        {
            string ip = string.Empty;

            if (HostingEnvironment.IsHosted)
            {
                HttpContext current = HttpContext.Current;
                if (current != null)
                    ip = current.Request.UserHostAddress;
            }

            if (string.IsNullOrEmpty(ip))
            {
                string host = Dns.GetHostName();
                IPAddress[] ips = Dns.GetHostAddresses(host);
                if (ips != null && ips.Length > 0)
                    ip = ips[0].ToString();
            }

            return ip;
        }
    }
}