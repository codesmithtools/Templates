using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CodeSmith.Engine;
using SchemaExplorer;
using LinqToSqlShared.DbmlObjectModel;
using Type=LinqToSqlShared.DbmlObjectModel.Type;


namespace LinqToSqlShared.Generator
{
    public class DbmlGenerator
    {
        public event EventHandler<SchemaItemProcessedEventArgs> SchemaItemProcessed;

        private static readonly Regex CleanIdRegex = new Regex(
            @"(_ID|_id|_Id|\.ID|\.id|\.Id|ID|Id)$", RegexOptions.Compiled);

        // must be sorted
        private static readonly string[] ExistingContextProperties = new[] {
        "ChangeConflicts", "CommandTimeout", "Connection", 
        "DeferredLoadingEnabled", 
        "LoadOptions", "Log", 
        "Manager", "Mapping", 
        "ObjectTrackingEnabled", 
        "Provider", 
        "Services", 
        "Transaction"};

        #region Properties
        private GeneratorSettings settings;

        public GeneratorSettings Settings
        {
            get { return settings; }
        }

        private List<string> _classNames = new List<string>();

        internal List<string> ClassNames
        {
            get { return _classNames; }
        }

        private List<string> _functionNames = new List<string>();

        internal List<string> FunctionNames
        {
            get { return _functionNames; }
        }

        private List<string> _associationNames = new List<string>();

        internal List<string> AssociationNames
        {
            get { return _associationNames; }
        }

        private Dictionary<string, List<string>> _propertyNames = new Dictionary<string, List<string>>();

        internal Dictionary<string, List<string>> PropertyNames
        {
            get { return _propertyNames; }
        }

        private Database _database;

        public Database Database
        {
            get { return _database; }
        }
        #endregion

        public DbmlGenerator(GeneratorSettings settings)
        {
            this.settings = settings;
        }

        public Database Create(DatabaseSchema databaseSchema)
        {
            if (File.Exists(settings.MappingFile))
                _database = Dbml.FromFile(settings.MappingFile);
            else
                _database = new Database();

            Database.Name = databaseSchema.Name;
            CreateContext(databaseSchema);

            foreach (TableSchema t in databaseSchema.Tables)
            {
                if (Settings.IsIgnored(t.Name))
                {
                    Debug.WriteLine("Skipping Table: " + t.FullName);
                }
                else
                {
                    Debug.WriteLine("Getting Table Schema: " + t.FullName);
                    GetTable(t);
                }
                OnSchemaItemProcessed(t.FullName);
            }

            if (Settings.IncludeViews)
            {
                foreach (ViewSchema v in databaseSchema.Views)
                {
                    if (Settings.IsIgnored(v.Name))
                    {
                        Debug.WriteLine("Skipping View: " + v.FullName);
                    }
                    else
                    {
                        Debug.WriteLine("Getting View Schema: " + v.FullName);
                        CreateView(v);
                    }
                    OnSchemaItemProcessed(v.FullName);
                }
            }

            if (Settings.IncludeFunctions)
            {
                foreach (CommandSchema c in databaseSchema.Commands)
                {
                    if (Settings.IsIgnored(c.Name))
                    {
                        Debug.WriteLine("Skipping Function: " + c.FullName);
                    }
                    else
                    {
                        Debug.WriteLine("Getting Function Schema: " + c.FullName);
                        CreateFunction(c);
                    }
                    OnSchemaItemProcessed(c.FullName);
                }
            }

            //sync tables
            RemoveExtraMembers(_database);

            _database.Tables.Sort();
            Dbml.ToFile(Dbml.CopyWithNulledOutDefaults(_database), 
                settings.MappingFile);

            return _database;
        }

        private void CreateContext(DatabaseSchema databaseSchema)
        {
            if (!string.IsNullOrEmpty(Settings.ContextNamespace))
                Database.ContextNamespace = Settings.ContextNamespace;

            if (!string.IsNullOrEmpty(Settings.EntityNamespace))
                Database.EntityNamespace = Settings.EntityNamespace;

            if (!string.IsNullOrEmpty(Settings.EntityBase))
                Database.EntityBase = Settings.EntityBase;

            if (string.IsNullOrEmpty(Database.Class))
                Database.Class = ToClassName(databaseSchema.Name + "DataContext");

            if (Database.Connection == null)
                Database.Connection = new Connection("System.Data.SqlClient");

            if (Database.Connection.Mode == ConnectionMode.ConnectionString
                && string.IsNullOrEmpty(Database.Connection.ConnectionString))
                Database.Connection.Mode = ConnectionMode.AppSettings;

            if (string.IsNullOrEmpty(Database.Connection.SettingsObjectName))
                Database.Connection.SettingsObjectName = "Properties.Settings";

            if (string.IsNullOrEmpty(Database.Connection.SettingsPropertyName))
                Database.Connection.SettingsPropertyName = ToLegalName(databaseSchema.Name) + "ConnectionString";

            if (string.IsNullOrEmpty(Database.Connection.ConnectionString))
                Database.Connection.ConnectionString = databaseSchema.ConnectionString;
        }

