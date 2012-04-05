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
using Generator.CSLA.CodeTemplates;
using SchemaExplorer;

namespace Generator.CSLA
{
    public class QuickStartCodeTemplate : EntitiesBaseCodeTemplate
    {
        private DatabaseSchema _database;

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
            //if (string.IsNullOrEmpty(DataClassName))
            //    DataClassName = "DataAccessLayer";

            if (string.IsNullOrEmpty(SolutionName))
                SolutionName = SourceDatabase.Name; //.Namespace();

            if (string.IsNullOrEmpty(ProcedurePrefix))
                ProcedurePrefix = "CSLA_";

            if (string.IsNullOrEmpty(Location))
                Location = Path.Combine(CodeSmith.Engine.Configuration.Instance.CodeSmithTemplatesDirectory, Path.Combine("CSLA", SourceDatabase.Name));
        }

        public override void OnDataAccessImplementationChanged()
        {
            if (DataAccessImplementation == DataAccessMethod.LinqToSQL)
            {
                if (string.IsNullOrEmpty(LinqToSQLContextNamespace))
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
