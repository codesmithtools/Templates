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
using CodeSmith.Engine;
using SchemaExplorer;

namespace CodeSmith.Samples
{
    [PropertySerializer(typeof(TableConfigurationSerializer))]
    [Serializable]
    public class TableConfiguration
    {
        public TableConfiguration()
        {
        }

        public TableSchema SourceTable { get; set; }

        public ViewSchema SourceView { get; set; }

        public override string ToString()
        {
            return (SourceTable != null) ? SourceTable.ToString() : "The table has not been set.";
        }
    }
}
