//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2011 CodeSmith Tools, LLC.  All rights reserved.
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
        private string mappingFile;
        public string MappingFile
        {
            get { return mappingFile; }
            set { mappingFile = value; }
        }

        private string _contextNamespace;
        public string ContextNamespace
        {
            get { return _contextNamespace; }
            set { _contextNamespace = value; }
        }

        private string _dataContextName;
        public string DataContextName
        {
            get { return _dataContextName; }
            set { _dataContextName = value; }
        }

        private string _entityBase;
        public string EntityBase
        {
            get { return _entityBase; }
            set { _entityBase = value; }
        }

        private string _entityNamespace;
        public string EntityNamespace
        {
            get { return _entityNamespace; }
            set { _entityNamespace = value; }
        }

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
