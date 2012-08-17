using System;
using CodeSmith.Data.Properties;

namespace CodeSmith.Data.Rules.Validation
{
    /// <summary>
    /// A rule to compare values.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <example>
    /// <para>Add rule using the rule manager directly.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new CompareRule<int>("Age", 21, ComparisonOperator.GreaterThanOrEqual));
    /// }
    /// ]]></code>
    /// </example>
    public class CompareRule<T> : PropertyRuleBase
        where T : IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public CompareRule(string property, T value)
            : this(property, value, ComparisonOperator.Equal)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        public CompareRule(string property, T value, ComparisonOperator comparison)
            : base(property)
        {
            ComparisonOperator = comparison;
            ExpectedValue = value;
            ErrorMessage = string.Format(
                Resources.ValidatorCompareMessage,
                property,
                comparison,
                value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        /// <param name="value">The value.</param>
        public CompareRule(string property, string message, T value)
            : this(property, message, value, ComparisonOperator.Equal)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        public CompareRule(string property, string message, T value, ComparisonOperator comparison)
            : base(property, message)
        {
            ComparisonOperator = comparison;
            ExpectedValue = value;
        }

        /// <summary>
        /// Gets or sets the comparison operator.
        /// </summary>
        /// <value>The comparison operator.</value>
        public ComparisonOperator ComparisonOperator { get; private set; }

        /// <summary>
        /// Gets or sets the expected value.
        /// </summary>
        /// <value>The expected value.</value>
        public T ExpectedValue { get; private set; }

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

            int result = comparer.CompareTo(ExpectedValue);
            context.Success = CompareResult(result);
        }

        /// <summary>
        /// Tests the comparison result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private bool CompareResult(int result)
        {
            switch (ComparisonOperator)
            {
                case ComparisonOperator.Equal:
                    return result == 0;
                case ComparisonOperator.NotEqual:
                    return result != 0;
                case ComparisonOperator.GreaterThan:
                    return result > 0;
                case ComparisonOperator.GreaterThanOrEqual:
                    return result >= 0;
                case ComparisonOperator.LessThan:
                    return result < 0;
                case ComparisonOperator.LessThanOrEqual:
                    return result <= 0;
            }
            return false;
        }
    }
}