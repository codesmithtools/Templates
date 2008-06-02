using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CodeSmith.Data.Rules.Assign
{
    /// <summary>
    /// Assigns the current date time to the property.
    /// </summary>
    public class NowRule : PropertyRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNowRule"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public NowRule(string property)
            : base(property)
        {
            // lower priority because we need to assign before validate
            Priority = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNowRule"/> class.
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
            if (property.PropertyType != typeof(DateTime))
                return;

            DateTime value = (DateTime)property.GetValue(current, null);
            if (CanRun(context.TrackedObject) || value == default(DateTime))
                property.SetValue(current, DateTime.Now, null);
        }
    }
}
