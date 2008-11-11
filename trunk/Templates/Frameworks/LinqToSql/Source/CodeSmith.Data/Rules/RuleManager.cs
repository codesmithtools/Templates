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

        public static void AddShared<EntityType>()
        {
            AddRules<EntityType>(typeof(EntityType).GetProperties());
        }

        public static void AddShared<EntityType>(Type metaData)
        {
            AddRules<EntityType>(metaData.GetProperties());
        }

        private static void  AddRules<EntityType>(PropertyInfo[] properties)
        {
            foreach (PropertyInfo property in properties)
            {
                if (property.GetType() == typeof(DateTime))
                    AddShared<EntityType>(new RangeRule<DateTime>(property.Name,
                        SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value));

                foreach (Attribute attribute in property.GetCustomAttributes(true))
                {
                    if (attribute.GetType() == typeof(StringLengthAttribute))
                        AddShared<EntityType>(new LengthRule(property.Name, ((StringLengthAttribute)attribute).MaximumLength));

                    if (attribute.GetType() == typeof(RequiredAttribute))
                    {
                        if (((RequiredAttribute)attribute).ErrorMessage != null)
                            AddShared<EntityType>(new RequiredRule(property.Name, ((RequiredAttribute)attribute).ErrorMessage));
                        else
                            AddShared<EntityType>(new RequiredRule(property.Name));
                    }

                    if (attribute.GetType() == typeof(RegularExpressionAttribute))
                    {
                        if (((RegularExpressionAttribute)attribute).ErrorMessage != null)
                            AddShared<EntityType>(new RegexRule(property.Name, ((RegularExpressionAttribute)attribute).ErrorMessage, ((RegularExpressionAttribute)attribute).Pattern));
                        else
                            AddShared<EntityType>(new RegexRule(property.Name, ((RegularExpressionAttribute)attribute).Pattern));
                    }

                    if (attribute.GetType() == typeof(NowAttribute))
                    {
                        if (((NowAttribute)attribute).IsStateSet)
                            AddShared<EntityType>(new NowRule(property.Name, ((NowAttribute)attribute).State));
                        else
                            AddShared<EntityType>(new NowRule(property.Name));
                    }

                    if (attribute.GetType() == typeof(RangeAttribute))
                    {
                        bool isInt = property.GetType() == typeof(int) || property.GetType() == typeof(long);
                        bool isFloat = property.GetType() == typeof(float) || property.GetType() == typeof(double) || property.GetType() == typeof(decimal);

                        if (((RangeAttribute)attribute).ErrorMessage != null)
                        {
                            if (isInt)
                                AddShared<EntityType>(new RangeRule<int>(property.Name, ((RangeAttribute)attribute).ErrorMessage,
                                    (int)((RangeAttribute)attribute).Minimum, (int)((RangeAttribute)attribute).Maximum));
                            else if (isFloat)
                                AddShared<EntityType>(new RangeRule<Decimal>(property.Name, ((RangeAttribute)attribute).ErrorMessage,
                                    Convert.ToDecimal(((RangeAttribute)attribute).Minimum), Convert.ToDecimal(((RangeAttribute)attribute).Maximum)));
                            else
                                AddShared<EntityType>(new RangeRule<DateTime>(property.Name, ((RangeAttribute)attribute).ErrorMessage,
                                    (DateTime)((RangeAttribute)attribute).Minimum, (DateTime)((RangeAttribute)attribute).Maximum));
                        }
                        else
                        {
                            if (isInt)
                                AddShared<EntityType>(new RangeRule<int>(property.Name,
                                    (int)((RangeAttribute)attribute).Minimum, (int)((RangeAttribute)attribute).Maximum));
                            else if (isFloat)
                                AddShared<EntityType>(new RangeRule<Decimal>(property.Name,
                                    Convert.ToDecimal(((RangeAttribute)attribute).Minimum), Convert.ToDecimal(((RangeAttribute)attribute).Maximum)));
                            else
                                AddShared<EntityType>(new RangeRule<DateTime>(property.Name,
                                    (DateTime)((RangeAttribute)attribute).Minimum, (DateTime)((RangeAttribute)attribute).Maximum));
                        }
                    }

                    if (attribute.GetType() == typeof(GuidAttribute))
                    {
                        if (((GuidAttribute)attribute).IsStateSet)
                            AddShared<EntityType>(new GuidRule(property.Name, ((GuidAttribute)attribute).State));
                        else
                            AddShared<EntityType>(new GuidRule(property.Name));
                    }

                    if (attribute.GetType() == typeof(UserNameAttribute))
                    {
                        if (((UserNameAttribute)attribute).IsStateSet)
                            AddShared<EntityType>(new UseNamerRule(property.Name, ((UserNameAttribute)attribute).State));
                        else
                            AddShared<EntityType>(new UseNamerRule(property.Name));
                    }

                    if (attribute.GetType() == typeof(IpAddressAttribute))
                    {
                        if (((IpAddressAttribute)attribute).IsStateSet)
                            AddShared<EntityType>(new IpAddressRule(property.Name, ((IpAddressAttribute)attribute).State));
                        else
                            AddShared<EntityType>(new IpAddressRule(property.Name));
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