using System;
using System.Collections.Generic;
using CodeSmith.Data.Collections;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// A collection of rules.
    /// </summary>
    public class RuleCollection : ConcurrentDictionary<Type, List<IRule>>
    {}
}