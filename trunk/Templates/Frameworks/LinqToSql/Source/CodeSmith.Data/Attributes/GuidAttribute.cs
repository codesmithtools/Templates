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
    /// Assigns a new GUID to the property for the specified entity states.
    /// </summary>
    public class GuidAttribute : RuleAttribute
    {
        public GuidAttribute()
        {
        }

        public GuidAttribute(EntityState state)
        {
            this.State = state;
        }

        public override IRule CreateRule(PropertyInfo property)
        {
            return new GuidRule(property.Name, this.State);
        }

        public override bool IsValid(object value)
        {
            return true;
        }
    }
}
