using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace EntityFramework.Extentions
{
  public static class DbDataRecordExtensions
  {
    public static object GetValue(this DbDataRecord record, string name)
    {
      int ordinal = record.GetOrdinal(name);
      if (record.IsDBNull(ordinal))
        return null;

      return record.GetValue(ordinal);
    }
  }
}
