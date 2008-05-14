using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSmith.Data.Rules.Validation
{
    /// <summary>
    /// The operator the Compare Validator uses.
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        /// A comparison for equality.  
        /// </summary>
        Equal,
        /// <summary>
        /// A comparison for greater than.  
        /// </summary>
        GreaterThan,
        /// <summary>
        /// A comparison for greater than or equal to. 
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// A comparison for less than.  
        /// </summary>
        LessThan,
        /// <summary>
        /// A comparison for less than or equal to.  
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// A comparison for inequality.  
        /// </summary>
        NotEqual
    }
}