        private Table GetTable(TableSchema tableSchema)
        {
            return GetTable(tableSchema, true);
        }

        private Table GetTable(TableSchema tableSchema, bool processAssociations)
        {
            Table t;
            string key = tableSchema.FullName;

            if (Database.Tables.Contains(key))
            {
                t = Database.Tables[key];
                ClassNames.Add(t.Type.Name);
            }
            else
            {
                t = CreateTable(tableSchema);
            }

            if (!PropertyNames.ContainsKey(t.Type.Name))
                PropertyNames.Add(t.Type.Name, new List<string>());

            if (!t.Type.Columns.IsProcessed)
                CreateColumns(t, tableSchema);

            if (processAssociations && !t.Type.Associations.IsProcessed)
                CreateAssociations(t, tableSchema);

            t.IsProcessed = true;
            return t;
        }

        private Table CreateTable(TableSchema tableSchema)
        {
            Type type = new Type(ToClassName(tableSchema.Name));
            Table t = new Table(tableSchema.FullName, type);
            t.Member = t.Type.Name;

            if (Array.BinarySearch(ExistingContextProperties, t.Type.Name) >= 1)
                t.Member += "Table";

            Database.Tables.Add(t);

            return t;
        }

        private void CreateAssociations(Table table, TableSchema tableSchema)
        {
            foreach (TableKeySchema tableKey in tableSchema.ForeignKeys)
            {
                if (Settings.IsIgnored(tableKey.ForeignKeyTable.Name)
                    || Settings.IsIgnored(tableKey.PrimaryKeyTable.Name))
                    continue;

                CreateAssociation(table, tableKey);
            }

            table.Type.Associations.IsProcessed = true;
        }

        private void CreateAssociation(Table foreignTable, TableKeySchema tableKeySchema)
        {
            Table primaryTable = GetTable(tableKeySchema.PrimaryKeyTable, false);

            string primaryClass = primaryTable.Type.Name;
            string foreignClass = foreignTable.Type.Name;

            string name = string.Format("{0}_{1}", primaryClass, foreignClass);
            name = MakeUnique(AssociationNames, name);

            string foreignMembers = GetKeyMembers(foreignTable,
                tableKeySchema.ForeignKeyMemberColumns, tableKeySchema.Name);

            string primaryMembers = GetKeyMembers(primaryTable,
                tableKeySchema.PrimaryKeyMemberColumns, tableKeySchema.Name);

            AssociationKey key = AssociationKey.CreateForeignKey(name);
            bool isNew = !foreignTable.Type.Associations.Contains(key);

            Association foreignAssociation;

            if (isNew)
                foreignAssociation = new Association(name);
            else
                foreignAssociation = foreignTable.Type.Associations[key]; 

            foreignAssociation.IsForeignKey = true;
            foreignAssociation.ThisKey = foreignMembers;
            foreignAssociation.OtherKey = primaryMembers;
            foreignAssociation.Type = primaryClass;

            string prefix = GetMemberPrefix(foreignAssociation, primaryClass, foreignClass);

            if (isNew)
            {
                foreignAssociation.Member = ToPropertyName(foreignTable.Type.Name, prefix + primaryClass);
                foreignAssociation.Storage = CommonUtility.GetFieldName(foreignAssociation.Member);
                foreignTable.Type.Associations.Add(foreignAssociation);
            }
            else
            {
                PropertyNames[foreignTable.Type.Name].Add(foreignAssociation.Member);
            }

            // add reverse association
            key = AssociationKey.CreatePrimaryKey(name);
            isNew = !primaryTable.Type.Associations.Contains(key);

            Association primaryAssociation;
            if (isNew)
                primaryAssociation = new Association(name);
            else
                primaryAssociation = primaryTable.Type.Associations[key];

            primaryAssociation.IsForeignKey = false;
            primaryAssociation.ThisKey = foreignAssociation.OtherKey;
            primaryAssociation.OtherKey = foreignAssociation.ThisKey;
            primaryAssociation.Type = foreignClass;

            bool isOneToOne = tableKeySchema.ForeignKeyTable.HasPrimaryKey
                && tableKeySchema.ForeignKeyTable.PrimaryKey != null
                && tableKeySchema.ForeignKeyTable.PrimaryKey.MemberColumns.Count == 1
                && tableKeySchema.ForeignKeyTable.PrimaryKey.MemberColumns.Contains(foreignAssociation.ThisKey);

            if (isOneToOne)
                primaryAssociation.Cardinality = Cardinality.One;

            if (isNew)
            {
                string propertyName = prefix + foreignClass;
                if (!isOneToOne)
                    propertyName += "List";

                primaryAssociation.Member = ToPropertyName(primaryTable.Type.Name, propertyName);
                primaryAssociation.Storage = CommonUtility.GetFieldName(primaryAssociation.Member);
                primaryTable.Type.Associations.Add(primaryAssociation);
            }
            else
            {
                PropertyNames[primaryTable.Type.Name].Add(primaryAssociation.Member);
            }

            if (IsTableKeySchemaCascadeDelete(tableKeySchema))
            {
                foreignAssociation.DeleteRule = "CASCADE";
            }

            if (IsTableDeleteOnNull(tableKeySchema))
            {
                foreignAssociation.DeleteOnNull = true;
            }
            
            foreignAssociation.IsProcessed = true;
            primaryAssociation.IsProcessed = true;
        }

