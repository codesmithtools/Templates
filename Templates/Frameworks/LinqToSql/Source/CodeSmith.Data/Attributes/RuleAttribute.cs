using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeSmith.Data.Rules;

namespace CodeSmith.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class RuleAttribute : ValidationAttribute
    {
        protected RuleAttribute()
        {
            this.State = EntityState.All;
        }
        
        public abstract IRule CreateRule(PropertyInfo property);

        public EntityState State { get; protected set; }
    }
}
