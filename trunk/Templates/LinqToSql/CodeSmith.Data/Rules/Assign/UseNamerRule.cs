using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Web;
using System.Threading;
using System.Reflection;

namespace CodeSmith.Data.Rules.Assign
{
    /// <summary>
    /// Assigns the current logged in username.
    /// </summary>
    public class UseNamerRule : PropertyRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultUseNamerRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public UseNamerRule(string property)
            : base(property)
        {
            // lower priority because we need to assign before validate
            Priority = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultUseNamerRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="assignState">State of the object that can be assigned.</param>
        public UseNamerRule(string property, EntityState assignState)
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
                property.SetValue(current, GetCurrentUserName(), null);
        }

        private static string GetCurrentUserName()
        {
            IPrincipal currentUser = null;

            if (System.Web.Hosting.HostingEnvironment.IsHosted)
            {
                HttpContext current = HttpContext.Current;
                if (current != null)
                    currentUser = current.User;
            }
            
            if (currentUser == null)
                currentUser = Thread.CurrentPrincipal;

            if ((currentUser != null) && (currentUser.Identity != null))
                return currentUser.Identity.Name;
            
            return string.Empty;
        }



    }
}
