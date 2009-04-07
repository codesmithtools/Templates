using System;
using System.Collections.ObjectModel;
using System.Text;

namespace LinqToSqlShared.DbmlObjectModel
{
    [Serializable]
    public class TableFunctionParameterCollection : KeyedCollection<string, TableFunctionParameter>
    {
        protected override string GetKeyForItem(TableFunctionParameter item)
        {
            return item.ParameterName;
        }
    }

}
