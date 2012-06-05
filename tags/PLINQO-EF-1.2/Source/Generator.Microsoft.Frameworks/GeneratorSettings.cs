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

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Generator.Microsoft.Frameworks
{
    public class GeneratorSettings
    {
        public string MappingFile { get; set; }

        public string ContextNamespace { get; set; }

        public string DataContextName { get; set; }

        public string EntityBase { get; set; }

        public string EntityNamespace { get; set; }

        private List<Regex> _ignoreExpressions = new List<Regex>();
        public List<Regex> IgnoreExpressions
        {
            get { return _ignoreExpressions; }
        }

        private List<Regex> _includeExpressions = new List<Regex>();
        public List<Regex> IncludeExpressions
        {
            get { return _includeExpressions; }
        }

        private List<Regex> _cleanExpressions = new List<Regex>();
        public List<Regex> CleanExpressions
        {
            get { return _cleanExpressions; }
        }

        private string _databaseName;
        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = CodeSmith.SchemaHelper.Util.NamingConventions.PropertyName(value); }
        }
    }
}
