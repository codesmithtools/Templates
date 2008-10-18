using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Data.SqlTypes;
using System.Web.DynamicData;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules.Assign;
using CodeSmith.Data.Rules.Validation;

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

        public static void AddShared<EntityType>(Type context)
        {
            MetaModel model = null;
            try
            {
                model = MetaModel.GetModel(context);
            }
            catch
            {
                model = new MetaModel();
                model.RegisterContext(context);
            }

            foreach (MetaColumn col in model.GetTable(typeof(EntityType)).Columns)
            {
                if (col.MaxLength > 0)
                    AddShared<EntityType>(new LengthRule(col.Name, col.MaxLength));

                if (col.IsRequired)
                {
                    if (col.RequiredErrorMessage != null)
                        AddShared<EntityType>(new RequiredRule(col.Name, col.RequiredErrorMessage));
                    else
                        AddShared<EntityType>(new RequiredRule(col.Name));
                }

                if (col.Attributes[typeof(RegularExpressionAttribute)] != null)
                {
                    RegularExpressionAttribute attribute = ((RegularExpressionAttribute)col.Attributes[typeof(RegularExpressionAttribute)]);
                    if (attribute.ErrorMessage != null)
                        AddShared<EntityType>(new RegexRule(col.Name, attribute.ErrorMessage, attribute.Pattern));
                    else
                        AddShared<EntityType>(new RegexRule(col.Name, attribute.Pattern));
                }

                if (col.Attributes[typeof(NowAttribute)] != null)
                {
                    NowAttribute attribute = ((NowAttribute)col.Attributes[typeof(NowAttribute)]);
                    if (attribute.IsStateSet)
                        AddShared<EntityType>(new NowRule(col.Name, attribute.State));
                    else
                        AddShared<EntityType>(new NowRule(col.Name));
                }
                
                if (col.TypeCode == TypeCode.DateTime)
                    AddShared<EntityType>(new RangeRule<DateTime>(col.Name, 
                        SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value));
                
                if (col.Attributes[typeof(RangeAttribute)] != null)
                {
                    RangeAttribute rangeAttribute = ((RangeAttribute)col.Attributes[typeof(RangeAttribute)]);
                    if (rangeAttribute.ErrorMessage != null)
                    {
                        if (col.IsInteger)
                            AddShared<EntityType>(new RangeRule<int>(col.Name, rangeAttribute.ErrorMessage, 
                                (int)rangeAttribute.Minimum, (int)rangeAttribute.Maximum));
                        else if (col.IsFloatingPoint)
                            AddShared<EntityType>(new RangeRule<Decimal>(col.Name, rangeAttribute.ErrorMessage, 
                                Convert.ToDecimal(rangeAttribute.Minimum), Convert.ToDecimal(rangeAttribute.Maximum)));
                        else
                            AddShared<EntityType>(new RangeRule<DateTime>(col.Name, rangeAttribute.ErrorMessage, 
                                (DateTime)rangeAttribute.Minimum, (DateTime)rangeAttribute.Maximum));
                    }
                    else
                    {
                        if (col.IsInteger)
                            AddShared<EntityType>(new RangeRule<int>(col.Name, 
                                (int)rangeAttribute.Minimum, (int)rangeAttribute.Maximum));
                        else if (col.IsFloatingPoint)
                            AddShared<EntityType>(new RangeRule<Decimal>(col.Name, 
                                Convert.ToDecimal(rangeAttribute.Minimum), Convert.ToDecimal(rangeAttribute.Maximum)));
                        else
                            AddShared<EntityType>(new RangeRule<DateTime>(col.Name, 
                                (DateTime)rangeAttribute.Minimum, (DateTime)rangeAttribute.Maximum));
                    }
                }

                if (col.Attributes[typeof(GuidAttribute)] != null)
                {
                    GuidAttribute attribute = ((GuidAttribute)col.Attributes[typeof(GuidAttribute)]);
                    if (attribute.IsStateSet)
                        AddShared<EntityType>(new GuidRule(col.Name, attribute.State));
                    else
                        AddShared<EntityType>(new GuidRule(col.Name));
                }

                if (col.Attributes[typeof(UserNameAttribute)] != null)
                {
                    UserNameAttribute userName = ((UserNameAttribute)col.Attributes[typeof(UserNameAttribute)]);
                    if (userName.IsStateSet)
                        AddShared<EntityType>(new UseNamerRule(col.Name, userName.State));
                    else
                        AddShared<EntityType>(new UseNamerRule(col.Name));
                }

                if (col.Attributes[typeof(IpAddressAttribute)] != null)
                {
                    IpAddressAttribute address = ((IpAddressAttribute)col.Attributes[typeof(IpAddressAttribute)]);
                    if (address.IsStateSet)
                        AddShared<EntityType>(new IpAddressRule(col.Name, address.State));
                    else
                        AddShared<EntityType>(new IpAddressRule(col.Name));
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