using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace PLINQO.Tracker.Data.Rules.Attributes
{
    public class TaskAssignmentAttribute : RuleAttributeBase
    {
        public TaskAssignmentAttribute()
        {
        }

        public override IRule CreateRule(string property)
        {
            return new TaskAssignmentRule();
        }

        public override bool IsValid(object value)
        {
            return true;
        }
    }
}
