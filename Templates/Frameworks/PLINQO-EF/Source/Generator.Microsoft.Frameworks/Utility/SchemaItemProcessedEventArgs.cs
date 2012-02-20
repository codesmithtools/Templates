//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2012 CodeSmith Tools, LLC.  All rights reserved.
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

namespace Generator.Microsoft.Frameworks
{
    [Serializable]
    public class SchemaItemProcessedEventArgs : EventArgs
    {
        public SchemaItemProcessedEventArgs(string name)
        {
            _name = name;
        }

        private string _name;

        public string Name
        {
            get { return _name; }
        }
    }
}