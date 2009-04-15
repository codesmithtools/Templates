using System;
using System.Reflection;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// A base class for property rules.
    /// </summary>
    public abstract class PropertyRuleBase : IRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRuleBase"/> class.
        /// </summary>
        /// <param name="property">The target property to apply rule to.</param>
        protected PropertyRuleBase(string property)
            : this(property, EntityState.Dirty)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRuleBase"/> class.
        /// </summary>
        /// <param name="property">The target property to apply rule to.</param>
        /// <param name="message">The error message when rule fails.</param>
        protected PropertyRuleBase(string property, string message)
            : this(property, message, EntityState.Dirty)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRuleBase"/> class.
        /// </summary>
        /// <param name="property">The target property to apply rule to.</param>
        /// <param name="applyState">The state of the entity in which to apply rule.</param>
        protected PropertyRuleBase(string property, EntityState applyState)
        {
            TargetProperty = property;
            ApplyState = applyState;
            Priority = 100;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRuleBase"/> class.
        /// </summary>
        /// <param name="property">The target property.</param>
        /// <param name="message">The error message when rule fails.</param>
        /// <param name="applyState">The state of the entity in which to apply rule.</param>
        protected PropertyRuleBase(string property, string message, EntityState applyState)
        {
            TargetProperty = property;
            ErrorMessage = message;
            ApplyState = applyState;
            Priority = 100;
        }

        /// <summary>
        /// Gets the state of the entity in which to apply rule.
        /// </summary>
        /// <value>The state of the apply.</value>
        public EntityState ApplyState { get; private set; }

        #region IRule Members

        /// <summary>
        /// Gets the target property to apply rule to.
        /// </summary>
        /// <value>The target property.</value>
        public string TargetProperty { get; protected set; }

        /// <summary>
        /// Gets the error message when rule fails.
        /// </summary>
        /// <value>The error message when rule fails.</value>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// Gets the rule priority. The lowest number runs first.
        /// </summary>
        /// <value>The rule priority.</value>
        public int Priority { get; set; }

        /// <summary>
        /// Runs the specified rule using the <see cref="RuleContext"/>.
        /// </summary>
        /// <param name="context">The current <see cref="RuleContext"/>.</param>
        public abstract void Run(RuleContext context);

        #endregion

        /// <summary>
        /// Determines whether this instance can run the specified rule based on the <see cref="TrackedObject"/>.
        /// </summary>
        /// <param name="trackedObject">The tracked object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can run the rule; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanRun(TrackedObject trackedObject)
        {
            if (trackedObject.IsNew && (ApplyState & EntityState.New) == EntityState.New)
                return true;
            if (trackedObject.IsChanged && (ApplyState & EntityState.Changed) == EntityState.Changed)
                return true;
            if (trackedObject.IsDeleted && (ApplyState & EntityState.Deleted) == EntityState.Deleted)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> for the <see cref="TargetProperty"/>.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns></returns>
        protected PropertyInfo GetPropertyInfo(object target)
        {
            return target.GetType().GetProperty(TargetProperty);
        }

        /// <summary>
        /// Gets the property value for the <see cref="TargetProperty"/>.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>The property value.</returns>
        protected object GetPropertyValue(object target)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(target);
            if (propertyInfo == null)
                return null;

            return propertyInfo.GetValue(target, null);
        }

        /// <summary>
        /// Gets the property value for the <see cref="TargetProperty"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="target">The target object.</param>
        /// <returns>The property value.</returns>
        protected T GetPropertyValue<T>(object target)
        {
            object value = GetPropertyValue(target);
            return (value == null) ? default(T) : (T) value;
        }

        /// <summary>
        /// Sets the property value for the <see cref="TargetProperty"/>.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="value">The value to set.</param>
        protected void SetPropertyValue(object target, object value)
        {
            if (target == null)
                throw new ArgumentNullException("target", "Target object can not be Null.");

            PropertyInfo propertyInfo = GetPropertyInfo(target);
            if (propertyInfo == null)
                return;

            if (value == null)
                propertyInfo.SetValue(target, value, null);
            else
            {
                Type pType = GetUnderlyingType(propertyInfo.PropertyType);
                Type vType = GetUnderlyingType(value.GetType());

                if (pType.Equals(vType))
                    // types match, just copy value
                    propertyInfo.SetValue(target, value, null);
                else
                    // types don't match, try to coerce
                    if (pType.Equals(typeof (Guid)))
                        propertyInfo.SetValue(target, new Guid(value.ToString()), null);
                    else if (pType.IsEnum && vType.Equals(typeof (string)))
                        propertyInfo.SetValue(target, Enum.Parse(pType, value.ToString()), null);
                    else
                        propertyInfo.SetValue(target, Convert.ChangeType(value, pType), null);
            }
        }

        protected bool IsPropertyValueModified(object original, object current)
        {
            object currentValue = GetPropertyValue(current);
            object originalValue;

            originalValue = original != null ? GetPropertyValue(original) : Activator.CreateInstance(GetPropertyInfo(current).PropertyType);

            return !currentValue.Equals(originalValue);
        }

        private static Type GetUnderlyingType(Type propertyType)
        {
            Type type = propertyType;
            bool isNullable = type.IsGenericType && (type.GetGenericTypeDefinition() == typeof (Nullable<>));
            if (isNullable)
                return Nullable.GetUnderlyingType(type);
            return type;
        }
    }
}