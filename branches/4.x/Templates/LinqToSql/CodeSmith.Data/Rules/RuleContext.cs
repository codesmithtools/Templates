using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using CodeSmith.Data;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// The context for a rule.
    /// </summary>
    public class RuleContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleContext"/> class.
        /// </summary>
        /// <param name="trackedObject">The tracked object.</param>
        /// <param name="rule">The rule to apply.</param>
        public RuleContext(TrackedObject trackedObject, IRule rule)
        {
            TrackedObject = trackedObject;
            Rule = rule;
            Message = rule.ErrorMessage;
            Success = false;
        }

        /// <summary>
        /// Gets or sets the tracked object.
        /// </summary>
        /// <value>The tracked object.</value>
        public TrackedObject TrackedObject { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RuleContext"/> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        public bool Success { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets the rule to run.
        /// </summary>
        /// <value>The rule to run.</value>
        public IRule Rule { get; private set; }
    }
}
