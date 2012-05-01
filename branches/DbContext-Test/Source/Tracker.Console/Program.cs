using System;
using System.Collections.Generic;
using System.Data.Mapping;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Tracker.Core;
using Tracker.Entity;

namespace Tracker.Console
{
  class Program
  {
    static void Main(string[] args)
    {
    }

    private static bool MetadataProperyCompare<TValue>(ReadOnlyMetadataCollection<MetadataProperty> readOnlyMetadataCollection, string name, TValue value)
    {
      if (!readOnlyMetadataCollection.Contains(name))
        return false;


      var dataSpace = readOnlyMetadataCollection[name].Value;
      if (dataSpace == null || dataSpace.GetType() != typeof(TValue))
        return false;

      return Equals((TValue)dataSpace, value);
    }
  }

}

