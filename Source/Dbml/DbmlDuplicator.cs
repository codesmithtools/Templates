using System;
using System.Collections.Generic;

namespace LinqToSqlShared.DbmlObjectModel
{
    public static partial class Dbml
    {
        #region Nested type: DbmlDuplicator

        private class DbmlDuplicator : DbmlVisitor
        {
            private Dictionary<TableFunction, int> functionReferences;
            private Database originalDb;
            private Dictionary<string, Type> processedTypes;

            public Database DuplicateDatabase(Database db)
            {
                functionReferences = new Dictionary<TableFunction, int>();
                processedTypes = new Dictionary<string, Type>();
                db = VisitDatabase(db);
                return db;
            }

            private static int FindReferencedFunctionIndex(Database db, Function function)
            {
                int num = 0;
                foreach (Function f in db.Functions)
                {
                    if (function == f)
                        return num;
                    
                    num++;
                }
                throw new InvalidOperationException();
            }

            public override Association VisitAssociation(Association association)
            {
                if (association == null)
                    return null;
                
                var a = new Association(association.Name)
                                       {
                                           Member = association.Member,
                                           Storage = association.Storage,
                                           AccessModifier = association.AccessModifier,
                                           Modifier = association.Modifier
                                       };
                a.SetThisKey(association.GetThisKey());
                a.SetOtherKey(association.GetOtherKey());
                a.IsForeignKey = association.IsForeignKey;
                a.Cardinality = association.Cardinality;
                a.DeleteOnNull = association.DeleteOnNull;
                a.DeleteRule = association.DeleteRule;
                a.Type = association.Type;
                return a;
            }

            public override Column VisitColumn(Column column)
            {
                if (column == null)
                    return null;
                
                return new Column(column.Type)
                           {
                               Name = column.Name,
                               Member = column.Member,
                               Storage = column.Storage,
                               AccessModifier = column.AccessModifier,
                               Modifier = column.Modifier,
                               AutoSync = column.AutoSync,
                               DbType = column.DbType,
                               IsReadOnly = column.IsReadOnly,
                               IsPrimaryKey = column.IsPrimaryKey,
                               IsDbGenerated = column.IsDbGenerated,
                               CanBeNull = column.CanBeNull,
                               UpdateCheck = column.UpdateCheck,
                               IsDiscriminator = column.IsDiscriminator,
                               Expression = column.Expression,
                               IsVersion = column.IsVersion,
                               IsDelayLoaded = column.IsDelayLoaded
                           };
            }

            public override Connection VisitConnection(Connection connection)
            {
                if (connection == null)
                    return null;
                
                return new Connection(connection.Provider)
                           {
                               Mode = connection.Mode,
                               ConnectionString = connection.ConnectionString,
                               SettingsObjectName = connection.SettingsObjectName,
                               SettingsPropertyName = connection.SettingsPropertyName
                           };
            }

            public override Database VisitDatabase(Database db)
            {
                if (db == null)
                    return null;
                
                var database = new Database();
                originalDb = db;
                database.Name = db.Name;
                database.EntityNamespace = db.EntityNamespace;
                database.ContextNamespace = db.ContextNamespace;
                database.Class = db.Class;
                database.AccessModifier = db.AccessModifier;
                database.Modifier = db.Modifier;
                database.BaseType = db.BaseType;
                database.Provider = db.Provider;
                database.ExternalMapping = db.ExternalMapping;
                database.Serialization = db.Serialization;
                database.EntityBase = db.EntityBase;
                database.Connection = VisitConnection(db.Connection);

                foreach (Table table in db.Tables)
                    database.Tables.Add(VisitTable(table));

                foreach (Function function in db.Functions)
                    database.Functions.Add(VisitFunction(function));

                foreach (TableFunction function in functionReferences.Keys)
                    function.MappedFunction = database.Functions[functionReferences[function]];

                return database;
            }

            public override Function VisitFunction(Function f)
            {
                if (f == null)
                    return null;
                
                var function = new Function(f.Name)
                                   {
                                       Method = f.Method,
                                       AccessModifier = f.AccessModifier,
                                       Modifier = f.Modifier,
                                       HasMultipleResults = f.HasMultipleResults,
                                       IsComposable = f.IsComposable
                                   };

                foreach (Parameter parameter in f.Parameters)
                    function.Parameters.Add(VisitParameter(parameter));

                foreach (Type type in f.Types)
                    function.Types.Add(VisitType(type));

                function.Return = VisitReturn(f.Return);
                
                return function;
            }

            public override Parameter VisitParameter(Parameter parameter)
            {
                if (parameter == null)
                    return null;
                
                return new Parameter(parameter.Name, parameter.Type)
                           {
                               ParameterName = parameter.ParameterName,
                               Type = parameter.Type,
                               DbType = parameter.DbType,
                               Direction = parameter.Direction
                           };
            }

            public override Return VisitReturn(Return r)
            {
                if (r == null)
                    return null;
                
                return new Return(r.Type) {DbType = r.DbType};
            }

            public override Table VisitTable(Table table)
            {
                if (table == null)
                    return null;
                
                return new Table(table.Name, VisitType(table.Type))
                           {
                               Member = table.Member,
                               AccessModifier = table.AccessModifier,
                               Modifier = table.Modifier,
                               Type = VisitType(table.Type),
                               InsertFunction = VisitTableFunction(table.InsertFunction),
                               UpdateFunction = VisitTableFunction(table.UpdateFunction),
                               DeleteFunction = VisitTableFunction(table.DeleteFunction)
                           };
            }

            public override TableFunction VisitTableFunction(TableFunction tf)
            {
                if (tf == null)
                    return null;
                
                var key = new TableFunction
                              {
                                  AccessModifier = tf.AccessModifier
                              };
                
                int num = FindReferencedFunctionIndex(originalDb, tf.MappedFunction);
                functionReferences.Add(key, num);
                
                foreach (TableFunctionParameter parameter in tf.Arguments)
                {
                    key.Arguments.Add(VisitTableFunctionParameter(parameter));
                }
                
                key.Return = VisitTableFunctionReturn(tf.Return);                
                return key;
            }

            public override TableFunctionParameter VisitTableFunctionParameter(TableFunctionParameter parameter)
            {
                return parameter == null ? null : new TableFunctionParameter(parameter.ParameterName, parameter.Member)
                                                      {Version = parameter.Version};
            }

            public override TableFunctionReturn VisitTableFunctionReturn(TableFunctionReturn r)
            {
                return r == null ? null : new TableFunctionReturn {Member = r.Member};
            }

            public override Type VisitType(Type type)
            {
                if (type == null)
                    return null;

                Type t;
                if (processedTypes.TryGetValue(type.Name, out t))
                    return t;

                t = new Type(type.Name);
                processedTypes.Add(type.Name, t);
                t.InheritanceCode = type.InheritanceCode;
                t.IsInheritanceDefault = type.IsInheritanceDefault;
                t.AccessModifier = type.AccessModifier;
                t.Modifier = type.Modifier;

                foreach (Column column in type.Columns)
                    t.Columns.Add(VisitColumn(column));

                foreach (Association association in type.Associations)
                    t.Associations.Add(VisitAssociation(association));

                foreach (Type subType in type.SubTypes)
                    t.SubTypes.Add(VisitType(subType));
                
                return t;
            }
        }

        #endregion
    }
}