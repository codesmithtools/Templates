using System;

namespace CodeSmith.Data.Rules.Assign
{
    /// <summary>
    /// Assigns a new Guid to a property when the entity is committed from the <see cref="System.Data.Linq.DataContext"/>.
    /// </summary>
    /// <example>
    /// <para>Add rule using the rule manager directly.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new GuidRule("UserId", EntityState.New));
    /// }
    /// ]]></code>
    /// <para>Add rule using the Metadata class and attribute.</para>
    /// <code><![CDATA[
    /// private class Metadata
    /// {
    ///     // fragment of the metadata class
    /// 
    ///     [Guid(EntityState.New)]
    ///     public Guid UserId { get; set; }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="T:CodeSmith.Data.Attributes.GuidAttribute"/>
    public class GuidRule : PropertyRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public GuidRule(string property)
            : base(property)
        {
            // lower priority because we need to assign before validate
            Priority = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="assignState">State of the object that can be assigned.</param>
        public GuidRule(string property, EntityState assignState)
            : base(property, assignState)
        {
            // lower priority because we need to assign before validate
            Priority = 10;
        }

        /// <summary>
        /// Runs this rule.
        /// </summary>
        /// <param name="context">The rule context.</param>
        public override void Run(RuleContext context)
        {
            context.Message = ErrorMessage;
            context.Success = true;

            object current = context.TrackedObject.Current;
            var value = GetPropertyValue<Guid>(current);

            if (CanRun(context.TrackedObject) || value == default(Guid))
                SetPropertyValue(current, Guid.NewGuid());
        }
    }
}