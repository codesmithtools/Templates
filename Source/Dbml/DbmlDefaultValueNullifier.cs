using System.Collections.Generic;

namespace LinqToSqlShared.DbmlObjectModel
{
    public static partial class Dbml
    {
        #region Nested type: DbmlDefaultValueNullifier

        private class DbmlDefaultValueNullifier : DbmlVisitor
        {
            private Dictionary<Association, Association> associationPartners;
            private Dictionary<string, Table> typeToTable;

            public Database NullifyDefaultValues(Database db)
            {
                associationPartners = GetAssociationPairs(db);
                typeToTable = GetTablesByTypeName(db);
                return VisitDatabase(db);
            }

            private static void NullifyUpdateCheckOfColumns(Type type)
            {
                bool isVersion = false;
                foreach (Column column in type.Columns)
                {
                    if (column.IsVersion == true)
                    {
                        isVersion = true;
                        break;
                    }
                }

                if (isVersion)
                {
                    foreach (Column column in type.Columns)
                        if (column.UpdateCheck.HasValue && column.UpdateCheck.Value == UpdateCheck.Never)
                            column.UpdateCheck = null;
                }
                else
                {
                    foreach (Column column in type.Columns)
                    {
                        if (column.IsReadOnly == true)
                        {
                            if (column.UpdateCheck.HasValue && column.UpdateCheck.Value == UpdateCheck.Never)
                                column.UpdateCheck = null;
                        }
                        else if (column.UpdateCheck.HasValue && column.UpdateCheck.Value == UpdateCheck.Always)
                        {
                            column.UpdateCheck = null;
                        }
                    }
                }
            }

            private Table TableFromTypeName(string typeName)
            {
                Table table;
                typeToTable.TryGetValue(typeName, out table);
                return table;
            }

            private Type TypeFromTypeName(string typeName)
            {
                Table table = TableFromTypeName(typeName);
                return table == null ? null : table.Type;
            }

            public override Association VisitAssociation(Association association)
            {
                if (association == null)
                    return null;

                if (association.AccessModifier.HasValue && association.AccessModifier.Value == AccessModifier.Public)
                    association.AccessModifier = null;
                
                if (association.IsForeignKey == false)
                    association.IsForeignKey = null;

                if (association.Cardinality.HasValue && association.Cardinality.Value == Cardinality.One && association.IsForeignKey == true)
                    association.Cardinality = null;
                
                if (association.DeleteOnNull == false)
                    association.DeleteOnNull = null;
                
                if (association.Cardinality.HasValue && association.Cardinality.Value == Cardinality.Many && association.IsForeignKey != true)
                    association.Cardinality = null;
                
                if ((association.Member != null) && (association.Storage == ("_" + association.Member)))
                    association.Storage = null;
                
                if (association.IsForeignKey != true)
                {
                    Association otherAssociation = associationPartners[association];
                    if (otherAssociation != null)
                    {
                        Type type = TypeFromTypeName(otherAssociation.Type);
                        if ((type != null) && IsPrimaryKeyOfType(type, association.GetThisKey()))
                            association.SetThisKey(null);
                    }
                }

                if (association.IsForeignKey == true)
                {
                    Type type = TypeFromTypeName(association.Type);
                    if ((type != null) && IsPrimaryKeyOfType(type, association.GetOtherKey()))
                        association.SetOtherKey(null);
                }

                return association;
            }

