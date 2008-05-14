using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using DbmlSchema;
using CodeSmith.Engine;
using SchemaExplorer;
using System.Text;
using System.Collections;

namespace Manager
{
    public class ManagerGenerator
    {
        public static DataManager Create(DatabaseSchema databaseSchema, Database database)
        {
            DataManager managerMapping = new DataManager(database);
            GetMethods(databaseSchema, database, managerMapping);
            return managerMapping;
        }

        private static void GetMethods(DatabaseSchema databaseSchema, Database database, DataManager managerMapping)
        {
            foreach (EntityManager manager in managerMapping.Managers)
            {
                Table tableMapping = database.Tables[manager.TableName];
                TableSchema table;
                string[] parts = manager.TableName.Split('.');

                if (parts.Length == 2)
                    table = databaseSchema.Tables[parts[0], parts[1]];
                else
                    table = databaseSchema.Tables[manager.TableName];

                if (table == null)
                    continue;

                if (table.HasPrimaryKey)
                {
                    ManagerMethod method = GetMethodFromColumns(tableMapping, table.PrimaryKey.MemberColumns);
                    method.IsKey = true;
                    if (!manager.Methods.Contains(method.NameSuffix))
                        manager.Methods.Add(method);
                }

                GetIndexes(manager, tableMapping, table);
                GetForienKeys(manager, tableMapping, table);
            }
        }

        private static void GetForienKeys(EntityManager manager, Table tableMapping, TableSchema table)
        {
            List<ColumnSchema> columns = new List<ColumnSchema>();

            foreach (ColumnSchema column in table.ForeignKeyColumns)
            {
                columns.Add(column);

                ManagerMethod method = GetMethodFromColumns(tableMapping, columns);
                if (!manager.Methods.Contains(method.NameSuffix))
                    manager.Methods.Add(method);

                columns.Clear();
            }
        }

        private static void GetIndexes(EntityManager manager, Table tableMapping, TableSchema table)
        {
            foreach (IndexSchema index in table.Indexes)
            {
                ManagerMethod method = GetMethodFromColumns(tableMapping, index.MemberColumns);
                method.IsUnique = index.IsUnique;

                if (!manager.Methods.Contains(method.NameSuffix))
                    manager.Methods.Add(method);
            }
        }

        private static ManagerMethod GetMethodFromColumns(Table tableMapping, IList columns)
        {
            ManagerMethod method = new ManagerMethod();
            method.EntityName = tableMapping.Type.Name;
            string methodName = string.Empty;
            foreach (ColumnSchema column in columns)
            {
                Column columnMapping = tableMapping.Type.Columns[column.Name];
                method.Columns.Add(columnMapping);
                methodName += columnMapping.Member;
            }
            method.NameSuffix = methodName;
            return method;
        }

    }

    public class DataManager
    {
        public const string ManagerSuffix = "Manager";
        public const string DataManagerSuffix = "DataManager";

        public DataManager(Database database)
        {
            Initialize(database);
        }

        public DataManager()
        { }

        public void Initialize(Database database)
        {
            DataManagerName = StringUtil.ToPascalCase(database.Name) + DataManagerSuffix;
            DataContextName = CommonUtility.GetClassName(database.Class);
            foreach (Table table in database.Tables)
            {
                Managers.Add(new EntityManager(table));
            }
        }

        public string DataManagerName;
        public string DataContextName;

        public List<EntityManager> Managers = new List<EntityManager>();

        public override string ToString()
        {
            return DataManagerName;
        }
    }


    public class EntityManager
    {
        public EntityManager(Table table)
        {
            TableName = table.Name;
            EntityName = table.Type.Name;
            PropertyName = string.IsNullOrEmpty(table.Member) ? table.Type.Name : table.Member;
            ManagerName = EntityName + DataManager.ManagerSuffix;
            FieldName = CommonUtility.GetFieldName(ManagerName);
        }

        public EntityManager()
        { }

        public string TableName;
        public string EntityName;
        public string ManagerName;

        public string FieldName;
        public string PropertyName;

        public ManagerMethodCollection Methods = new ManagerMethodCollection();

        public override string ToString()
        {
            return ManagerName;
        }
    }

    public class ManagerMethod
    {
        public string NameSuffix;
        public string EntityName;
        public bool IsKey;
        public bool IsUnique;
        
        public List<Column> Columns = new List<Column>();

        public override string ToString()
        {
            return NameSuffix;
        }
    }

    public class ManagerMethodCollection : KeyedCollection<string, ManagerMethod>
    {
        protected override string GetKeyForItem(ManagerMethod item)
        {
            return item.NameSuffix;
        }
    }
}