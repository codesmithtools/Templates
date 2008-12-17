using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Rules.Validation
{
    /// <summary>
    /// A rule that run a method to validate.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    public class CustomRule<T> : PropertyRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        /// <param name="method">The method.</param>
        public CustomRule(string property, string message, Predicate<T> method)
            : base(property, message)
        {
            Method = method;
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public Predicate<T> Method { get; private set; }

        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Run(RuleContext context)
        {            
            context.Message = ErrorMessage; 
            T value = GetPropertyValue<T>(context.TrackedObject.Current);
            try
            {
                context.Success = Method.Invoke(value);
            }
            catch(Exception ex)
            {
                context.Message = "Custom Rule Error: " + ex.Message;
            }
                        
        }
    }
}
