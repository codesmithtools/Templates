using System;
using System.ComponentModel;
using System.Data.Linq;
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
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRuleBase"/> class.
        /// </summary>
        /// <param name="property">The target property to apply rule to.</param>
        /// <param name="message">The error message when rule fails.</param>
        protected PropertyRuleBase(string property, string message)
            : this(property, message, EntityState.Dirty)
        { }

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
            if (target == null)
                return null;

            return target.GetType().GetProperty(TargetProperty);
        }

        /// <summary>
        /// Gets the property value for the <see cref="TargetProperty"/>.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>The property value.</returns>
        protected object GetPropertyValue(object target)
        {
            if (target == null)
                return null;

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

            return (value == null) ? default(T) : (T)CoerceValue(typeof(T), value);
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
                    if (pType.Equals(typeof(Guid)))
                        propertyInfo.SetValue(target, new Guid(value.ToString()), null);
                    else if (pType.IsEnum && vType.Equals(typeof(string)))
                        propertyInfo.SetValue(target, Enum.Parse(pType, value.ToString()), null);
                    else
                        propertyInfo.SetValue(target, Convert.ChangeType(value, pType), null);
            }
        }

        /// <summary>
        /// Determines whether the property value is modified.
        /// </summary>
        /// <param name="original">The original entity.</param>
        /// <param name="current">The current entity.</param>
        /// <returns>
        /// 	<c>true</c> if the property value is modified; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsPropertyValueModified(object original, object current)
        {
            if (current == null)
                return false;

            object currentValue = GetPropertyValue(current);

            if (original == null)
            {
                Type type = current.GetType();
                original = Activator.CreateInstance(type);
            }

            object originalValue = GetPropertyValue(original);
            return !Equals(currentValue, originalValue);
        }

        private static Type GetUnderlyingType(Type propertyType)
        {
            Type type = propertyType;
            bool isNullable = type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
            if (isNullable)
                return Nullable.GetUnderlyingType(type);
            return type;
        }

        private static readonly Type StringType = typeof(string);
        private static readonly Type ByteArrayType = typeof(byte[]);
        private static readonly Type NullableType = typeof(Nullable<>);
        private static readonly Type BinaryType = typeof(Binary);

        /// <summary>
        /// Attempts to coerce a value of one type into a value of a different type.
        /// </summary>
        /// <param name="desiredType">Type to which the value should be coerced.</param>
        /// <param name="valueType">Original type of the value.</param>
        /// <param name="value">The value to coerce.</param>
        public static object CoerceValue(Type desiredType, object value)
        {
            if (value == null)
                return null;

            return CoerceValue(desiredType, value.GetType(), value);
        }

        /// <summary>
        /// Attempts to coerce a value of one type into a value of a different type.
        /// </summary>
        /// <param name="desiredType">Type to which the value should be coerced.</param>
        /// <param name="valueType">Original type of the value.</param>
        /// <param name="value">The value to coerce.</param>
        public static object CoerceValue(Type desiredType, Type valueType, object value)
        {
            // types match, just copy value
            if (desiredType.Equals(valueType))
                return value;

            bool isNullable = desiredType.IsGenericType && (desiredType.GetGenericTypeDefinition() == NullableType);
            if (isNullable)
            {
                if (value == null)
                    return null;
                if (valueType.Equals(StringType) && Convert.ToString(value) == string.Empty)
                    return null;
            }

            desiredType = GetUnderlyingType(desiredType);

            if ((desiredType.IsPrimitive || desiredType.Equals(typeof(decimal)))
                && valueType.Equals(StringType)
                && string.IsNullOrEmpty((string)value))
                return 0;

            if (value == null)
                return null;

            // types don't match, try to convert
            if (desiredType.Equals(typeof(Guid)))
                return new Guid(value.ToString());

            if (desiredType.IsEnum && valueType.Equals(StringType))
                return Enum.Parse(desiredType, value.ToString());

            bool isBinary = (desiredType.IsArray && desiredType.Equals(ByteArrayType)) || desiredType.Equals(BinaryType);

            if (isBinary && valueType.Equals(StringType))
            {
                byte[] bytes = Convert.FromBase64String((string)value);
                if (desiredType.IsArray && desiredType.Equals(ByteArrayType))
                    return bytes;

                return new Binary(bytes);
            }

            isBinary = (valueType.IsArray && valueType.Equals(ByteArrayType)) || valueType.Equals(BinaryType);

            if (isBinary && desiredType.Equals(StringType))
            {
                byte[] bytes = (value is Binary) ? ((Binary)value).ToArray() : (byte[])value;
                return Convert.ToBase64String(bytes);
            }

            try
            {
                if (desiredType.Equals(StringType))
                    return value.ToString();

                return Convert.ChangeType(value, desiredType);
            }
            catch
            {
                TypeConverter converter = TypeDescriptor.GetConverter(desiredType);
                if (converter != null && converter.CanConvertFrom(valueType))
                    return converter.ConvertFrom(value);

                throw;
            }
        }
    }
}