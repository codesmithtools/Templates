using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Data.SqlTypes;
using System.Web.DynamicData;
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

        private static void FillAttributes(IDictionary<string, Dictionary<string, Attribute>> dictionary,
            IEnumerable<PropertyInfo> properties)
        {
            foreach (PropertyInfo property in properties)
            {
                if (!dictionary.ContainsKey(property.Name))
                    dictionary.Add(property.Name, new Dictionary<string, Attribute>());

                foreach (Attribute attribute in property.GetCustomAttributes(true))
                {
                    if (attribute.GetType() == typeof(StringLengthAttribute) && !dictionary[property.Name].ContainsKey("StringLength"))
                        dictionary[property.Name]["StringLength"] = attribute;

                    if (attribute.GetType() == typeof(RequiredAttribute) && !dictionary[property.Name].ContainsKey("Required"))
                        dictionary[property.Name]["Required"] = attribute;

                    if (attribute.GetType() == typeof(RegularExpressionAttribute) && !dictionary[property.Name].ContainsKey("RegularExpression"))
                        dictionary[property.Name]["RegularExpression"] = attribute;

                    if (attribute.GetType() == typeof(NowAttribute) && !dictionary[property.Name].ContainsKey("Now"))
                        dictionary[property.Name]["Now"] = attribute;

                    if (attribute.GetType() == typeof(RangeAttribute) && !dictionary[property.Name].ContainsKey("Range"))
                        dictionary[property.Name]["Range"] = attribute;

                    if (attribute.GetType() == typeof(GuidAttribute) && !dictionary[property.Name].ContainsKey("Guid"))
                        dictionary[property.Name]["Guid"] = attribute;

                    if (attribute.GetType() == typeof(UserNameAttribute) && !dictionary[property.Name].ContainsKey("UserName"))
                        dictionary[property.Name]["UserName"] = attribute;

                    if (attribute.GetType() == typeof(IpAddressAttribute) && !dictionary[property.Name].ContainsKey("IpAddress"))
                        dictionary[property.Name]["IpAddress"] = attribute;
                }
            }
        }

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

            PropertyInfo[] properties = typeof(EntityType).GetProperties();
            PropertyInfo[] metadataProperties = metadata == null ? new PropertyInfo[0] : metadata.GetProperties();

            var attributes = new Dictionary<string, Dictionary<string, Attribute>>();
            FillAttributes(attributes, metadataProperties);
            FillAttributes(attributes, properties);

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(DateTime))
                    AddShared<EntityType>(new RangeRule<DateTime>(property.Name,
                        SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value));

                if (attributes[property.Name].ContainsKey("StringLength"))
                    AddShared<EntityType>(new LengthRule(property.Name, ((StringLengthAttribute)attributes[property.Name]["StringLength"]).MaximumLength));

                if (attributes[property.Name].ContainsKey("Required"))
                {
                    RequiredAttribute attribute = (RequiredAttribute)attributes[property.Name]["Required"];
                    if (!string.IsNullOrEmpty(attribute.ErrorMessage))
                        AddShared<EntityType>(new RequiredRule(property.Name, attribute.ErrorMessage));
                    else
                        AddShared<EntityType>(new RequiredRule(property.Name));
                }

                if (attributes[property.Name].ContainsKey("RegularExpression"))
                {
                    RegularExpressionAttribute attribute = (RegularExpressionAttribute)attributes[property.Name]["RegularExpression"];
                    if (!string.IsNullOrEmpty(attribute.ErrorMessage))
                        AddShared<EntityType>(new RegexRule(property.Name, attribute.ErrorMessage, attribute.Pattern));
                    else
                        AddShared<EntityType>(new RegexRule(property.Name, attribute.Pattern));
                }

                if (attributes[property.Name].ContainsKey("Now"))
                {
                    NowAttribute attribute = (NowAttribute)attributes[property.Name]["Now"];
                    if (attribute.IsStateSet)
                        AddShared<EntityType>(new NowRule(property.Name, attribute.State));
                    else
                        AddShared<EntityType>(new NowRule(property.Name));
                }

                if (attributes[property.Name].ContainsKey("Range"))
                {
                    RangeAttribute attribute = (RangeAttribute)attributes[property.Name]["Range"];

                    if (!string.IsNullOrEmpty(attribute.ErrorMessage))
                    {
                        PropertyRuleBase generic = Activator.CreateInstance(typeof(RangeRule<>).MakeGenericType(attribute.OperandType),
                            property.Name, attribute.ErrorMessage,
                            attribute.Minimum, attribute.Maximum) as PropertyRuleBase;

                        AddShared<EntityType>(generic);
                    }
                    else
                    {
                        PropertyRuleBase generic = Activator.CreateInstance(typeof(RangeRule<>).MakeGenericType(attribute.OperandType),
                            property.Name, attribute.Minimum, attribute.Maximum) as PropertyRuleBase;

                        AddShared<EntityType>(generic);
                    }
                }

                if (attributes[property.Name].ContainsKey("Guid"))
                {
                    var guidAttribute = (GuidAttribute)attributes[property.Name]["Guid"];
                    if (guidAttribute.IsStateSet)
                        AddShared<EntityType>(new GuidRule(property.Name, guidAttribute.State));
                    else
                        AddShared<EntityType>(new GuidRule(property.Name));
                }

                if (attributes[property.Name].ContainsKey("UserName"))
                {
                    var userNameAttribute = (UserNameAttribute)attributes[property.Name]["UserName"];
                    if (userNameAttribute.IsStateSet)
                        AddShared<EntityType>(new UseNamerRule(property.Name, userNameAttribute.State));
                    else
                        AddShared<EntityType>(new UseNamerRule(property.Name));
                }

                if (attributes.ContainsKey("IpAddress"))
                {
                    var ipAddressAttribute = (IpAddressAttribute)attributes[property.Name]["IpAddress"];
                    if (ipAddressAttribute.IsStateSet)
                        AddShared<EntityType>(new IpAddressRule(property.Name, ipAddressAttribute.State));
                    else
                        AddShared<EntityType>(new IpAddressRule(property.Name));
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
            List<IRule> rules = new List<IRule>();

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
                        else if (x == null && y != null)
                            return -1;
                        else if (x != null && y == null)
                            return 1;
                        else
                            return x.Priority.CompareTo(y.Priority);
                    }
                );

                foreach (IRule rule in rules)
                {
                    RuleContext context = new RuleContext(o, rule);
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
            List<TrackedObject> tracked = new List<TrackedObject>();
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
            List<TrackedObject> tracked = new List<TrackedObject>();

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