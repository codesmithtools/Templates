using CodeSmith.Data.Rules;
using CodeSmith.Data.Rules.Assign;

namespace CodeSmith.Data.Attributes
{
    /// <summary>
    /// Assigns the current users name to the property for the specified entity states.
    /// </summary>
    /// <example>
    /// <para>Add rule using the Metadata class and attribute.</para>
    /// <code><![CDATA[
    /// private class Metadata
    /// {
    ///     // fragment of the metadata class
    /// 
    ///     [UserName(EntityState.New)]
    ///     public string CreatedBy { get; set; }
    /// 
    ///     [UserName(EntityState.Dirty)]
    ///     public string ModifiedBy { get; set; }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="T:CodeSmith.Data.Rules.Assign.UserNameRule"/>
    public class UserNameAttribute : RuleAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNameAttribute"/> class.
        /// </summary>
        public UserNameAttribute()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNameAttribute"/> class.
        /// </summary>
        /// <param name="state">State of the object that can be assigned.</param>
        public UserNameAttribute(EntityState state)
        {
            State = state;
        }

        /// <summary>
        /// Creates the rule.
        /// </summary>
        /// <param name="property">The property name this rule applies to.</param>
        /// <returns>A new instance of the rule.</returns>
        public override IRule CreateRule(string property)
        {
            return new UserNameRule(property, State);
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the specified validation object on which the <see cref="T:System.ComponentModel.DataAnnotations.ValidationAttribute"/> is declared.</param>
        /// <returns>
        /// true if the specified value is valid; otherwise, false.
        /// </returns>
        public override bool IsValid(object value)
        {
            return true;
        }
    }
}