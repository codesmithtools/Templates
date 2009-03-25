using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Audit
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AuditPropertyFormatAttribute : Attribute
    {
        public AuditPropertyFormatAttribute(Type formatType, string methodName)
        {
            FormatType = formatType;
            MethodName = methodName;
        }

        public Type FormatType { get; private set; }
        public string MethodName { get; private set; }
    }
}
