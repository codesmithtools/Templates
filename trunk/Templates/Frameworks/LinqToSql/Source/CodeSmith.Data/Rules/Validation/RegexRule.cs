using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CodeSmith.Data.Properties;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// A rule to match a regular expression.
    /// </summary>
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
                regex.ToString());

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
            string value = GetPropertyValue(context.TrackedObject.Current) as string;
            context.Message = ErrorMessage; 
            context.Success = (value != null && Regex.IsMatch(value));            
        }
    }
}
