using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CodeSmith.Data.Rules.Security
{
    /// <summary>
    /// A rule to determine if the current user has delete permission.
    /// </summary>
    public class DeleteRule : AuthorizationRuleBase
    {
        /// <summary>
        /// Runs the specified rule using the RuleContext.
        /// </summary>
        /// <param name="context">The current RuleContext.</param>
        public override void Run(RuleContext context)
        {
            if (context.TrackedObject.IsDeleted && IsAuthorized() == false)
            {
                context.Success = false;
                context.Message = String.Format("{0} is not authorized to delete this object.", GetUser());
            }
        }
    }
}
