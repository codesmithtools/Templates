using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;
using CodeSmith.Data.Rules.Assign;

namespace PLINQO.Tracker.Data.Rules
{
    public class CurrentUserNameAttribute : RuleAttributeBase
    {
        public CurrentUserNameAttribute()
        {}

        public CurrentUserNameAttribute(EntityState state)
        {
            State = state;
        }

        public override IRule CreateRule(string property)
        {
            return new CurrentUserNameRule(property, State);
        }

        public override bool IsValid(object value)
        {
            return true;
        }
    }
}
