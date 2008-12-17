using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// An exception class for broken rules.
    /// </summary>
    [Serializable]
    public class BrokenRuleException : ValidationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokenRuleException"/> class.
        /// </summary>
        /// <param name="brokenRules">The broken rules.</param>
        public BrokenRuleException(BrokenRuleCollection brokenRules) : base()
        {
            this.BrokenRules = brokenRules;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokenRuleException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected BrokenRuleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Gets the broken rules.
        /// </summary>
        /// <value>The broken rules.</value>
        public BrokenRuleCollection BrokenRules { get; private set; }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <value></value>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        public override string Message
        {
            get
            {
                return ToString();
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CodeSmith.Data.Rules.BrokenRuleException"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(BrokenRuleException ex)
        {
            return ex.ToString();
        }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>
        /// A string representation of the current exception.
        /// </returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
        /// </PermissionSet>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (this.BrokenRules.Count == 1)
                sb.Append("1 rule has been broken.  All rules must pass.");
            else
                sb.AppendFormat("{0} rules have been broken.  All rules must pass.", this.BrokenRules.Count);

            sb.AppendLine();

            foreach (BrokenRule rule in this.BrokenRules)
            {
                sb.AppendFormat("  - {0}", rule.Message);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
