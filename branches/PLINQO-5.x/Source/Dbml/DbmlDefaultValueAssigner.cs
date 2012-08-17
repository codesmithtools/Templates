using System.Collections.Generic;

namespace LinqToSqlShared.DbmlObjectModel
{
    public static partial class Dbml
    {
        #region Nested type: DbmlDefaultValueAssigner

        private class DbmlDefaultValueAssigner : DbmlVisitor
        {
            private Dictionary<Association, Association> associationPartners;
            private Dictionary<string, Table> typeToTable;

            public Database AssignDefaultValues(Database db)
            {
                associationPartners = GetAssociationPairs(db);
                return VisitDatabase(db);
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

                if (!association.AccessModifier.HasValue)
                    association.AccessModifier = 0;
                
                if (!association.IsForeignKey.HasValue)
                    association.IsForeignKey = false;
                
                if (!association.Cardinality.HasValue)
                    association.Cardinality = association.IsForeignKey == true ? 0 : (Cardinality) 1;
                
                if (!association.DeleteOnNull.HasValue)
                    association.DeleteOnNull = false;
                
                if (association.Storage == null)
                    association.Storage = "_" + association.Member;
                
                if ((association.GetThisKey().Length == 0) && (association.IsForeignKey != true))
                {
                    Association other = associationPartners[association];
                    if (other != null)
                    {
                        Type type = TypeFromTypeName(other.Type);
                        if (type != null)
                            association.SetThisKey(GetPrimaryKeys(type));
                    }
                }
                
                if ((association.GetOtherKey().Length == 0) && (association.IsForeignKey == true))
                {
                    Type type2 = TypeFromTypeName(association.Type);
                    if (type2 != null)
                        association.SetOtherKey(GetPrimaryKeys(type2));
                }
                return association;
            }

            public override Column VisitColumn(Column column)
            {
                if (column == null)
                    return null;
                
                if ((column.Name == null) && (column.Member != null))
                    column.Name = column.Member;
                
                if ((column.Member == null) && (column.Name != null))
                    column.Member = column.Name;
                
                if (column.Storage == null)
                    column.Storage = "_" + column.Member;
                
                if (!column.AccessModifier.HasValue)
                    column.AccessModifier = 0;
                
                if (!column.IsPrimaryKey.HasValue)
                    column.IsPrimaryKey = false;
                
                if (!column.IsDiscriminator.HasValue)
                    column.IsDiscriminator = false;
                
                if (!column.IsVersion.HasValue)
                    column.IsVersion = false;
                
                if (!column.IsDelayLoaded.HasValue)
                    column.IsDelayLoaded = false;
                
                if (!column.IsDbGenerated.HasValue)
                    column.IsDbGenerated = !string.IsNullOrEmpty(column.Expression) || (column.IsVersion == true);
                
                if (!column.IsReadOnly.HasValue)
                    column.IsReadOnly = false;
                
                if (!column.AutoSync.HasValue)
                {
                    if ((column.IsDbGenerated == true) && (column.IsPrimaryKey == true))
                        column.AutoSync = (AutoSync) 3;
                    else if (column.IsDbGenerated == true)
                        column.AutoSync = (AutoSync) 1;
                    else
                        column.AutoSync = (AutoSync) 2;
                }
                
                if (column.IsReadOnly == true)
                    column.UpdateCheck = (UpdateCheck) 1;
                
                return column;
            }

            public override Connection VisitConnection(Connection connection)
            {
                if (connection == null)
                    return null;
                
                if (!connection.Mode.HasValue)
                    connection.Mode = 0;
                
                return connection;
            }

            public override Database VisitDatabase(Database db)
            {
                if (db == null)
                    return null;
                
                if ((db.Class == null) && (db.Name == null))
                    db.Class = db.Connection != null ? db.Connection.ConnectionString : "Context";
                else if (db.Class == null)
                    db.Class = db.Name;

                if (!db.AccessModifier.HasValue)
                    db.AccessModifier = 0;
                
                if (db.BaseType == null)
                    db.BaseType = "System.Data.Linq.DataContext";
                
                if (!db.ExternalMapping.HasValue)
                    db.ExternalMapping = false;
                
                if (!db.Serialization.HasValue)
                    db.Serialization = 0;
                
                typeToTable = GetTablesByTypeName(db);
                return base.VisitDatabase(db);
            }

            public override Function VisitFunction(Function f)
            {
                if (f == null)
                    return null;
                
                if (f.Method == null)
                    f.Method = f.Name;
                
                if (!f.AccessModifier.HasValue)
                    f.AccessModifier = 0;
                
                if (!f.HasMultipleResults.HasValue)
                    f.HasMultipleResults = f.Types.Count > 1;
                
                if (!f.IsComposable.HasValue)
                    f.IsComposable = false;
                
                return base.VisitFunction(f);
            }

            public override Parameter VisitParameter(Parameter parameter)
            {
                if (parameter == null)
                    return null;
                
                if (parameter.ParameterName == null)
                    parameter.ParameterName = parameter.Name;
                
                if (!parameter.Direction.HasValue)
                    parameter.Direction = 0;
                
                return parameter;
            }

            public override Return VisitReturn(Return r)
            {
                return r;
            }

            public override Table VisitTable(Table table)
            {
                if (table == null)
                    return null;

                if ((table.Name == null) && (table.Member != null))
                    table.Name = table.Member;

                if ((table.Member == null) && (table.Name != null))
                    table.Member = table.Name;

                if (!table.AccessModifier.HasValue)
                    table.AccessModifier = 0;
                
                return base.VisitTable(table);
            }

            public override TableFunction VisitTableFunction(TableFunction tf)
            {
                if (tf == null)
                    return null;
                
                if (!tf.AccessModifier.HasValue)
                    tf.AccessModifier = (AccessModifier) 4;
                
                VisitFunction(tf.MappedFunction);
                return base.VisitTableFunction(tf);
            }

            public override TableFunctionParameter VisitTableFunctionParameter(TableFunctionParameter parameter)
            {
                if (parameter == null)
                    return null;
                
                if (!parameter.Version.HasValue)
                    parameter.Version = 0;
                
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

                if (!type.IsInheritanceDefault.HasValue)
                    type.IsInheritanceDefault = false;

                if (!type.AccessModifier.HasValue)
                    type.AccessModifier = 0;
                
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
                        if (!column.UpdateCheck.HasValue)
                            column.UpdateCheck = (UpdateCheck) 1;
                }
                else
                {
                    foreach (Column column in type.Columns)
                        if (!column.UpdateCheck.HasValue)
                            column.UpdateCheck = 0;
                }

                foreach (Column column in type.Columns)
                    VisitColumn(column);

                foreach (Association association in type.Associations)
                    VisitAssociation(association);

                foreach (Type subType in type.SubTypes)
                    VisitType(subType);
                
                return type;
            }
        }

        #endregion
    }
}