using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Linq;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules.Validation;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// A class to manage rules.
    /// </summary>
    public class RuleManager
    {
        private static readonly RuleCollection _sharedBusinessRules;
        private readonly RuleCollection _businessRules;

        /// <summary>
        /// Initializes the <see cref="RuleManager"/> class.
        /// </summary>
        static RuleManager()
        {
            _sharedBusinessRules = new RuleCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleManager"/> class.
        /// </summary>
        public RuleManager()
        {
            BrokenRules = new BrokenRuleCollection();
            _businessRules = new RuleCollection();
        }

        /// <summary>
        /// Gets the broken rules.
        /// </summary>
        /// <value>The broken rules.</value>
        public BrokenRuleCollection BrokenRules { get; private set; }

        /// <summary>
        /// Adds the specified rule.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="rule">The rule to add.</param>
        public void Add<TEntity>(IRule rule)
        {
            if (_businessRules.ContainsKey(typeof(TEntity)))
                _businessRules[typeof(TEntity)].Add(rule);
            else
                _businessRules.TryAdd(typeof(TEntity), new RuleList { rule });
        }

        /// <summary>
        /// Adds the shared global rules.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="rule">The rule to add.</param>
        public static void AddShared<TEntity>(IRule rule)
        {
            AddShared(rule, typeof(TEntity));
        }

        /// <summary>
        /// Adds the shared global rules.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        /// <param name="entityType">The type of the entity</param>
        private static void AddShared(IRule rule, Type entityType)
        {
            if (!_sharedBusinessRules.ContainsKey(entityType))
                _sharedBusinessRules.TryAdd(entityType, new RuleList());

            _sharedBusinessRules[entityType].Add(rule);
        }

        /// <summary>
        /// Adds rules to the rule manager from any property attributes on the specified type. 
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        public static void AddShared<TEntity>()
        {
            AddShared(typeof(TEntity));
        }

        private static void AddShared(Type entityType)
        {
            Type metadata = null;

            if (!_sharedBusinessRules.ContainsKey(entityType))
                _sharedBusinessRules.TryAdd(entityType, new RuleList());

            //don't do anything if the properties have already been processed.
            if (_sharedBusinessRules[entityType].IsProcessed)
                return;

            _sharedBusinessRules[entityType].IsProcessed = true;

            var metadataTypeAttribute = entityType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault() as MetadataTypeAttribute;
            if (metadataTypeAttribute != null)
                metadata = metadataTypeAttribute.MetadataClassType;

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            PropertyDescriptorCollection metadataProperties = null;
            if (metadata != null)
                metadataProperties = TypeDescriptor.GetProperties(metadata);

            foreach (PropertyDescriptor property in properties)
            {
                IList<Attribute> attributes = GetAttributes(property, metadataProperties);

                foreach (Attribute attribute in attributes)
                    if (attribute is RuleAttributeBase)
                    {
                        var ruleAttribute = (RuleAttributeBase)attribute;
                        AddShared(ruleAttribute.CreateRule(property.Name), entityType);
                    }
                    else if (attribute is ValidationAttribute)
                        AddValidation(property.Name, attribute, entityType);
            }
        }

        private static void AddValidation<TEntity>(string property, Attribute attribute)
        {
            AddValidation(property, attribute, typeof(TEntity));
        }

        private static void AddValidation(string property, Attribute attribute, Type entityType)
        {
            if (attribute is StringLengthAttribute)
            {
                var validationAttribute = (StringLengthAttribute)attribute;
                if (string.IsNullOrEmpty(validationAttribute.ErrorMessage))
                    AddShared(new LengthRule(property, validationAttribute.MaximumLength), entityType);
                else
                    AddShared(new LengthRule(property, validationAttribute.ErrorMessage,
                                                      validationAttribute.MaximumLength), entityType);
            }
            else if (attribute is RequiredAttribute)
            {
                var validationAttribute = (RequiredAttribute)attribute;
                if (string.IsNullOrEmpty(validationAttribute.ErrorMessage))
                    AddShared(new RequiredRule(property), entityType);
                else
                    AddShared(new RequiredRule(property, validationAttribute.ErrorMessage), entityType);
            }
            else if (attribute is RegularExpressionAttribute)
            {
                var validationAttribute = (RegularExpressionAttribute)attribute;
                if (string.IsNullOrEmpty(validationAttribute.ErrorMessage))
                    AddShared(new RegexRule(property, validationAttribute.Pattern), entityType);
                else
                    AddShared(new RegexRule(property, validationAttribute.ErrorMessage,
                                                     validationAttribute.Pattern), entityType);
            }
            else if (attribute is RangeAttribute)
            {
                var validationAttribute = (RangeAttribute)attribute;
                if (string.IsNullOrEmpty(validationAttribute.ErrorMessage))
                {
                    var rangeRule = Activator.CreateInstance(typeof(RangeRule<>).MakeGenericType(validationAttribute.OperandType),
                                                 property, validationAttribute.Minimum, validationAttribute.Maximum) as PropertyRuleBase;
                    AddShared(rangeRule, entityType);
                }

                else
                {
                    var rangeRule = Activator.CreateInstance(typeof(RangeRule<>).MakeGenericType(validationAttribute.OperandType),
                                                 property, validationAttribute.ErrorMessage, validationAttribute.Minimum, validationAttribute.Maximum) as PropertyRuleBase;
                    AddShared(rangeRule, entityType);
                }
            }
        }

        private static IList<Attribute> GetAttributes(MemberDescriptor property, PropertyDescriptorCollection metadataProperties)
        {
            // 1) only looking for ValidationAttribute attributes
            // 2) only one rule per attribute type can exist, last in wins.

            var attributes = new Dictionary<Type, Attribute>();
            foreach (Attribute attribute in property.Attributes)
                if (attribute is ValidationAttribute)
                    attributes[attribute.GetType()] = attribute;

            if (metadataProperties == null)
                return attributes.Values.ToList();

            PropertyDescriptor metadataProperty = metadataProperties.Find(property.Name, false);
            if (metadataProperty == null)
                return attributes.Values.ToList();

            foreach (Attribute attribute in metadataProperty.Attributes)
                if (attribute is ValidationAttribute)
                    attributes[attribute.GetType()] = attribute; // overwrite previous type

            return attributes.Values.ToList();
        }

        /// <summary>
        /// Gets the rules for a type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A collection of rules.</returns>
        public List<IRule> GetRules<TEntity>()
        {
            return GetRules(typeof(TEntity));
        }

        /// <summary>
        /// Gets the rules for a type.
        /// </summary>
        /// <param name="type">The type of the entity.</param>
        /// <returns>A collection of rules.</returns>
        public List<IRule> GetRules(Type type)
        {
            var rules = new List<IRule>();

            //ensure the properties were processed.
            AddShared(type);

            if (_sharedBusinessRules.ContainsKey(type))
                rules.AddRange(_sharedBusinessRules[type]);
            if (_businessRules != null && _businessRules.ContainsKey(type))
                rules.AddRange(_businessRules[type]);

            return rules;
        }


        /// <summary>
        /// Run the rules for the specified <see cref="TrackedObject"/> list.
        /// </summary>
        /// <param name="objects">The <see cref="TrackedObject"/> to run rules on.</param>
        /// <returns><c>true</c> if rules ran successfully; otherwise, <c>false</c>.</returns>
        public bool Run(IEnumerable<TrackedObject> objects)
        {
            BrokenRules.Clear();
            bool isSuccess = true;

            foreach (TrackedObject o in objects)
                isSuccess &= Run(o);

            return isSuccess;
        }

        /// <summary>
        /// Run the rules for the specified <see cref="TrackedObject"/>.
        /// </summary>
        /// <param name="trackedObject">The <see cref="TrackedObject"/> to run rules on.</param>
        /// <returns><c>true</c> if rules ran successfully; otherwise, <c>false</c>.</returns>
        public bool Run(TrackedObject trackedObject)
        {
            bool isSuccess = true;

            List<IRule> rules = GetRules(trackedObject.Current.GetType());
            rules.Sort(
                delegate(IRule x, IRule y)
                {
                    if (x == null && y == null)
                        return 0;

                    if (x == null)
                        return -1;

                    if (y == null)
                        return 1;

                    return x.Priority.CompareTo(y.Priority);
                });

            foreach (IRule rule in rules)
            {
                var context = new RuleContext(trackedObject, rule);
                rule.Run(context);
                isSuccess &= context.Success;
                if (!context.Success)
                    BrokenRules.Add(new BrokenRule(context));
            }

            return isSuccess;
        }

        /// <summary>
        /// Run the rules for the specified entity.
        /// </summary>
        /// <param name="entity">The entity to run validation for.</param>
        /// <returns><c>true</c> if rules ran successfully; otherwise, <c>false</c>.</returns>
        public bool Run(object entity)
        {
            return Run(entity, true);
        }

        /// <summary>
        /// Run the rules for the specified entity.
        /// </summary>
        /// <param name="entity">The entity to run validation for.</param>
        /// <param name="modified">if set to <c>true</c> [modified].</param>
        /// <returns>
        /// 	<c>true</c> if rules ran successfully; otherwise, <c>false</c>.
        /// </returns>
        public bool Run(object entity, bool modified)
        {
            TrackedObject trackedObject = new TrackedObject();

            trackedObject.Current = entity;
            trackedObject.IsNew = !modified;
            trackedObject.IsDeleted = false;
            trackedObject.IsChanged = modified;

            return Run(trackedObject);
        }

        /// <summary>
        /// Run the rules for the the changed objects in the <see cref="DataContext"/>.
        /// </summary>
        /// <param name="dataContext">The <see cref="DataContext"/> to get the <see cref="ChangeSet"/> from and run rules against.</param>
        /// <returns><c>true</c> if rules ran successfully; otherwise, <c>false</c>.</returns>
        public bool Run(DataContext dataContext)
        {
            ChangeSet changeSet = dataContext.GetChangeSet();

            var tracked = new List<TrackedObject>();

            foreach (object o in changeSet.Inserts)
                tracked.Add(new TrackedObject { Current = o, IsNew = true });

            foreach (object o in changeSet.Deletes)
                tracked.Add(new TrackedObject { Current = o, IsDeleted = true });

            foreach (object o in changeSet.Updates)
            {
                var trackedObject = new TrackedObject { Current = o, IsChanged = true };

                if (o != null)
                {
                    var metaType = dataContext.Mapping.GetMetaType(o.GetType());
                    var type = metaType.InheritanceRoot == null
                        ? metaType.Type
                        : metaType.InheritanceRoot.Type;

                    ITable table = dataContext.GetTable(type);
                    trackedObject.Original = table.GetOriginalEntityState(o);
                }

                tracked.Add(trackedObject);
            }


            return Run(tracked);
        }
    }
}