using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// A class indicating a broken rule.
    /// </summary>
    public class BrokenRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokenRule"/> class.
        /// </summary>
        /// <param name="context">The rule context.</param>
        internal BrokenRule(RuleContext context)
        {
            Message = context.Message;
            Context = context;
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string Message { get; private set; }
        /// <summary>
        /// Gets or sets the rule context.
        /// </summary>
        /// <value>The context.</value>
        public RuleContext Context { get; private set; }
    }
}