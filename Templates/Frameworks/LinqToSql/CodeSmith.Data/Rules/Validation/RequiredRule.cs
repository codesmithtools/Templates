using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Properties;

namespace CodeSmith.Data.Rules.Validation
{
    /// <summary>
    /// A rule to check that the value is not null.
    /// </summary>
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
        { }

        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Run(RuleContext context)
        {
            object value = GetPropertyValue(context.TrackedObject.Current);
            context.Success = (value != null);
            context.Message = ErrorMessage;
        }
    }
}
