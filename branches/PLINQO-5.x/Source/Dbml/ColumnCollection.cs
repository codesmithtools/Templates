using System;
using System.Collections.ObjectModel;

namespace LinqToSqlShared.DbmlObjectModel
{
    [Serializable]
    public class ColumnCollection : KeyedCollection<string, Column>, IProcessed
    {
        protected override string GetKeyForItem(Column item)
        {
            return item.Name;
        }

        public bool IsProcessed { get; set; }

    }
}