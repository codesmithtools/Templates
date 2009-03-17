using System;
using System.Reflection;

namespace CodeSmith.Data.Rules.Assign
{
    /// <summary>
    /// Assigns the current date time to the property when the entity is committed from the <see cref="System.Data.Linq.DataContext"/>.
    /// </summary>
    /// <example>
    /// <para>Add rule using the rule manager directly.</para>
    /// <code><![CDATA[
    /// static partial void AddSharedRules()
    /// {
    ///     RuleManager.AddShared<User>(new NowRule("CreatedDate", EntityState.New));
    ///     RuleManager.AddShared<User>(new NowRule("ModifiedDate", EntityState.Dirty));
    /// }
    /// ]]></code>
    /// <para>Add rule using the Metadata class and attribute.</para>
    /// <code><![CDATA[
    /// private class Metadata
    /// {
    ///     // fragment of the metadata class
    /// 
    ///     [Now(EntityState.New)]
    ///     public DateTime CreatedDate { get; set; }
    /// 
    ///     [Now(EntityState.Dirty)]
    ///     public DateTime ModifiedDate { get; set; }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="T:CodeSmith.Data.Attributes.NowAttribute"/>
    public class NowRule : PropertyRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NowRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public NowRule(string property)
            : base(property)
        {
            // lower priority because we need to assign before validate
            Priority = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NowRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="assignState">State of the object that can be assigned.</param>
        public NowRule(string property, EntityState assignState)
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
            PropertyInfo property = GetPropertyInfo(current);
            if (property.PropertyType != typeof (DateTime))
                return;

            var value = (DateTime) property.GetValue(current, null);
            if (CanRun(context.TrackedObject) && value == default(DateTime))
                property.SetValue(current, DateTime.Now, null);
        }
    }
}