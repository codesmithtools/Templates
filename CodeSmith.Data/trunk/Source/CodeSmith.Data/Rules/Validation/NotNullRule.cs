using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Properties;

namespace CodeSmith.Data.Rules.Validation
{
    public class NotNullRule : PropertyRuleBase
    {
        public NotNullRule(string property)
            : base(property, string.Format(Resources.ValidatorNotNullMessage, property))
        { }

        public NotNullRule(string property, string message)
            : base(property, message)
        { }

        public NotNullRule(string property, EntityState applyState)
            : base(property, string.Format(Resources.ValidatorNotNullMessage, property), applyState)
        { }

        public NotNullRule(string property, string message, EntityState applyState)
            : base(property, message, applyState)
        { }

        public override void Run(RuleContext context)
        {
            context.Message = ErrorMessage;
            context.Success = true;

            if (!CanRun(context.TrackedObject))
                return;

            object value = GetPropertyValue(context.TrackedObject.Current);
            context.Success = value != null;
        }
    }
}