            public override Column VisitColumn(Column column)
            {
                if (column == null)
                    return null;
                
                if (column.Storage == ("_" + column.Member))
                    column.Storage = null;
                
                if (column.Name == column.Member)
                    column.Member = null;
                
                if (column.AccessModifier.HasValue && column.AccessModifier.Value == AccessModifier.Public)
                    column.AccessModifier = null;

                if (column.AutoSync.HasValue && column.AutoSync == AutoSync.OnInsert 
                    && column.IsDbGenerated == true && column.IsPrimaryKey == true)
                    column.AutoSync = null;
                else if (column.AutoSync.HasValue && column.AutoSync == AutoSync.Always
                    && column.IsDbGenerated == true)
                    column.AutoSync = null;
                else if (column.AutoSync.HasValue && column.AutoSync == AutoSync.Never 
                    && column.IsVersion != true && column.IsDbGenerated != true)
                    column.AutoSync = null;

                if (column.IsReadOnly == false)
                    column.IsReadOnly = null;
                
                if (!string.IsNullOrEmpty(column.Expression) || (column.IsVersion == true))
                {
                    if (column.IsDbGenerated == true)
                        column.IsDbGenerated = null;
                }
                else if (column.IsDbGenerated == false)
                {
                    column.IsDbGenerated = null;
                }

                if (column.IsPrimaryKey == false)
                    column.IsPrimaryKey = null;

                if (column.IsDiscriminator == false)
                    column.IsDiscriminator = null;

                if (column.IsVersion == false)
                    column.IsVersion = null;

                if (column.IsDelayLoaded == false)
                    column.IsDelayLoaded = null;
                
                return column;
            }

            public override Connection VisitConnection(Connection connection)
            {
                return connection;
            }

            public override Database VisitDatabase(Database db)
            {
                if (db == null)
                    return null;
                
                if (db.Class == db.Name)
                    db.Class = null;
                
                if (db.BaseType == "System.Data.Linq.DataContext")
                    db.BaseType = null;

                if (db.AccessModifier.HasValue && db.AccessModifier.Value == AccessModifier.Public)
                    db.AccessModifier = null;

                if (db.ExternalMapping == false)
                    db.ExternalMapping = null;

                if (db.Serialization.HasValue && db.Serialization.Value == SerializationMode.None)
                    db.Serialization = null;
                
                return base.VisitDatabase(db);
            }

            public override Function VisitFunction(Function f)
            {
                if (f == null)
                    return null;

                if (f.Method == f.Name)
                    f.Method = null;

                if (f.AccessModifier.HasValue && f.AccessModifier.Value == AccessModifier.Public)
                    f.AccessModifier = null;
                
                bool? hasMultipleResults = f.HasMultipleResults;
                bool multiple = f.Types.Count > 1;

                if ((hasMultipleResults.GetValueOrDefault() == multiple) && hasMultipleResults.HasValue)
                    f.HasMultipleResults = null;

                if (f.IsComposable == false)
                    f.IsComposable = null;
                
                return base.VisitFunction(f);
            }

            public override Parameter VisitParameter(Parameter parameter)
            {
                if (parameter == null)
                    return null;

                if (parameter.ParameterName == parameter.Name)
                    parameter.ParameterName = null;

                if (parameter.Direction.HasValue && parameter.Direction.Value == ParameterDirection.In)
                    parameter.Direction = null;
                
                return parameter;
            }

            public override Return VisitReturn(Return r)
            {
                return r;
            }

            public override Table VisitTable(Table table)
            {
                if (table.Name == table.Member)
                    table.Member = null;

                if (table.AccessModifier.HasValue && table.AccessModifier.Value == AccessModifier.Public)
                    table.AccessModifier = null;
                
                return base.VisitTable(table);
            }

            public override TableFunction VisitTableFunction(TableFunction tf)
            {
                if (tf == null)
                    return null;

                if (tf.AccessModifier.HasValue && tf.AccessModifier.Value == AccessModifier.Private)
                    tf.AccessModifier = null;
                
                return base.VisitTableFunction(tf);
            }

            public override TableFunctionParameter VisitTableFunctionParameter(TableFunctionParameter parameter)
            {
                if (parameter == null)
                    return null;

                if (parameter.Version.HasValue && parameter.Version.Value == Version.Current)
                    parameter.Version = null;
                
                return parameter;
            }

            public override TableFunctionReturn VisitTableFunctionReturn(TableFunctionReturn r)
            {
                return r;
            }

            public override Type VisitType(Type type)
            {
                if (type == null)
                    return null;

                if (type.IsInheritanceDefault == false)
                    type.IsInheritanceDefault = null;

                if (type.AccessModifier.HasValue && type.AccessModifier.Value == AccessModifier.Public)
                    type.AccessModifier = null;
                
                NullifyUpdateCheckOfColumns(type);
                return base.VisitType(type);
            }
        }

        #endregion
    }
}