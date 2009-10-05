//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2009 CodeSmith Tools, LLC.  All rights reserved.
// 
// The terms of use for this software are contained in the file
// named sourcelicense.txt, which can be found in the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by the
// terms of this license.
// 
// You must not remove this notice, or any other, from this software.
//
//------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Text;
using CodeSmith.Engine;

namespace CodeSmith.Samples
{
    [PropertySerializer(typeof(TableConfigurationCollectionSerializer))]
    [Serializable]
    public class TableConfigurationCollection : Collection<TableConfiguration>
    {
        public TableConfigurationCollection()
        {
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (TableConfiguration oItem in this)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(oItem.SourceTable.Name);
            }

            return builder.ToString();
        }
    }
}
