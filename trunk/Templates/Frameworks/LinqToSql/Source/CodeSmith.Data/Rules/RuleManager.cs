using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Data.SqlTypes;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules.Assign;
using CodeSmith.Data.Rules.Validation;
using System.Reflection;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// A class to manage rules.
    /// </summary>
    public class RuleManager
    {
        private RuleCollection _businessRules;
        private static RuleCollection _sharedBusinessRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleManager"/> class.
        /// </summary>
        public RuleManager()
        {
            BrokenRules = new BrokenRuleCollection();
        }

        /// <summary>
        /// Gets the broken rules.
        /// </summary>
        /// <value>The broken rules.</value>
        public BrokenRuleCollection BrokenRules { get; private set; }

        /// <summary>
        /// Adds the specified rule.
        /// </summary>
        /// <typeparam name="EntityType">The type of the entity.</typeparam>
        /// <param name="rule">The rule.</param>
        public void Add<EntityType>(IRule rule)
        {
            if (_businessRules == null)
                _businessRules = new RuleCollection();

            if (_businessRules.ContainsKey(typeof(EntityType)))
                _businessRules[typeof(EntityType)].Add(rule);
            else
                _businessRules.Add(typeof(EntityType), new List<IRule> { rule });
        }

        /// <summary>
        /// Adds the shared global rules.
        /// </summary>
        /// <typeparam name="EntityType">The type of the entity.</typeparam>
        /// <param name="rule">The rule.</param>
        public static void AddShared<EntityType>(IRule rule)
        {
            if (_sharedBusinessRules == null)
                _sharedBusinessRules = new RuleCollection();

            if (_sharedBusinessRules.ContainsKey(typeof(EntityType)))
                _sharedBusinessRules[typeof(EntityType)].Add(rule);
            else
                _sharedBusinessRules.Add(typeof(EntityType), new List<IRule> { rule });
        }

        /// <summary>
        /// Adds rules to the rule manager from any property attributes on the specified type. 
        /// </summary>
        /// <typeparam name="EntityType">The type of the entity.</typeparam>
        public static void AddShared<EntityType>()
        {
            Type metadata = null;
            foreach (Attribute attribute in typeof(EntityType).GetCustomAttributes(true))
            {
                if (!(attribute is MetadataTypeAttribute))
                    continue;

                metadata = ((MetadataTypeAttribute)attribute).MetadataClassType;
                break;
            }

            PropertyInfo[] properties = typeof(EntityType).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var property in properties)
            {
                PropertyInfo metadataProperty = null;
                if (metadata != null)
                    metadataProperty = metadata.GetProperty(property.Name,
                                                            BindingFlags.Instance | BindingFlags.NonPublic |
                                                            BindingFlags.Public);

                foreach (var attribute in property.GetCustomAttributes(true))
                {
                    object metadataAttribute = null;
                    if (metadataProperty != null)
                        metadataAttribute = metadataProperty.GetCustomAttributes(attribute.GetType(), false);

                    if (attribute is RuleAttributeBase)
                    {
                        var ruleAttribute = metadataAttribute != null ? (RuleAttributeBase)metadataAttribute : (RuleAttributeBase)attribute;
                        AddShared<EntityType>(ruleAttribute.CreateRule(property));
                    }
                    else if (attribute is ValidationAttribute)
                    {
                        if (attribute.GetType() == typeof(StringLengthAttribute))
                        {
                            var validationAttribute = metadataAttribute != null ? (StringLengthAttribute)metadataAttribute : (StringLengthAttribute)attribute;
                            AddShared<EntityType>(new LengthRule(property.Name, validationAttribute.ErrorMessage, validationAttribute.MaximumLength));
                        }
                        else if (attribute.GetType() == typeof(RequiredAttribute))
                        {
                            var validationAttribute = metadataAttribute != null ? (RequiredAttribute)metadataAttribute : (RequiredAttribute)attribute;
                            AddShared<EntityType>(new RequiredRule(property.Name, validationAttribute.ErrorMessage));
                        }
                        else if (attribute.GetType() == typeof(RegularExpressionAttribute))
                        {
                            var validationAttribute = metadataAttribute != null ? (RegularExpressionAttribute)metadataAttribute : (RegularExpressionAttribute)attribute;
                            AddShared<EntityType>(new RegexRule(property.Name, validationAttribute.ErrorMessage, validationAttribute.Pattern));
                        }
                        else if (attribute.GetType() == typeof(RangeAttribute))
                        {
                            var validationAttribute = metadataAttribute != null ? (RangeAttribute)metadataAttribute : (RangeAttribute)attribute;
                            PropertyRuleBase rangeRule = Activator.CreateInstance(typeof(RangeRule<>).MakeGenericType(validationAttribute.OperandType),
                                property.Name, validationAttribute.ErrorMessage,
                                validationAttribute.Minimum, validationAttribute.Maximum) as PropertyRuleBase;

                            AddShared<EntityType>(rangeRule);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the rules for a type.
        /// </summary>
        /// <typeparam name="EntityType">The type of the entity.</typeparam>
        /// <returns>A collection of rules.</returns>
        public List<IRule> GetRules<EntityType>()
        {
            return GetRules(typeof(EntityType));
        }

        /// <summary>
        /// Gets the rules for a type.
        /// </summary>
        /// <param name="type">The type of the entity.</param>
        /// <returns>A collection of rules.</returns>
        public List<IRule> GetRules(Type type)
        {
            var rules = new List<IRule>();

            if (_sharedBusinessRules != null && _sharedBusinessRules.ContainsKey(type))
                rules.AddRange(_sharedBusinessRules[type]);
            if (_businessRules != null && _businessRules.ContainsKey(type))
                rules.AddRange(_businessRules[type]);

            return rules;
        }

        /// <summary>
        /// Run the rules specified objects.
        /// </summary>
        /// <param name="objects">The objects to run rules on.</param>
        /// <returns><c>true</c> if rules ran successfully; otherwise, <c>false</c>.</returns>
        public bool Run(List<TrackedObject> objects)
        {
            BrokenRules.Clear();
            bool isSuccess = true;

            foreach (TrackedObject o in objects)
            {
                List<IRule> rules = GetRules(o.Current.GetType());
                rules.Sort(
                    delegate(IRule x, IRule y)
                    {
                        if (x == null && y == null)
                            return 0;
                        else if (x == null)
                            return -1;
                        else if (y == null)
                            return 1;
                        else
                            return x.Priority.CompareTo(y.Priority);
                    }
                );

                foreach (IRule rule in rules)
                {
                    var context = new RuleContext(o, rule);
                    rule.Run(context);
                    isSuccess &= context.Success;
                    if (!context.Success)
                        BrokenRules.Add(new BrokenRule(context));
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// Run the rules specified objects.
        /// </summary>
        /// <param name="objects">The objects to run rules on.</param>
        /// <returns><c>true</c> if rules ran successfully; otherwise, <c>false</c>.</returns>
        public bool Run(params object[] objects)
        {
            var tracked = new List<TrackedObject>();
            foreach (object o in objects)
                tracked.Add(new TrackedObject { Current = o });

            return Run(tracked);
        }

        /// <summary>
        /// Run the rules specified changed objects.
        /// </summary>
        /// <param name="changedObjects">The changed objects.</param>
        /// <returns><c>true</c> if rules ran successfully; otherwise, <c>false</c>.</returns>
        public bool Run(ChangeSet changedObjects)
        {
            var tracked = new List<TrackedObject>();

            foreach (object o in changedObjects.Inserts)
                tracked.Add(new TrackedObject { Current = o, IsNew = true });
            foreach (object o in changedObjects.Updates)
                tracked.Add(new TrackedObject { Current = o, IsChanged = true });
            foreach (object o in changedObjects.Deletes)
                tracked.Add(new TrackedObject { Current = o, IsDeleted = true });

            return Run(tracked);
        }
    }
}