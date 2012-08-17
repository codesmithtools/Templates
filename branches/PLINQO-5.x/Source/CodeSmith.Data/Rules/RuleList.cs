using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// A collection of rules
    /// </summary>
    public class RuleList : List<IRule>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleList"/> class.
        /// </summary>
        public RuleList()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleList"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public RuleList(int capacity) : base(capacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleList"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="collection"/> is null.
        /// </exception>
        public RuleList(IEnumerable<IRule> collection) : base(collection)
        { }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is processed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is processed; otherwise, <c>false</c>.
        /// </value>
        internal bool IsProcessed { get; set; }
    }
}
