using System;
using System.Collections.Generic;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// A collection of rules.
    /// </summary>
    public class RuleCollection : Dictionary<Type, List<IRule>>
    {}
}