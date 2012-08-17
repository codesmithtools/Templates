using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace LinqToSqlShared.DbmlObjectModel
{
    public static partial class Dbml
    {
        #region Nested type: DbmlSerializer

        private class DbmlSerializer : DbmlVisitor
        {
            private readonly IdsCreater creater = new IdsCreater();
            private readonly List<Type> pocessedTypes = new List<Type>();
            private string currentTableFun;
            private Dictionary<Function, string> functionIds;
            private bool isFunctionElementType;
            private bool isSubType;
            private Dictionary<Type, string> typeIds;
            private XmlWriter writer;

            public void DbmlToFile(Database db, string filename, Encoding encoding)
            {
                var settings = new XmlWriterSettings {Indent = true, Encoding = encoding};
                writer = XmlWriter.Create(filename, settings);
                VisitDatabase(db);
                writer.Flush();
                writer.Close();
            }

            public string DbmlToString(Database db, Encoding encoding)
            {
                string str;
                using (var stream = new MemoryStream())
                using (var streamWriter = new StreamWriter(stream, encoding))
                {
                    var settings = new XmlWriterSettings {Indent = true, Encoding = encoding};
                    writer = XmlWriter.Create(streamWriter, settings);
                    VisitDatabase(db);
                    writer.Flush();
                    writer.Close();
                    using (var reader = new StreamReader(stream, encoding))
                    {
                        stream.Position = 0L;
                        str = reader.ReadToEnd();
                    }
                }
                return str;
            }

            private static string ToXmlBooleanString(bool? b)
            {
                return !b.HasValue ? null : b.ToString().ToLower(CultureInfo.InvariantCulture);
            }

            public override Association VisitAssociation(Association association)
            {
                if (association == null)
                    return null;
                
                writer.WriteStartElement("Association");
                
                if (association.Name != null)
                    writer.WriteAttributeString("Name", association.Name);
                
                if (association.Member != null)
                    writer.WriteAttributeString("Member", association.Member);
                
                if (association.Storage != null)
                    writer.WriteAttributeString("Storage", association.Storage);
                
                if (association.AccessModifier.HasValue)
                    writer.WriteAttributeString("AccessModifier", association.AccessModifier.ToString());
                
                if (association.Modifier.HasValue)
                    writer.WriteAttributeString("Modifier", association.Modifier.ToString());
                
                string thisKey = BuildKeyField(association.GetThisKey());
                if (!string.IsNullOrEmpty(thisKey))
                    writer.WriteAttributeString("ThisKey", thisKey);
                
                string otherKey = BuildKeyField(association.GetOtherKey());
                if (!string.IsNullOrEmpty(otherKey))
                    writer.WriteAttributeString("OtherKey", otherKey);
                
                if (association.Type != null)
                    writer.WriteAttributeString("Type", association.Type);
                
                if (association.IsForeignKey.HasValue)
                    writer.WriteAttributeString("IsForeignKey", ToXmlBooleanString(association.IsForeignKey));
                
                if (association.Cardinality.HasValue)
                    writer.WriteAttributeString("Cardinality", association.Cardinality.ToString());
                
                if (association.DeleteRule != null)
                    writer.WriteAttributeString("DeleteRule", association.DeleteRule);
                
                if (association.DeleteOnNull.HasValue)
                    writer.WriteAttributeString("DeleteOnNull", ToXmlBooleanString(association.DeleteOnNull));
                
                writer.WriteEndElement();
                return association;
            }

            public override Column VisitColumn(Column column)
            {
                if (column == null)
                    return null;
                
                writer.WriteStartElement("Column");
                
                if (column.Name != null)
                    writer.WriteAttributeString("Name", column.Name);
                
                if (column.Member != null)
                    writer.WriteAttributeString("Member", column.Member);
                
                if (column.Storage != null)
                    writer.WriteAttributeString("Storage", column.Storage);
                
                if (column.AccessModifier.HasValue)
                    writer.WriteAttributeString("AccessModifier", column.AccessModifier.ToString());
                
                if (column.Modifier.HasValue)
                    writer.WriteAttributeString("Modifier", column.Modifier.ToString());
                
                if (column.AutoSync.HasValue)
                    writer.WriteAttributeString("AutoSync", column.AutoSync.ToString());
                
                if (column.Type != null)
                    writer.WriteAttributeString("Type", column.Type);
                
                if (column.DbType != null)
                    writer.WriteAttributeString("DbType", column.DbType);
                
                if (column.IsReadOnly.HasValue)
                    writer.WriteAttributeString("IsReadOnly", ToXmlBooleanString(column.IsReadOnly));
                
                if (column.IsPrimaryKey.HasValue)
                    writer.WriteAttributeString("IsPrimaryKey", ToXmlBooleanString(column.IsPrimaryKey));
                
                if (column.IsDbGenerated.HasValue)
                    writer.WriteAttributeString("IsDbGenerated", ToXmlBooleanString(column.IsDbGenerated));
                
                if (column.CanBeNull.HasValue)
                    writer.WriteAttributeString("CanBeNull", ToXmlBooleanString(column.CanBeNull));
                
                if (column.UpdateCheck.HasValue)
                    writer.WriteAttributeString("UpdateCheck", column.UpdateCheck.ToString());
                
                if (column.IsDiscriminator.HasValue)
                    writer.WriteAttributeString("IsDiscriminator", ToXmlBooleanString(column.IsDiscriminator));
                
                if (column.Expression != null)
                    writer.WriteAttributeString("Expression", column.Expression);
                
                if (column.IsVersion.HasValue)
                    writer.WriteAttributeString("IsVersion", ToXmlBooleanString(column.IsVersion));
                
                if (column.IsDelayLoaded.HasValue)
                    writer.WriteAttributeString("IsDelayLoaded", ToXmlBooleanString(column.IsDelayLoaded));
                
                writer.WriteEndElement();
                return column;
            }

            public override Connection VisitConnection(Connection connection)
            {
                if (connection == null)
                    return null;
                
                writer.WriteStartElement("Connection");
                
                if (connection.Mode.HasValue)
                    writer.WriteAttributeString("Mode", connection.Mode.ToString());
                
                if (connection.ConnectionString != null)
                    writer.WriteAttributeString("ConnectionString", connection.ConnectionString);
                
                if (connection.SettingsObjectName != null)
                    writer.WriteAttributeString("SettingsObjectName", connection.SettingsObjectName);
                
                if (connection.SettingsPropertyName != null)
                    writer.WriteAttributeString("SettingsPropertyName", connection.SettingsPropertyName);
                
                if (connection.Provider != null)
                    writer.WriteAttributeString("Provider", connection.Provider);
                
                writer.WriteEndElement();
                return connection;
            }

            public override Database VisitDatabase(Database db)
            {
                if (db == null)
                    return null;
                
                creater.GetTypeIds(db, ref typeIds, ref functionIds);
                writer.WriteStartElement("Database", "http://schemas.microsoft.com/linqtosql/dbml/2007");
                
                if (db.Name != null)
                    writer.WriteAttributeString("Name", db.Name);
                
                if (db.EntityNamespace != null)
                    writer.WriteAttributeString("EntityNamespace", db.EntityNamespace);
                
                if (db.ContextNamespace != null)
                    writer.WriteAttributeString("ContextNamespace", db.ContextNamespace);
                
                if (db.Class != null)
                    writer.WriteAttributeString("Class", db.Class);
                
                if (db.AccessModifier.HasValue)
                    writer.WriteAttributeString("AccessModifier", db.AccessModifier.ToString());
                
                if (db.Modifier.HasValue)
                    writer.WriteAttributeString("Modifier", db.Modifier.ToString());
                
                if (db.BaseType != null)
                    writer.WriteAttributeString("BaseType", db.BaseType);
                
                if (db.Provider != null)
                    writer.WriteAttributeString("Provider", db.Provider);
                
                if (db.ExternalMapping.HasValue)
                    writer.WriteAttributeString("ExternalMapping", ToXmlBooleanString(db.ExternalMapping));
                
                if (db.Serialization.HasValue)
                    writer.WriteAttributeString("Serialization", db.Serialization.ToString());
                
                if (db.EntityBase != null)
                    writer.WriteAttributeString("EntityBase", db.EntityBase);
                
                base.VisitDatabase(db);
                writer.WriteEndElement();
                return db;
            }

            public override Function VisitFunction(Function f)
            {
                string str;
                if (f == null)
                    return null;
                
                writer.WriteStartElement("Function");
                
                functionIds.TryGetValue(f, out str);
                
                if (str != null)
                    writer.WriteAttributeString("Id", str);
                
                if (f.Name != null)
                    writer.WriteAttributeString("Name", f.Name);
                
                if (f.Method != null)
                    writer.WriteAttributeString("Method", f.Method);
                
                if (f.AccessModifier.HasValue)
                    writer.WriteAttributeString("AccessModifier", f.AccessModifier.ToString());
                
                if (f.Modifier.HasValue)
                    writer.WriteAttributeString("Modifier", f.Modifier.ToString());
                
                if (f.HasMultipleResults.HasValue)
                    writer.WriteAttributeString("HasMultipleResults", ToXmlBooleanString(f.HasMultipleResults));
                
                if (f.IsComposable.HasValue)
                    writer.WriteAttributeString("IsComposable", ToXmlBooleanString(f.IsComposable));
                
                isFunctionElementType = true;
                isSubType = false;
                base.VisitFunction(f);
                isFunctionElementType = false;
                writer.WriteEndElement();
                return f;
            }

            public override Parameter VisitParameter(Parameter parameter)
            {
                if (parameter == null)
                    return null;
                
                writer.WriteStartElement("Parameter");
                
                if (parameter.Name != null)
                    writer.WriteAttributeString("Name", parameter.Name);
                
                if ((parameter.ParameterName != null) && (parameter.ParameterName != parameter.Name))
                    writer.WriteAttributeString("Parameter", parameter.ParameterName);
                
                if (parameter.Type != null)
                    writer.WriteAttributeString("Type", parameter.Type);
                
                if (parameter.DbType != null)
                    writer.WriteAttributeString("DbType", parameter.DbType);
                
                if (parameter.Direction.HasValue)
                    writer.WriteAttributeString("Direction", parameter.Direction.ToString());
                
                writer.WriteEndElement();
                return parameter;
            }

            public override Return VisitReturn(Return r)
            {
                if (r == null)
                    return null;
                
                writer.WriteStartElement("Return");
                
                if (r.Type != null)
                    writer.WriteAttributeString("Type", r.Type);
                
                if (r.DbType != null)
                    writer.WriteAttributeString("DbType", r.DbType);
                
                writer.WriteEndElement();
                return r;
            }

            public override Table VisitTable(Table table)
            {
                if (table == null)
                    return null;
                
                writer.WriteStartElement("Table");
                
                if (table.Name != null)
                    writer.WriteAttributeString("Name", table.Name);
                
                if (table.Member != null)
                    writer.WriteAttributeString("Member", table.Member);
                
                if (table.AccessModifier.HasValue)
                    writer.WriteAttributeString("AccessModifier", table.AccessModifier.ToString());
                
                if (table.Modifier.HasValue)
                    writer.WriteAttributeString("Modifier", table.Modifier.ToString());
                
                isSubType = false;
                VisitType(table.Type);
                currentTableFun = "InsertFunction";
                VisitTableFunction(table.InsertFunction);
                currentTableFun = "UpdateFunction";
                VisitTableFunction(table.UpdateFunction);
                currentTableFun = "DeleteFunction";
                VisitTableFunction(table.DeleteFunction);
                writer.WriteEndElement();
                return table;
            }

            public override TableFunction VisitTableFunction(TableFunction tf)
            {
                string str;
                if (tf == null)
                    return null;
                
                writer.WriteStartElement(currentTableFun);
                functionIds.TryGetValue(tf.MappedFunction, out str);
                
                if (str != null)
                    writer.WriteAttributeString("FunctionId", str);
                
                if (tf.AccessModifier.HasValue)
                    writer.WriteAttributeString("AccessModifier", tf.AccessModifier.ToString());
                
                base.VisitTableFunction(tf);
                writer.WriteEndElement();
                return tf;
            }

            public override TableFunctionParameter VisitTableFunctionParameter(TableFunctionParameter parameter)
            {
                if (parameter == null)
                    return null;
                
                writer.WriteStartElement("Argument");
                if (parameter.ParameterName != null)
                    writer.WriteAttributeString("Parameter", parameter.ParameterName);
                
                if (parameter.Member != null)
                    writer.WriteAttributeString("Member", parameter.Member);
                
                if (parameter.Version.HasValue)
                    writer.WriteAttributeString("Version", parameter.Version.ToString());
                
                writer.WriteEndElement();
                return parameter;
            }

            public override TableFunctionReturn VisitTableFunctionReturn(TableFunctionReturn r)
            {
                if (r == null)
                    return null;
                
                writer.WriteStartElement("Return");
                if (r.Member != null)
                    writer.WriteAttributeString("Member", r.Member);
                
                writer.WriteEndElement();
                return r;
            }

            public override Type VisitType(Type type)
            {
                string localName = "";
                if (isFunctionElementType && !isSubType)
                    localName = "ElementType";
                else
                    localName = "Type";
                
                if (type == null)
                    return null;
                
                if ((typeIds[type] != null) && pocessedTypes.Contains(type))
                {
                    string str2 = typeIds[type];
                    writer.WriteStartElement(localName);
                    writer.WriteAttributeString("IdRef", str2);
                }
                else
                {
                    pocessedTypes.Add(type);
                    writer.WriteStartElement(localName);
                    if (type.Name != null)
                        writer.WriteAttributeString("Name", type.Name);
                    if (typeIds[type] != null)
                        writer.WriteAttributeString("Id", typeIds[type]);
                    if (type.InheritanceCode != null)
                        writer.WriteAttributeString("InheritanceCode", type.InheritanceCode);
                    if (type.IsInheritanceDefault.HasValue)
                        writer.WriteAttributeString("IsInheritanceDefault",
                                                    ToXmlBooleanString(type.IsInheritanceDefault));
                    if (type.AccessModifier.HasValue)
                        writer.WriteAttributeString("AccessModifier", type.AccessModifier.ToString());
                    if (type.Modifier.HasValue)
                        writer.WriteAttributeString("Modifier", type.Modifier.ToString());
                    
                    bool subType = isSubType;
                    isSubType = true;
                    base.VisitType(type);
                    isSubType = subType;
                }
                writer.WriteEndElement();
                return type;
            }

            #region Nested type: IdsCreater

            private class IdsCreater : DbmlVisitor
            {
                private readonly Dictionary<Function, string> functionIds = new Dictionary<Function, string>();
                private readonly Dictionary<Type, string> typeIds = new Dictionary<Type, string>();
                private int currentFunctionId = 1;
                private int currentTypeId = 1;

                public void GetTypeIds(Database db, ref Dictionary<Type, string> tyIds,
                                       ref Dictionary<Function, string> funIds)
                {
                    VisitDatabase(db);
                    tyIds = typeIds;
                    funIds = functionIds;
                }

                public override TableFunction VisitTableFunction(TableFunction tf)
                {
                    if (tf == null)
                        return null;
                    string str = "FunctionId" + currentFunctionId.ToString(CultureInfo.InvariantCulture);
                    if ((tf.MappedFunction != null) && !functionIds.ContainsKey(tf.MappedFunction))
                    {
                        functionIds.Add(tf.MappedFunction, str);
                        currentFunctionId++;
                    }
                    return tf;
                }

                public override Type VisitType(Type type)
                {
                    if (type == null)
                        return type;
                    if (typeIds.ContainsKey(type))
                    {
                        if (typeIds[type] == null)
                        {
                            typeIds[type] = "ID" + currentTypeId.ToString(CultureInfo.InvariantCulture);
                            currentTypeId++;
                        }
                    }
                    else
                        typeIds.Add(type, null);
                    return base.VisitType(type);
                }
            }

            #endregion
        }

        #endregion
    }
}