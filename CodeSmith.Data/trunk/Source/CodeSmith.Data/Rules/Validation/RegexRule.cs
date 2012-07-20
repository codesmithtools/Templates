using System.Text.RegularExpressions;
using CodeSmith.Data.Properties;

namespace CodeSmith.Data.Rules.Validation
{
    /// <summary>
    /// A rule to match a regular expression.
    /// </summary>
    /// <example>
    /// <para>Add rule using the rule manager directly.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new RegexRule("EmailAddress", @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"));
    /// }
    /// ]]></code>
    /// <para>Add rule using the Metadata class and attribute.</para>
    /// <code><![CDATA[
    /// private class Metadata
    /// {
    ///     // fragment of the metadata class
    /// 
    ///     [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
    ///     public string EmailAddress { get; set; }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="T:System.ComponentModel.DataAnnotations.RegularExpressionAttribute"/>
    public class RegexRule : PropertyRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="pattern">The pattern.</param>
        public RegexRule(string property, string pattern)
            : base(property)
        {
            Regex = new Regex(pattern);
            ErrorMessage = string.Format(
                Resources.ValidatorRegexMessage,
                property,
                pattern);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        /// <param name="pattern">The pattern.</param>
        public RegexRule(string property, string message, string pattern)
            : base(property, message)
        {
            Regex = new Regex(pattern);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="regex">The regex.</param>
        public RegexRule(string property, Regex regex)
            : base(property)
        {
            Regex = regex;
            ErrorMessage = string.Format(
                Resources.ValidatorRegexMessage,
                property,
                regex);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        /// <param name="regex">The regex.</param>
        public RegexRule(string property, string message, Regex regex)
            : base(property, message)
        {
            Regex = regex;
        }

        /// <summary>
        /// Gets or sets the regex.
        /// </summary>
        /// <value>The regex.</value>
        public Regex Regex { get; private set; }

        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Run(RuleContext context)
        {
            context.Message = ErrorMessage;
            context.Success = true;

            if (!CanRun(context.TrackedObject))
                return;

            var value = GetPropertyValue(context.TrackedObject.Current) as string;
            context.Success = (value != null && Regex.IsMatch(value));
        }
    }
}