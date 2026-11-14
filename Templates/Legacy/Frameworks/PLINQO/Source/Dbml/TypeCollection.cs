using System;
using System.Collections.ObjectModel;

namespace LinqToSqlShared.DbmlObjectModel
{
    [Serializable]
    public class TypeCollection : KeyedCollection<string, Type>
    {
        protected override string GetKeyForItem(Type item)
        {
            return item.Name;
        }
    }
}