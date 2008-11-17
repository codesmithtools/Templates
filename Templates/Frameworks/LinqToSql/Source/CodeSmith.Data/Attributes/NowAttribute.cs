using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Rules;
using CodeSmith.Data.Rules.Assign;

namespace CodeSmith.Data.Attributes
{
    /// <summary>
    /// Assigns the current date / time to the property for the specified entity states.
    /// </summary>
    public class NowAttribute : RuleAttribute
    {
        public NowAttribute()
        {
        }

        public NowAttribute(EntityState state)
        {
            this.State = state;
        }

        public override IRule CreateRule(PropertyInfo property)
        {
            return new NowRule(property.Name, this.State);
        }

        public override bool IsValid(object value)
        {
            return true;
        }
    }
}
