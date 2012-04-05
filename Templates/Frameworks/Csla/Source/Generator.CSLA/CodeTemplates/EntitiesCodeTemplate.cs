using System;
using System.IO;
using Generator.CSLA.CodeTemplates;

namespace Generator.CSLA
{
    /// <summary>
    /// This template will be used in the Entities.cst and other related templates.
    /// </summary>
    public class EntitiesCodeTemplate : SchemaExplorerEntitiesCodeTemplate
    {
        public override void OnDatabaseChanged()
        {
            base.OnDatabaseChanged();

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
