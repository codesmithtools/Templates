namespace CodeSmith.Data.Rules.Assign
{
    /// <summary>
    /// Assign a default value to a property when the entity is committed from the <see cref="System.Data.Linq.DataContext"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <example>
    /// <para>Add rule using the rule manager directly.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new DefaultValueRule<int>("Score", 100, EntityState.New));
    /// }
    /// ]]></code>
    /// </example>
    public class DefaultValueRule<T> : PropertyRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValueRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="defaultValue">The default value.</param>
        public DefaultValueRule(string property, T defaultValue)
            : this(property, defaultValue, EntityState.New)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValueRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="assignState">State of the object that can be assigned.</param>
        public DefaultValueRule(string property, T defaultValue, EntityState assignState)
            : base(property, assignState)
        {
            DefaultValue = defaultValue;
            // lower priority because we need to assign before validate
            Priority = 10;
        }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public T DefaultValue { get; private set; }

        /// <summary>
        /// Runs this rule.
        /// </summary>
        /// <param name="context">The rule context.</param>
        public override void Run(RuleContext context)
        {
            context.Message = ErrorMessage;
            context.Success = true;

            if (!GetPropertyInfo(context.TrackedObject.Current).PropertyType.IsAssignableFrom(DefaultValue.GetType()))
                return;

            // Only set if CanRun and if the value has not been manually changed.
            if (CanRun(context.TrackedObject) && !IsPropertyValueModified(context.TrackedObject.Original, context.TrackedObject.Current))
                SetPropertyValue(context.TrackedObject.Current, DefaultValue);
        }
    }
}