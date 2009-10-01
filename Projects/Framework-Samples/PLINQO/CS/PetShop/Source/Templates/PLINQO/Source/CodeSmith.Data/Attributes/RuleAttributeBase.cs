using System;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Rules;

namespace CodeSmith.Data.Attributes
{
    /// <summary>
    /// A base class for rule attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class RuleAttributeBase : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleAttributeBase"/> class.
        /// </summary>
        protected RuleAttributeBase()
        {
            State = EntityState.Dirty;
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state of the object that the rule can run on.</value>
        public EntityState State { get; protected set; }

        /// <summary>
        /// Creates the rule.
        /// </summary>
        /// <param name="property">The property name this rule applies to.</param>
        /// <returns>A new instance of the rule.</returns>
        public abstract IRule CreateRule(string property);
    }
}