using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// A collection of rules.
    /// </summary>
    public class RuleCollection : Dictionary<Type, List<IRule>>
    {
    }
}
