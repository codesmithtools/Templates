using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading;
using System.Security.Principal;

namespace CodeSmith.Data.Rules.Security
{
    /// <summary>
    /// A base class for authorization rules.
    /// </summary>
    public abstract class AuthorizationRuleBase : IRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationRuleBase"/> class.
        /// </summary>
        public AuthorizationRuleBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationRuleBase"/> class.
        /// </summary>
        /// <param name="authorizedRoles">The authorized roles.</param>
        public AuthorizationRuleBase(string[] authorizedRoles)
        {
            AuthorizedRoles = authorizedRoles;
        }

        /// <summary>
        /// Gets or sets the authorized roles.
        /// </summary>
        /// <value>The authorized roles.</value>
        public string[] AuthorizedRoles { get; set; }

        /// <summary>
        /// Determines whether this instance is authorized.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is authorized; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsAuthorized()
        {
            IPrincipal currentUser = GetPrincipal();
            foreach (string role in AuthorizedRoles)
            {
                if (currentUser.IsInRole(role))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the current username.
        /// </summary>
        /// <returns></returns>
        protected string GetUser()
        {
            IPrincipal currentUser = GetPrincipal();

            if ((currentUser != null) && (currentUser.Identity != null))
                return currentUser.Identity.Name;

            return "Anonymous";
        }

        /// <summary>
        /// Gets the current user principal.
        /// </summary>
        /// <returns></returns>
        protected static IPrincipal GetPrincipal()
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
            
            return currentUser;
        }


        /// <summary>
        /// Runs the specified rule using the RuleContext.
        /// </summary>
        /// <param name="context">The current RuleContext.</param>
        public abstract void Run(RuleContext context);
        /// <summary>
        /// Gets the error message when rule fails.
        /// </summary>
        /// <value>The error message when rule fails.</value>
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// Gets the target property to apply rule to.
        /// </summary>
        /// <value>The target property.</value>
        public string TargetProperty { get; private set; }
        /// <summary>
        /// Gets the rule priority. The lowest number runs first.
        /// </summary>
        /// <value>The rule priority.</value>
        public int Priority { get; set; }
    }
}
