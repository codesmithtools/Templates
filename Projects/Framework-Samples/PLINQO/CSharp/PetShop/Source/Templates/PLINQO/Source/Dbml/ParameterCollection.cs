using System;
using System.Collections.ObjectModel;

namespace LinqToSqlShared.DbmlObjectModel
{
    [Serializable]
    public class ParameterCollection : KeyedCollection<string, Parameter>
    {
        protected override string GetKeyForItem(Parameter item)
        {
            return item.Name;
        }
    }
}