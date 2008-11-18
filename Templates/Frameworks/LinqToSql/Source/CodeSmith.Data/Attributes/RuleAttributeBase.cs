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
    public abstract class RuleAttributeBase : ValidationAttribute
    {
        protected RuleAttributeBase()
        {
            this.State = EntityState.Dirty;
        }

        public abstract IRule CreateRule(string property);

        public EntityState State { get; protected set; }
    }
}
