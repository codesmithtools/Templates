using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace CodeSmith.Data.Rules.Assign
{
    /// <summary>
    /// Assigns the current logged in username when the entity is committed from the <see cref="System.Data.Linq.DataContext"/>.
    /// </summary>
    /// <example>
    /// <para>Add rule using the rule manager directly.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new UserNameRule("CreatedBy", EntityState.New));
    ///     RuleManager.AddShared<User>(new UserNameRule("ModifiedBy", EntityState.Dirty));
    /// }
    /// ]]></code>
    /// <para>Add rule using the Metadata class and attribute.</para>
    /// <code><![CDATA[
    /// private class Metadata
    /// {
    ///     // fragment of the metadata class
    /// 
    ///     [UserName(EntityState.New)]
    ///     public string CreatedBy { get; set; }
    /// 
    ///     [UserName(EntityState.Dirty)]
    ///     public string ModifiedBy { get; set; }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="T:CodeSmith.Data.Attributes.NowAttribute"/>
    public class UserNameRule : PropertyRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNameRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public UserNameRule(string property)
            : base(property)
        {
            // lower priority because we need to assign before validate
            Priority = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNameRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="assignState">State of the object that can be assigned.</param>
        public UserNameRule(string property, EntityState assignState)
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
            if (property.PropertyType != typeof (string))
                return;

            var value = (string) property.GetValue(current, null);
            if (CanRun(context.TrackedObject))
                property.SetValue(current, GetCurrentUserName(), null);
        }

        private static string GetCurrentUserName()
        {
            IPrincipal currentUser = null;

            if (HostingEnvironment.IsHosted)
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