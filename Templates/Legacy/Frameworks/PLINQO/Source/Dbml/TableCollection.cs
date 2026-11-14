using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LinqToSqlShared.DbmlObjectModel
{
    [Serializable]
    public class TableCollection : KeyedCollection<string, Table>
    {
        protected override string GetKeyForItem(Table item)
        {
            return item.Name;
        }

        public void Sort()
        {
            List<Table> tables = new List<Table>(base.Items);
            tables.Sort(delegate(Table x, Table y) { return x.Name.CompareTo(y.Name); });
            base.Clear();
            foreach (Table t in tables)
                base.Add(t);
        }
    }
}