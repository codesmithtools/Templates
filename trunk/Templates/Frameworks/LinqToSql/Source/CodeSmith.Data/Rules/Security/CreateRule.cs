using System;

namespace CodeSmith.Data.Rules.Security
{
    /// <summary>
    /// A rule to determine if the current user has create permission.
    /// </summary>
    /// <example>
    /// <para>Add rule using the rule manager directly indication which roles have access to create the object.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new CreateRule("Administrators", "Users"));
    /// }
    /// ]]></code>
    /// </example>
    public class CreateRule : AuthorizationRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRule"/> class.
        /// </summary>
        /// <param name="authorizedRoles">The authorized roles.</param>
        public CreateRule(params string[] authorizedRoles)
            : base(authorizedRoles)
        {}

        /// <summary>
        /// Runs the specified rule using the RuleContext.
        /// </summary>
        /// <param name="context">The current RuleContext.</param>
        public override void Run(RuleContext context)
        {
            if (context.TrackedObject.IsNew && IsAuthorized() == false)
            {
                context.Success = false;
                context.Message = String.Format("{0} is not authorized to create this object.", GetUser());
            }
        }
    }
}