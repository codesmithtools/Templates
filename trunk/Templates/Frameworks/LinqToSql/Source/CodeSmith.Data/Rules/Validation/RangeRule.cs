using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Properties;

namespace CodeSmith.Data.Rules.Validation
{
    /// <summary>
    /// A rule to check if the value is between a range.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public class RangeRule<T> : PropertyRuleBase where T : IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        public RangeRule(string property, T minValue, T maxValue)
            : base(property)
        {
            MaxValue = maxValue;
            MinValue = minValue;
            ErrorMessage = string.Format(
                    Resources.ValidatorRangeMessage,
                    property,
                    minValue,
                    maxValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        public RangeRule(string property, string message, T minValue, T maxValue)
            : base(property, message)
        {
            MaxValue = maxValue;
            MinValue = minValue;
        }

        /// <summary>
        /// Gets or sets the max value.
        /// </summary>
        /// <value>The max value.</value>
        public T MaxValue { get; private set; }
        /// <summary>
        /// Gets or sets the min value.
        /// </summary>
        /// <value>The min value.</value>
        public T MinValue { get; private set; }

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
            
            context.Success = false;
            
            T value = GetPropertyValue<T>(context.TrackedObject.Current);
            IComparable comparer = value as IComparable;

            if (comparer == null)
                return;

            context.Success = (comparer.CompareTo(MinValue) >= 0 && comparer.CompareTo(MaxValue) <= 0);
        }

    }

}