        private bool IsTableKeySchemaCascadeDelete(TableKeySchema tableKeySchema)
        {
            return (tableKeySchema.ExtendedProperties.Contains("CS_CascadeDelete")
                && tableKeySchema.ExtendedProperties["CS_CascadeDelete"].Value != null
                && tableKeySchema.ExtendedProperties["CS_CascadeDelete"].Value is Boolean
                && (bool)tableKeySchema.ExtendedProperties["CS_CascadeDelete"].Value);
        }

        /// <summary>
        /// DeleteOnNull deletes an entity when a Associated Property on that entity is
        /// set to null and the Foreign Key for that association does not allow nulls.
        /// 
        /// Should return true when all of the following conditions are met...
        /// A) A Foreign Key column on the entity does not allow null values.
        /// B) All dependancies on that entity will do a cascade delete.
        /// </summary>
        /// <param name="tableKeySchema">TableKeySchema.ForeignKeyTable is the table being analyzed.</param>
        /// <returns>Value of AssociationAttribute DeleteOnNull Property</returns>
        private bool IsTableDeleteOnNull(TableKeySchema tableKeySchema)
        {
            bool foreignTableForeignKeyColumnAllowDBNull = (tableKeySchema.ForeignKeyMemberColumns.Count == 0);
            foreach (MemberColumnSchema foreignColumn in tableKeySchema.ForeignKeyMemberColumns)
            {
                if (foreignColumn.AllowDBNull)
                {
                    foreignTableForeignKeyColumnAllowDBNull = true;
                    break;
                }
            }

            if (foreignTableForeignKeyColumnAllowDBNull)
                return false;

            bool foreignTablePrimaryKeysCascadeDelete = true;
            foreach (TableKeySchema foreignTableKeySchema in tableKeySchema.ForeignKeyTable.PrimaryKeys)
            {
                if(IsTableKeySchemaCascadeDelete(foreignTableKeySchema))
                {
                    foreignTablePrimaryKeysCascadeDelete = false;
                    break;
                }
            }

            return foreignTablePrimaryKeysCascadeDelete;
        }

        private string GetKeyMembers(Table table, MemberColumnSchemaCollection members, string name)
        {
            StringBuilder keyMembers = new StringBuilder();

            foreach (MemberColumnSchema member in members)
            {
                if (!table.Type.Columns.Contains(member.Name))
                    throw new InvalidOperationException(string.Format(
                        "Could not find column {0} for assoication {1}.",
                        member.Name,
                        name));

                Column column = table.Type.Columns[member.Name];
                if (keyMembers.Length > 0)
                    keyMembers.Append(',');

                keyMembers.Append(column.Member);
            }

            return keyMembers.ToString();
        }

