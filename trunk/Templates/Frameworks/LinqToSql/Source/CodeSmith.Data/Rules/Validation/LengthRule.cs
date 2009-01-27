using System;
using System.Collections;
using CodeSmith.Data.Properties;

namespace CodeSmith.Data.Rules.Validation
{
    /// <summary>
    /// A rule to check the length.
    /// </summary>
    /// <example>
    /// <para>Add rule using the rule manager directly.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new LengthRule("UserName", 100));
    /// }
    /// ]]></code>
    /// <para>Add rule using the Metadata class and attribute.</para>
    /// <code><![CDATA[
    /// private class Metadata
    /// {
    ///     // fragment of the metadata class
    /// 
    ///     [StringLength(100)]            
    ///     public string UserName { get; set; }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="T:System.ComponentModel.DataAnnotations.StringLengthAttribute"/>
    public class LengthRule : PropertyRuleBase
    {
        private const int MinDefault = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="maxLength">The maximum length.</param>
        public LengthRule(string property, int maxLength)
            : this(property, MinDefault, maxLength)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        public LengthRule(string property, int minLength, int maxLength)
            : base(property)
        {
            MaxLength = maxLength;
            MinLength = minLength;
            ErrorMessage = string.Format(
                Resources.ValidatorLengthMessage,
                property,
                minLength,
                maxLength);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        /// <param name="maxLength">The maximum length.</param>
        public LengthRule(string property, string message, int maxLength)
            : this(property, message, MinDefault, maxLength)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        public LengthRule(string property, string message, int minLength, int maxLength)
            : base(property, message)
        {
            MaxLength = maxLength;
            MinLength = minLength;
        }

        /// <summary>
        /// Gets or sets the max length.
        /// </summary>
        /// <value>The maximum length.</value>
        public int MaxLength { get; private set; }

        /// <summary>
        /// Gets or sets the minimum length.
        /// </summary>
        /// <value>The minimum length.</value>
        public int MinLength { get; private set; }

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

            object value = GetPropertyValue(context.TrackedObject.Current);

            if (value == null)
            {
                context.Success = MinLength == MinDefault;
                return;
            }

            var c = value as ICollection;
            if (c != null)
            {
                context.Success = IsValidLength(c.Count);
                return;
            }

            var a = value as Array;
            if (a != null)
            {
                context.Success = IsValidLength(a.Length);
                return;
            }

            var s = value as string;
            if (s != null)
            {
                context.Success = IsValidLength(s.Length);
                return;
            }

            context.Success = false;
        }

        private bool IsValidLength(int length)
        {
            return MinLength <= length && length <= MaxLength;
        }
    }
}