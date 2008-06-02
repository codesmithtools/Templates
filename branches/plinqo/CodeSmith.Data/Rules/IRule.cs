using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// Interface defining validation rules.
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// Gets the rule priority. The lowest number runs first.
        /// </summary>
        /// <value>The rule priority.</value>        
        int Priority { get; }
        /// <summary>
        /// Gets the target property to apply rule to.
        /// </summary>
        /// <value>The target property.</value>
        string TargetProperty { get; }
        /// <summary>
        /// Gets the error message when rule fails.
        /// </summary>
        /// <value>The error message when rule fails.</value>
        string ErrorMessage { get; }
        /// <summary>
        /// Runs the specified rule using the RuleContext.
        /// </summary>
        /// <param name="context">The current RuleContext.</param>
        void Run(RuleContext context);
    }
}
