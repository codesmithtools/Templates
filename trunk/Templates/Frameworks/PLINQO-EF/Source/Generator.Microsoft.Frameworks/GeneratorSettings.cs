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
using SchemaExplorer;

namespace Generator.Microsoft.Frameworks {
    public class GeneratorSettings {
        public GeneratorSettings(IDatabaseSchema schema) {
            EntityFrameworkVersion = EntityFrameworkVersion.v6;

            DatabaseName = schema.Name;
            switch (schema.Provider.Name) {
                case "SQLAnywhereSchemaProvider":
                    SchemaProviderName = "iAnywhere.Data.SQLAnywhere";
                    SchemaProviderManifestToken = "12";
                    break;

                case "MySQLSchemaProvider":
                    SchemaProviderName = "MySql.Data.MySqlClient";
                    SchemaProviderManifestToken = "5.5";
                    break;

                default:
                    SchemaProviderName = "System.Data.SqlClient";
                    SchemaProviderManifestToken = "2008";
                    break;
            }
        }

        public string MappingFile { get; set; }

        public string ContextNamespace { get; set; }

        public string DataContextName { get; set; }

        public string EntityBase { get; set; }

        public string EntityNamespace { get; set; }

        private readonly List<Regex> _ignoreExpressions = new List<Regex>();
        public List<Regex> IgnoreExpressions { get { return _ignoreExpressions; } }

        private readonly List<Regex> _includeExpressions = new List<Regex>();
        public List<Regex> IncludeExpressions { get { return _includeExpressions; } }

        private readonly List<Regex> _cleanExpressions = new List<Regex>();
        public List<Regex> CleanExpressions { get { return _cleanExpressions; } }

        private string _databaseName;
        public string DatabaseName { 
            get { return _databaseName; } 
            set { _databaseName = CodeSmith.SchemaHelper.Util.NamingConventions.PropertyName(value); } 
        }

        public string SchemaProviderName { get; private set; }
        public string SchemaProviderManifestToken { get; private set; }

        public EntityFrameworkVersion EntityFrameworkVersion { get; set; }
    }
}