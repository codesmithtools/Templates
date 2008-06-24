using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// A collection of broken rules.
    /// </summary>
    public class BrokenRuleCollection : List<BrokenRule>
    {
        /// <summary>
        /// Filters colleciton to the specified type.
        /// </summary>
        /// <param name="type">The type to filter.</param>
        /// <returns>Enumerator of BrokenRule for the type.</returns>
        public IEnumerator<BrokenRule> Filter(Type type)
        {
            foreach (BrokenRule r in this)
            {
                bool isMatch = (r.Context.TrackedObject.Current.GetType() == type);
                if (isMatch)
                    yield return r;
            }
        }

        /// <summary>
        /// Filters colleciton to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to filter.</typeparam>
        /// <returns>Enumerator of BrokenRule for the type.</returns>
        public IEnumerator<BrokenRule> Filter<T>()
        {
            return Filter(typeof(T));
        }

        /// <summary>
        /// Filters colleciton to the specified type and property.
        /// </summary>
        /// <param name="property">The property to filter.</param>
        /// <param name="type">The type to filter.</param>
        /// <returns>Enumerator of BrokenRule for the type.</returns>        
        public IEnumerator<BrokenRule> Filter(Type type, string property)
        {
            foreach (BrokenRule r in this)
            {
                bool isMatch = (r.Context.TrackedObject.Current.GetType() == type)
                    && property == r.Context.Rule.TargetProperty;

                if (isMatch)
                    yield return r;
            }
        }

        /// <summary>
        /// Filters colleciton to the specified type and property.
        /// </summary>
        /// <param name="property">The property to filter.</param>
        /// <typeparam name="T">The type to filter.</typeparam>
        /// <returns>Enumerator of BrokenRule for the type.</returns>        
        public IEnumerator<BrokenRule> Filter<T>(string property)
        {
            return Filter(typeof(T), property);
        }

    }
}
