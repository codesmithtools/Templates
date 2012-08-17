using System;
using CodeSmith.Data.Properties;
using System.Data.SqlTypes;

namespace CodeSmith.Data.Rules.Validation
{
    /// <summary>
    /// A rule to check that the value is not null.
    /// </summary>
    /// <example>
    /// <para>Add rule using the rule manager directly.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new RequiredRule("UserName"));
    /// }
    /// ]]></code>
    /// <para>Add rule using the Metadata class and attribute.</para>
    /// <code><![CDATA[
    /// private class Metadata
    /// {
    ///     // fragment of the metadata class
    /// 
    ///     [Required]
    ///     public string UserName { get; set; }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="T:System.ComponentModel.DataAnnotations.RequiredAttribute"/>
    public class RequiredRule : PropertyRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredRule"/> class.
        /// </summary>
        /// <param name="property">The target property to apply rule to.</param>
        public RequiredRule(string property)
            : base(property)
        {
            ErrorMessage = string.Format(
                Resources.ValidatorRequiredMessage,
                property);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        public RequiredRule(string property, string message)
            : base(property, message)
        {
        }

        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The rule context.</param>
        public override void Run(RuleContext context)
        {
            context.Message = ErrorMessage;
            context.Success = true;

            if (!CanRun(context.TrackedObject))
                return;

            object value = GetPropertyValue(context.TrackedObject.Current);

            if (value is string && value != null)
            {
                context.Success = ((string)value).Trim().Length > 0;
            }
            else if (value is Guid)
            {
                context.Success = (Guid)value != Guid.Empty;
            }
            else if (value is DateTime)
            {
                context.Success = (DateTime)value > (DateTime)SqlDateTime.MinValue;
            }
            else
            {
                context.Success = value != null;
            }
        }
    }
}