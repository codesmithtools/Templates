using System;

namespace CodeSmith.Data.Rules.Validation
{
    /// <summary>
    /// A rule that run a method to validate.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <example>
    /// <para>Add rule using the rule manager directly.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new CustomRule<string>("UserName", "UserName must be unique.", User.UniqueUserName));
    ///     RuleManager.AddShared<User>(new CustomRule<RuleContext>("UserName", "UserName must be unique.", User.UniqueUserNameContext));
    /// }
    /// //This is called by the custom rule. The first argument is the property value.
    /// private static bool UniqueUserName(string username)
    /// {
    ///     //check user name is unique, return true when valid.
    ///     return true;
    /// }
    /// //This is called by the custom rule. The first argument is the RuleContext.
    /// private static void UniqueUserNameContext(RuleContext context)
    /// {
    ///     //check user name is unique, return true when valid.
    ///     context.Success = true;
    /// }
    /// ]]></code>
    /// </example>
    /// <remarks>
    /// The custom method should return true when the property is valid.
    /// </remarks>
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
        /// Initializes a new instance of the <see cref="CustomRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        /// <param name="method">The method.</param>
        public CustomRule(string property, string message, Action<RuleContext> method)
            : base(property, message)
        {
            Method = method;
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public object Method { get; private set; }

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

            try
            {
                if (Method is Action<RuleContext>)
                {
                    ((Action<RuleContext>)Method).Invoke(context);
                }
                else
                {
                    var value = GetPropertyValue<T>(context.TrackedObject.Current);
                    context.Success = ((Predicate<T>)Method).Invoke(value);
                }
            }
            catch (Exception ex)
            {
                context.Message = "Custom Rule Error: " + ex.Message;
            }
        }
    }
}