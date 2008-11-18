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
    /// Assigns the current user's IP address to the property for the specified entity states.
    /// </summary>
    public class IpAddressAttribute : RuleAttributeBase
    {
        public IpAddressAttribute()
        {
        }

        public IpAddressAttribute(EntityState state)
        {
            this.State = state;
        }

        public override IRule CreateRule(string property)
        {
            return new IpAddressRule(property, this.State);
        }

        public override bool IsValid(object value)
        {
            return true;
        }
    }
}
