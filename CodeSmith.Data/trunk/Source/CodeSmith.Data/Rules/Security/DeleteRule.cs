using System;

namespace CodeSmith.Data.Rules.Security
{
    /// <summary>
    /// A rule to determine if the current user has delete permission.
    /// </summary>
    /// <example>
    /// <para>Add rule using the rule manager directly indication which roles have access to delete the object.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new DeleteRule("Administrators"));
    /// }
    /// ]]></code>
    /// </example>
    public class DeleteRule : AuthorizationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteRule"/> class.
        /// </summary>
        /// <param name="authorizedRoles">The authorized roles.</param>
        public DeleteRule(params string[] authorizedRoles)
            : base(authorizedRoles)
        {}

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