        private void CreateView(ViewSchema viewSchema)
        {
            Table table;

            if (Database.Tables.Contains(viewSchema.FullName))
            {
                table = Database.Tables[viewSchema.FullName];
            }
            else
            {
                Type type = new Type(ToClassName(viewSchema.Name));
                table = new Table(viewSchema.FullName, type);
                Database.Tables.Add(table);
            }

            if (string.IsNullOrEmpty(table.Type.Name))
                table.Type.Name = ToClassName(viewSchema.Name);

            if (string.IsNullOrEmpty(table.Member))
                table.Member = table.Type.Name;

            foreach (ViewColumnSchema columnSchema in viewSchema.Columns)
            {
                Column column;

                if (table.Type.Columns.Contains(columnSchema.Name))
                {
                    column = table.Type.Columns[columnSchema.Name];
                }
                else
                {
                    column = new Column(GetSystemType(columnSchema));
                    column.Name = columnSchema.Name;
                    table.Type.Columns.Add(column);
                }

                PopulateColumn(column, columnSchema, table.Type.Name);
            }

            table.Type.Columns.IsProcessed = true;
            table.Type.Associations.IsProcessed = true;
            table.IsProcessed = true;
        }

        private void CreateFunction(CommandSchema commandSchema)
        {
            Function function;
            string key = commandSchema.FullName;
            bool isNew = !Database.Functions.Contains(key);

            if (isNew)
            {
                function = new Function(key);
                Database.Functions.Add(function);
            }
            else
            {
                function = Database.Functions[key];
            }

            //Function/@Method is safe to update
            if (string.IsNullOrEmpty(function.Method))
            {
                string methodName = ToLegalName(commandSchema.Name);
                if (ClassNames.Contains(methodName))
                    methodName += "Procedure";

                function.Method = MakeUnique(FunctionNames, methodName);
            }

            foreach (ParameterSchema p in commandSchema.Parameters)
            {
                if (p.Direction == System.Data.ParameterDirection.ReturnValue)
                    continue;

                CreateParameter(function, p);
            }

            try
            {
                if (function.Types.Count != commandSchema.CommandResults.Count)
                {
                    function.Types.Clear();
                    foreach (CommandResultSchema r in commandSchema.CommandResults)
                        CreateResult(function, r);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error reading result schema: " + ex.Message);
            }

            function.HasMultipleResults = function.Types.Count > 1;
            function.IsProcessed = true;

            if (function.Types.Count != 0)
                return;

            if (function.Return == null)
                function.Return = new Return("System.Int32");

            if (commandSchema.ReturnValueParameter != null)
            {
                function.Return.Type = commandSchema.ReturnValueParameter.SystemType.ToString();
                function.Return.DbType = GetDbType(commandSchema.ReturnValueParameter);
            }
            else
            {
                function.Return.Type = "System.Int32";
                function.Return.DbType = "int";
            }

        }

        private void CreateResult(Function function, CommandResultSchema commandResultSchema)
        {
            string name = function.Method + "Result";
            Type result;

            if (function.Types.Contains(name))
            {
                result = function.Types[name];
            } 
            else
            {
                result = new Type(ToClassName(name));                
                function.Types.Add(result);
            }
            
            // ElementType/@Name safe attribute
            if (string.IsNullOrEmpty(result.Name))
                result.Name = ToClassName(name);

            foreach (CommandResultColumnSchema c in commandResultSchema.Columns)
            {
                Column column;

                if (result.Columns.Contains(c.Name))
                {
                    column = result.Columns[c.Name];
                }
                else
                {
                    column = new Column(GetSystemType(c));
                    column.Name = c.Name;
                    result.Columns.Add(column);
                }

                PopulateColumn(column, c, result.Name);
            }
        }

        private void CreateParameter(Function function, ParameterSchema parameterSchema)
        {
            Parameter parameter;
            if (function.Parameters.Contains(parameterSchema.Name))
            {
                parameter = function.Parameters[parameterSchema.Name];
            }
            else
            {
                parameter = new Parameter(parameterSchema.Name, 
                    parameterSchema.SystemType.ToString());
                function.Parameters.Add(parameter);
            }

            //Parameter/@Parameter is safe
            if (string.IsNullOrEmpty(parameter.ParameterName))
            {
                string parameterName = parameterSchema.Name;
                if (parameterName.StartsWith("@"))
                    parameterName = parameterName.Substring(1);

                parameter.ParameterName = ToParameterName(parameterName);
            }

            parameter.Type = parameterSchema.SystemType.ToString();
            parameter.DbType = GetDbType(parameterSchema);

            switch (parameterSchema.Direction)
            {
                case System.Data.ParameterDirection.Input:
                    parameter.Direction = ParameterDirection.In;
                    break;
                case System.Data.ParameterDirection.InputOutput:
                    parameter.Direction = ParameterDirection.InOut;
                    break;
                case System.Data.ParameterDirection.Output:
                    parameter.Direction = ParameterDirection.Out;
                    break;
            }
        }

        private string GetMemberPrefix(Association association, string primaryClass, string foreignClass)
        {
            bool isSameName = association.ThisKey.Equals(association.OtherKey,
                StringComparison.OrdinalIgnoreCase);
            isSameName = (isSameName || association.ThisKey.Equals(
                primaryClass + association.OtherKey,
                StringComparison.OrdinalIgnoreCase));

            string prefix = string.Empty;
            if (isSameName)
                return prefix;

            prefix = association.ThisKey.Replace(association.OtherKey, "");
            prefix = prefix.Replace(primaryClass, "");
            prefix = prefix.Replace(foreignClass, "");
            prefix = CleanIdRegex.Replace(prefix, "");
            return prefix;
        }

        private void CreateColumns(Table table, TableSchema tableSchema)
        {

            foreach (ColumnSchema columnSchema in tableSchema.Columns)
            {
                bool isNew = !table.Type.Columns.Contains(columnSchema.Name);
                Column column;

                if (isNew)
                {
                    column = new Column(GetSystemType(columnSchema));
                    column.Name = columnSchema.Name;
                    table.Type.Columns.Add(column);
                }
                else
                {
                    column = table.Type.Columns[columnSchema.Name];
                }

                PopulateColumn(column, columnSchema, table.Type.Name);
                column.IsPrimaryKey = columnSchema.IsPrimaryKeyMember;
            }

            table.Type.Columns.IsProcessed = true;
        }

        private void PopulateColumn(Column column, DataObjectBase columnSchema, string className)
        {
            bool canUpdateType = string.IsNullOrEmpty(column.Type)
                || column.Type.StartsWith("System.");

            if (canUpdateType)
                column.Type = GetSystemType(columnSchema);

            if (!PropertyNames.ContainsKey(className))
                PropertyNames.Add(className, new List<string>());

            //Column/@Member is edit safe
            if (string.IsNullOrEmpty(column.Member))
            {
                column.Member = ToPropertyName(className, columnSchema.Name);
                column.Storage = CommonUtility.GetFieldName(column.Member);
            }
            else
            {
                PropertyNames[className].Add(column.Member);
            }

            if (columnSchema.NativeType.Equals("text", StringComparison.OrdinalIgnoreCase)
                || columnSchema.NativeType.Equals("ntext", StringComparison.OrdinalIgnoreCase))
                column.UpdateCheck = UpdateCheck.Never;

            column.DbType = GetDbType(columnSchema);
            column.CanBeNull = columnSchema.AllowDBNull;
            column.IsVersion = IsRowVersion(columnSchema);
            column.IsDbGenerated = IsDbGenerated(columnSchema);
            column.IsProcessed = true;
        }

        private static string GetSystemType(DataObjectBase d)
        {
            return d.SystemType == typeof(XmlDocument) ? "System.String" : d.SystemType.ToString();
        }

        private static string MakeUnique(ICollection<string> existingItems, string name)
        {
            string uniqueName = name;
            int count = 1;

            while (existingItems.Contains(uniqueName))
                uniqueName = string.Concat(name, count++);

            existingItems.Add(uniqueName);
            return uniqueName;
        }

        private static void RemoveExtraMembers(Database database)
        {
            List<Table> removedTables = new List<Table>();
            List<Column> removedColumns = new List<Column>();
            List<Association> removedAssociations = new List<Association>();
            List<Function> removedFunctions = new List<Function>();

            foreach (Table table in database.Tables)
            {
                if (!table.IsProcessed)
                {
                    removedTables.Add(table);
                    continue;
                }

                removedColumns.Clear();
                foreach (Column c in table.Type.Columns)
                    if (!c.IsProcessed)
                        removedColumns.Add(c);

                removedAssociations.Clear();
                foreach (Association a in table.Type.Associations)
                    if (!a.IsProcessed)
                        removedAssociations.Add(a);

                foreach (Column c in removedColumns)
                    table.Type.Columns.Remove(c);

                foreach (Association a in removedAssociations)
                    table.Type.Associations.Remove(a);
            }

            foreach (Table t in removedTables)
                database.Tables.Remove(t);

            foreach (Function f in database.Functions)
                if (!f.IsProcessed)
                    removedFunctions.Add(f);

            foreach (Function f in removedFunctions)
                database.Functions.Remove(f);

        }

        private string ToParameterName(string name)
        {
            string legalName = ToLegalName(name);
            if (Settings.DisableRenaming)
                return legalName;

            legalName = CommonUtility.GetParameterName(legalName);
            return legalName;
        }

        private string ToClassName(string name)
        {
            string legalName = ToLegalName(name);
            legalName = MakeUnique(ClassNames, legalName);
            return legalName;
        }

        private string ToPropertyName(string className, string name)
        {
            string propertyName = ToLegalName(name);
            if (className.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                propertyName += "Member";

            if (!PropertyNames.ContainsKey(className))
                PropertyNames.Add(className, new List<string>());

            propertyName = MakeUnique(PropertyNames[className], propertyName);
            return propertyName;
        }

        private string ToLegalName(string name)
        {
            string legalName = Settings.CleanName(name);
            if (Settings.DisableRenaming)
                return legalName.Replace(' ', '_');

            legalName = StringUtil.ToPascalCase(legalName);
            return legalName;
        }

        protected void OnSchemaItemProcessed(string name)
        {
            if (SchemaItemProcessed != null)
            {
                SchemaItemProcessed(this, new SchemaItemProcessedEventArgs(name));
            }
        }

        #region Column Flag Helpers

        private static bool IsRowVersion(DataObjectBase column)
        {
            bool isTimeStamp = column.NativeType.Equals(
                "timestamp", StringComparison.OrdinalIgnoreCase);
            bool isRowVersion = column.NativeType.Equals(
                "rowversion", StringComparison.OrdinalIgnoreCase);

            return (isTimeStamp || isRowVersion);
        }

        private static bool IsDbGenerated(DataObjectBase column)
        {
            if (IsRowVersion(column))
                return true;

            if (IsIdentity(column))
                return true;

            bool isComputed = false;
            string value;
            try
            {
                if (column.ExtendedProperties.Contains("CS_IsComputed"))
                {
                    value = column.ExtendedProperties["CS_IsComputed"].Value.ToString();
                    bool.TryParse(value, out isComputed);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }

            return isComputed;
        }

        private static bool IsIdentity(DataObjectBase column)
        {
            string temp;
            bool isIdentity = false;
            try
            {
                if (column.ExtendedProperties.Contains("CS_IsIdentity"))
                {
                    temp = column.ExtendedProperties["CS_IsIdentity"].Value.ToString();
                    bool.TryParse(temp, out isIdentity);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }

            return isIdentity;
        }

        #endregion

        #region DbType Helpers

        private static string GetDbType(ParameterSchema parameterSchema)
        {
            if (parameterSchema.Database.Provider.Name != "SqlSchemaProvider")
                return parameterSchema.NativeType;

            SqlParameter param = new SqlParameter();
            param.DbType = parameterSchema.DataType;

            return GetSqlTypeDeclaration(
                parameterSchema.NativeType,
                parameterSchema.Size,
                parameterSchema.Precision,
                parameterSchema.Scale,
                false,
                false);
        }

        private static string GetDbType(DataObjectBase column)
        {
            if (column.Database.Provider.Name != "SqlSchemaProvider")
                return column.NativeType;

            return GetSqlTypeDeclaration(
                column.NativeType,
                column.Size,
                column.Precision,
                column.Scale,
                !column.AllowDBNull,
                IsIdentity(column));
        }

        private static string GetSqlTypeDeclaration(string nativeType, int size, int precision, int scale, bool nonNull, bool isAutoIncrement)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(nativeType);

            switch (nativeType.Trim().ToLower())
            {
                case "binary":
                case "char":
                case "nchar":
                case "nvarchar":
                case "varbinary":
                case "varchar":
                    // add size
                    builder.AppendFormat("({0})", (size == 0x7fffffff || size == -1) ? "MAX" : size.ToString());
                    break;
                case "decimal":
                case "numeric":
                    // add scale
                    builder.AppendFormat("({0},{1})", precision, scale);
                    break;
            }

            if (nonNull)
                builder.Append(" NOT NULL");

            if (isAutoIncrement)
                builder.Append(" IDENTITY");

            return builder.ToString();
        }

        #endregion

    }
}
