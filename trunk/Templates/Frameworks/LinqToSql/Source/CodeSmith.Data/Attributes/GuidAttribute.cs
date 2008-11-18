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
    public class GuidAttribute : RuleAttributeBase
    {
        public GuidAttribute()
        {
        }

        public GuidAttribute(EntityState state)
        {
            this.State = state;
        }

        public override IRule CreateRule(string property)
        {
            return new GuidRule(property, this.State);
        }

        public override bool IsValid(object value)
        {
            return true;
        }
    }
}
