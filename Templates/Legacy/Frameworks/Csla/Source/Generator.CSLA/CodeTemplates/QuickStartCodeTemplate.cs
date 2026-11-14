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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using CodeSmith.CustomProperties;
using Generator.CSLA.CodeTemplates;
using SchemaExplorer;

namespace Generator.CSLA
{
    public class QuickStartCodeTemplate : EntitiesBaseCodeTemplate
    {
        private DatabaseSchema _database;

        public QuickStartCodeTemplate() {
            IgnoreExpressions = new StringCollection(new[] { "sysdiagrams$", "^dbo.aspnet", "^dbo.vw_aspnet" });
            CleanExpressions = new StringCollection(new[] { "^(sp|tbl|udf|vw)_" });
        }

        [Category("1. DataSource")]
        [Description("Source Database")]
        public DatabaseSchema SourceDatabase
        {
            get { return _database; }
            set
            {
                if (value != null && (_database == null || (_database != null && value.Name != _database.Name && value.ConnectionString != _database.ConnectionString)))
                {
                    _database = value;
                    if (!_database.DeepLoad)
                    {
                        _database.DeepLoad = true;
                        _database.Refresh();
                    }

                    OnDatabaseChanged();
                }
            }
        }

        public static void LaunchVisualStudioWithSolution(string solutionLink)
        {
            const string args = "/build debug";
            using (Process.Start(solutionLink, args))
            { }
        }

        public virtual void OnDatabaseChanged()
        {
            //if (String.IsNullOrEmpty(DataClassName))
            //    DataClassName = "DataAccessLayer";

            if (String.IsNullOrEmpty(SolutionName))
                SolutionName = SourceDatabase.Name; //.Namespace();

            if (String.IsNullOrEmpty(ProcedurePrefix))
                ProcedurePrefix = "CSLA_";

            if (String.IsNullOrEmpty(Location))
                Location = Path.Combine(CodeSmith.Engine.Configuration.Instance.CodeSmithTemplatesDirectory, Path.Combine("CSLA", SourceDatabase.Name));
        }

        public override void OnDataAccessImplementationChanged()
        {
            if (DataAccessImplementation == DataAccessMethod.LinqToSQL)
            {
                if (String.IsNullOrEmpty(LinqToSQLContextNamespace))
                {
                    LinqToSQLContextNamespace = String.Format("{0}.Data", SourceDatabase);
                    LinqToSQLDataContextName = String.Format("{0}DataContext", SourceDatabase);
                }
            }
            else
            {
                LinqToSQLContextNamespace = String.Empty;
                LinqToSQLDataContextName = String.Empty;
            }
        }
    }
}
