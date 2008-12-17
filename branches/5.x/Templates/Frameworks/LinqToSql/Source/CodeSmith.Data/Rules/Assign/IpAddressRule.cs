using System.Net;
using System.Reflection;
using System.Web;

namespace CodeSmith.Data.Rules.Assign
{
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

            object current = context.TrackedObject.Current;
            PropertyInfo property = GetPropertyInfo(current);
            if (property.PropertyType != typeof(string))
                return;

            string value = (string)property.GetValue(current, null);
            if (CanRun(context.TrackedObject))
                property.SetValue(current, GetCurrentIpAddress(), null);
        }

        private static string GetCurrentIpAddress()
        {
            string ip = string.Empty;

            if (System.Web.Hosting.HostingEnvironment.IsHosted)
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
