using System;
using System.Collections.ObjectModel;

namespace LinqToSqlShared.DbmlObjectModel
{
    [Serializable]
    public class AssociationCollection : KeyedCollection<string, Association>, IProcessed
    {
        protected override string GetKeyForItem(Association item)
        {
            return item.ToKey();
        }

        public bool IsProcessed { get; set; }
    }
}