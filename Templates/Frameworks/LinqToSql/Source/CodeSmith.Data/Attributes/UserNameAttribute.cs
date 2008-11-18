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
    /// Assigns the current users name to the property for the specified entity states.
    /// </summary>
    public class UserNameAttribute : RuleAttributeBase
    {
        public UserNameAttribute()
        {
        }

        public UserNameAttribute(EntityState state)
        {
            this.State = state;
        }

        public override IRule CreateRule(string property)
        {
            return new UserNameRule(property, this.State);
        }

        public override bool IsValid(object value)
        {
            return true;
        }
    }
}
