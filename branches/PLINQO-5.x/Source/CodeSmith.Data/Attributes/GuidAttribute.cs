using CodeSmith.Data.Rules;
using CodeSmith.Data.Rules.Assign;

namespace CodeSmith.Data.Attributes
{
    /// <summary>
    /// Assigns a new GUID to the property for the specified entity states.
    /// </summary>
    /// <example>
    /// <para>Add rule using the Metadata class and attribute.</para>
    /// <code><![CDATA[
    /// private class Metadata
    /// {
    ///     // fragment of the metadata class
    /// 
    ///     [Guid(EntityState.New)]
    ///     public Guid UserId { get; set; }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="T:CodeSmith.Data.Rules.Assign.GuidRule"/>
    public class GuidAttribute : RuleAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidAttribute"/> class.
        /// </summary>
        public GuidAttribute()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidAttribute"/> class.
        /// </summary>
        /// <param name="state">State of the object that can be assigned.</param>
        public GuidAttribute(EntityState state)
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
            return new GuidRule(property, State);
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