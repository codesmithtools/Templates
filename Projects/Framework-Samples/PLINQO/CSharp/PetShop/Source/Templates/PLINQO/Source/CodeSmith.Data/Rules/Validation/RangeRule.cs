using System;
using CodeSmith.Data.Properties;

namespace CodeSmith.Data.Rules.Validation
{
    /// <summary>
    /// A rule to check if the value is between a range.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <example>
    /// <para>Add rule using the rule manager directly.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new RangeRule<int>("Age", 18, 21));
    /// }
    /// ]]></code>
    /// <para>Add rule using the Metadata class and attribute.</para>
    /// <code><![CDATA[
    /// private class Metadata
    /// {
    ///     // fragment of the metadata class
    /// 
    ///     [Range(18, 21)]            
    ///     public int Age { get; set; }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="T:System.ComponentModel.DataAnnotations.RangeAttribute"/>
    public class RangeRule<T> : PropertyRuleBase
        where T : IComparable
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

            var value = GetPropertyValue<T>(context.TrackedObject.Current);
            IComparable comparer = value;

            if (comparer == null)
                return;

            context.Success = (comparer.CompareTo(MinValue) >= 0 && comparer.CompareTo(MaxValue) <= 0);
        }
    }
}