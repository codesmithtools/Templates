using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace LinqToSqlShared.DbmlObjectModel
{
    public static partial class Dbml
    {
        #region Nested type: DbmlReader

        private class DbmlReader
        {
            private readonly Dictionary<string, List<TableFunction>> cudFunctionIds =
                new Dictionary<string, List<TableFunction>>();

            private readonly Dictionary<string, Type> dbTypes = new Dictionary<string, Type>();
            private readonly List<string> typeNames = new List<string>();
            private Type currentTableType;
            private List<string> functionIds;
            private bool isTableType;

            private static readonly string[] _validDatabaseAttributes = new[] { "Name", "Class", "EntityNamespace", "ContextNamespace", "BaseType", "Provider", "AccessModifier", "Modifier", "ExternalMapping", "EntityBase", "Serialization" };
            private static readonly string[] _validTableAttributes = new[] { "Name", "Member", "AccessModifier", "Modifier" };
            private static readonly string[] _validTypeAttributes = new[] { "IdRef", "Name", "Id", "InheritanceCode", "IsInheritanceDefault", "AccessModifier", "Modifier" };
            private static readonly string[] _validColumnAttributes = new[] { "Name", "Type", "Member", "Storage", "AccessModifier", "Modifier", "AutoSync", "IsDbGenerated", "IsReadOnly", "IsPrimaryKey", "CanBeNull", "UpdateCheck", "Expression", "IsDiscriminator", "IsVersion", "IsDelayLoaded", "DbType" };
            
            private static readonly string[] _validFunctionAttributes = new[] { "Name", "Method", "Id", "AccessModifier", "Modifier", "HasMultipleResults", "IsComposable" };
            private static readonly string[] _validParameterAttributes = new[] { "Name", "Parameter", "Type", "DbType", "Direction" };
            private static readonly string[] _validReturnAttributes = new[] { "Type", "DbType" };
            private static readonly string[] _validTableFunctionAttributes = new[] { "FunctionId", "AccessModifier" };
            private static readonly string[] _validTableFunctionParameterAttributes = new[] { "Parameter", "Member", "Version" };
            private static readonly string[] _validTableFunctionReturnAttributes = new[] { "Member" };
         
            private static void AssertEmptyElement(XmlReader reader)
            {
                if (reader.IsEmptyElement)
                    return;

                string name = reader.Name;
                reader.Read();

                if (reader.NodeType != XmlNodeType.EndElement)
                    throw Error.SchemaExpectedEmptyElement(name, reader.NodeType, reader.Name);
            }

            public Database FileToDbml(string filename)
            {
                Database database = null;
                try
                {
                    using (var stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                    {
                        functionIds = GetAllFunctionIds(stream);
                        using (var reader = new XmlTextReader(stream))
                        {
                            while (reader.Read())
                            {
                                if (((reader.NodeType == XmlNodeType.Element)
                                     && (reader.Name == "Database")) && IsInNamespace(reader))
                                {
                                    database = ReadDatabase(reader);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(
                        string.Format("Error loading the dbml file '{0}'. Please verify the dbml file exists and is an xml file.", filename), 
                        ex);
                }

                if (database == null)
                    throw Error.DatabaseNodeNotFound("http://schemas.microsoft.com/linqtosql/dbml/2007");

                return database;
            }

            private static List<string> GetAllFunctionIds(Stream stream)
            {
                var reader = new XmlTextReader(stream);
                var list = new List<string>();
                while (reader.ReadToFollowing("Function"))
                {
                    string attribute = reader.GetAttribute("Id");
                    if (attribute != null)
                        list.Add(attribute);
                }
                stream.Position = 0L;
                return list;
            }

            private static bool HasInheritanceRelationship(Type root, Type child)
            {
                if (root == null)
                    return false;

                if (root == child)
                    return true;

                foreach (Type type in root.SubTypes)
                    if ((type == child) || HasInheritanceRelationship(type, child))
                        return true;

                return false;
            }

            private static bool HasOtherTypeAttributes(XmlTextReader reader)
            {
                return ((reader.GetAttribute("Name") != null) ||
                        ((reader.GetAttribute("InheritanceCode") != null) ||
                         ((reader.GetAttribute("IsInheritanceDefault") != null) ||
                          ((reader.GetAttribute("AccessModifier") != null) ||
                           ((reader.GetAttribute("Modifier") != null) || !reader.IsEmptyElement)))));
            }

            public static bool IsInNamespace(XmlReader reader)
            {
                return (reader.LookupNamespace(reader.Prefix) == "http://schemas.microsoft.com/linqtosql/dbml/2007");
            }

            private static Association ReadAssociation(XmlTextReader reader)
            {
                ValidateAttributes(reader,
                                   new[]
                                       {
                                           "Name", "Type", "Member", "Storage", "AccessModifier", "Modifier",
                                           "IsForeignKey", "Cardinality", "DeleteOnNull", "ThisKey", "OtherKey",
                                           "DeleteRule"
                                       });
                var association = new Association("");
                string attribute = reader.GetAttribute("Name");
                if (attribute == null)
                {
                    throw Error.RequiredAttributeMissingViolation("Name", reader.LineNumber);
                }
                if (reader.GetAttribute("Type") == null)
                {
                    throw Error.RequiredAttributeMissingViolation("Type", reader.LineNumber);
                }
                association.Name = attribute;
                association.Member = reader.GetAttribute("Member");
                if (reader.GetAttribute("Member") == null)
                {
                    throw Error.RequiredAttributeMissingViolation("Member", reader.LineNumber);
                }
                association.Storage = reader.GetAttribute("Storage");
                attribute = reader.GetAttribute("AccessModifier");
                try
                {
                    if (attribute == null)
                        association.AccessModifier = null;
                    else
                        association.AccessModifier =
                            (AccessModifier) Enum.Parse(typeof (AccessModifier), attribute, true);

                    attribute = reader.GetAttribute("Modifier");
                    if (attribute == null)
                        association.Modifier = null;
                    else
                        association.Modifier = (MemberModifier) Enum.Parse(typeof (MemberModifier), attribute, true);

                    attribute = reader.GetAttribute("IsForeignKey");
                    association.IsForeignKey = (attribute == null) ? null : new bool?(bool.Parse(attribute));

                    attribute = reader.GetAttribute("Cardinality");
                    if (attribute == null)
                        association.Cardinality = null;
                    else
                        association.Cardinality = (Cardinality) Enum.Parse(typeof (Cardinality), attribute, true);

                    attribute = reader.GetAttribute("DeleteOnNull");
                    association.DeleteOnNull = (attribute == null) ? null : new bool?(bool.Parse(attribute));
                }
                catch (FormatException)
                {
                    throw Error.InvalidBooleanAttributeValueViolation(attribute, reader.LineNumber);
                }
                catch (ArgumentException)
                {
                    throw Error.InvalidEnumAttributeValueViolation(attribute, reader.LineNumber);
                }
                association.SetThisKey(ParseKeyField(reader.GetAttribute("ThisKey")));
                association.Type = reader.GetAttribute("Type");
                association.SetOtherKey(ParseKeyField(reader.GetAttribute("OtherKey")));
                association.DeleteRule = reader.GetAttribute("DeleteRule");
                AssertEmptyElement(reader);
                return association;
            }

            private static Column ReadColumn(XmlTextReader reader)
            {
                ValidateAttributes(reader, _validColumnAttributes);

                var column = new Column("")
                                 {
                                     Name = reader.GetAttribute("Name"),
                                     Member = reader.GetAttribute("Member"),
                                     Storage = reader.GetAttribute("Storage")
                                 };

                if ((column.Name == null) && (column.Member == null))
                    throw Error.SchemaOrRequirementViolation("Column", "Name", "Member", reader.LineNumber);

                string attribute = reader.GetAttribute("AccessModifier");
                try
                {
                    if (attribute == null)
                        column.AccessModifier = null;
                    else
                        column.AccessModifier = (AccessModifier) Enum.Parse(typeof (AccessModifier), attribute, true);

                    attribute = reader.GetAttribute("Modifier");
                    if (attribute == null)
                        column.Modifier = null;
                    else
                        column.Modifier = (MemberModifier) Enum.Parse(typeof (MemberModifier), attribute, true);

                    attribute = reader.GetAttribute("AutoSync");
                    if (attribute == null)
                        column.AutoSync = null;
                    else
                        column.AutoSync = (AutoSync) Enum.Parse(typeof (AutoSync), attribute, true);

                    attribute = reader.GetAttribute("IsDbGenerated");
                    column.IsDbGenerated = (attribute == null) ? null : new bool?(bool.Parse(attribute));

                    attribute = reader.GetAttribute("IsReadOnly");
                    column.IsReadOnly = (attribute == null) ? null : new bool?(bool.Parse(attribute));

                    attribute = reader.GetAttribute("IsPrimaryKey");
                    column.IsPrimaryKey = (attribute == null) ? null : new bool?(bool.Parse(attribute));

                    attribute = reader.GetAttribute("CanBeNull");
                    column.CanBeNull = (attribute == null) ? null : new bool?(bool.Parse(attribute));

                    attribute = reader.GetAttribute("UpdateCheck");
                    if (attribute == null)
                        column.UpdateCheck = null;
                    else
                        column.UpdateCheck = (UpdateCheck) Enum.Parse(typeof (UpdateCheck), attribute, true);

                    attribute = reader.GetAttribute("IsDiscriminator");
                    column.IsDiscriminator = (attribute == null) ? null : new bool?(bool.Parse(attribute));
                    column.Expression = reader.GetAttribute("Expression");

                    attribute = reader.GetAttribute("IsVersion");
                    column.IsVersion = (attribute == null) ? null : new bool?(bool.Parse(attribute));

                    attribute = reader.GetAttribute("IsDelayLoaded");
                    column.IsDelayLoaded = (attribute == null) ? null : new bool?(bool.Parse(attribute));
                }
                catch (FormatException)
                {
                    throw Error.InvalidBooleanAttributeValueViolation(attribute, reader.LineNumber);
                }
                catch (ArgumentException)
                {
                    throw Error.InvalidEnumAttributeValueViolation(attribute, reader.LineNumber);
                }

                string typeAttribute = reader.GetAttribute("Type");
                if (typeAttribute == null)
                    throw Error.RequiredAttributeMissingViolation("Type", reader.LineNumber);

                column.Type = typeAttribute;
                column.DbType = reader.GetAttribute("DbType");
                AssertEmptyElement(reader);
                return column;
            }

            private static Connection ReadConnection(XmlTextReader reader)
            {
                ValidateAttributes(reader,
                                   new[]
                                       {
                                           "Provider", "Mode", "ConnectionString", "SettingsObjectName",
                                           "SettingsPropertyName"
                                       });

                var connection = new Connection(reader.GetAttribute("Provider"));
                string attribute = reader.GetAttribute("Mode");
                try
                {
                    if (attribute == null)
                        connection.Mode = null;
                    else
                        connection.Mode = (ConnectionMode) Enum.Parse(typeof (ConnectionMode), attribute);
                }
                catch (ArgumentException)
                {
                    throw Error.InvalidEnumAttributeValueViolation(attribute, reader.LineNumber);
                }
                connection.ConnectionString = reader.GetAttribute("ConnectionString");
                connection.SettingsObjectName = reader.GetAttribute("SettingsObjectName");
                connection.SettingsPropertyName = reader.GetAttribute("SettingsPropertyName");
                AssertEmptyElement(reader);
                return connection;
            }

            private Database ReadDatabase(XmlTextReader reader)
            {
                ValidateAttributes(reader, _validDatabaseAttributes);

                var database = new Database
                                   {
                                       Name = reader.GetAttribute("Name"),
                                       Class = reader.GetAttribute("Class")
                                   };

                if ((database.Name == null) && (database.Class == null))
                    throw Error.SchemaOrRequirementViolation("Database", "Name", "Class", reader.LineNumber);

                database.EntityNamespace = reader.GetAttribute("EntityNamespace");
                database.ContextNamespace = reader.GetAttribute("ContextNamespace");
                database.BaseType = reader.GetAttribute("BaseType");
                database.Provider = reader.GetAttribute("Provider");
                database.EntityBase = reader.GetAttribute("EntityBase");

                string attribute = reader.GetAttribute("AccessModifier");
                try
                {
                    if (attribute == null)
                        database.AccessModifier = null;
                    else
                        database.AccessModifier = (AccessModifier) Enum.Parse(typeof (AccessModifier), attribute, true);

                    attribute = reader.GetAttribute("Modifier");
                    if (attribute == null)
                        database.Modifier = null;
                    else
                        database.Modifier = (ClassModifier) Enum.Parse(typeof (ClassModifier), attribute, true);

                    attribute = reader.GetAttribute("ExternalMapping");
                    database.ExternalMapping = (attribute == null) ? null : new bool?(bool.Parse(attribute));

                    attribute = reader.GetAttribute("Serialization");
                    if (attribute == null)
                        database.Serialization = null;
                    else
                        database.Serialization =
                            (SerializationMode) Enum.Parse(typeof (SerializationMode), attribute, true);
                }
                catch (FormatException)
                {
                    throw Error.InvalidBooleanAttributeValueViolation(attribute, reader.LineNumber);
                }
                catch (ArgumentException)
                {
                    throw Error.InvalidEnumAttributeValueViolation(attribute, reader.LineNumber);
                }

                if (reader.IsEmptyElement)
                    return database;

                int elementCount = 0;
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Whitespace) || !IsInNamespace(reader))
                        continue;

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string name = reader.Name;

                        switch (name)
                        {
                            case "Table":
                                database.Tables.Add(ReadTable(reader));
                                break;
                            case "Function":
                                database.Functions.Add(ReadFunction(reader));
                                break;
                            case "Connection":
                                database.Connection = ReadConnection(reader);
                                elementCount++;
                                if (elementCount > 1)
                                    throw Error.ElementMoreThanOnceViolation("Connection", reader.LineNumber);
                                break;
                            default:
                                throw Error.SchemaUnexpectedElementViolation(reader.Name, "Database", reader.LineNumber);
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        return database;
                    }

                    continue;
                }
                return database;
            }

            private Function ReadFunction(XmlTextReader reader)
            {
                ValidateAttributes(reader, _validFunctionAttributes);

                var function = new Function("");
                currentTableType = null;
                string attribute = reader.GetAttribute("Name");
                if (attribute == null)
                    throw Error.RequiredAttributeMissingViolation("Name", reader.LineNumber);

                function.Name = attribute;
                function.Method = reader.GetAttribute("Method");

                string id = reader.GetAttribute("Id");
                if (!string.IsNullOrEmpty(id) && cudFunctionIds.ContainsKey(id))
                    foreach (TableFunction function2 in cudFunctionIds[id])
                        function2.MappedFunction = function;

                string convertAttribute = reader.GetAttribute("AccessModifier");
                try
                {
                    if (convertAttribute == null)
                        function.AccessModifier = null;
                    else
                        function.AccessModifier =
                            (AccessModifier) Enum.Parse(typeof (AccessModifier), convertAttribute, true);

                    convertAttribute = reader.GetAttribute("Modifier");
                    if (convertAttribute == null)
                        function.Modifier = null;
                    else
                        function.Modifier = (MemberModifier) Enum.Parse(typeof (MemberModifier), convertAttribute, true);

                    convertAttribute = reader.GetAttribute("HasMultipleResults");
                    function.HasMultipleResults = (convertAttribute == null)
                                                      ? null
                                                      : new bool?(bool.Parse(convertAttribute));

                    convertAttribute = reader.GetAttribute("IsComposable");
                    function.IsComposable = (convertAttribute == null) ? null : new bool?(bool.Parse(convertAttribute));
                }
                catch (FormatException)
                {
                    throw Error.InvalidBooleanAttributeValueViolation(convertAttribute, reader.LineNumber);
                }
                catch (ArgumentException)
                {
                    throw Error.InvalidEnumAttributeValueViolation(convertAttribute, reader.LineNumber);
                }

                if (reader.IsEmptyElement)
                    return function;

                int num = 0;
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Whitespace) || !IsInNamespace(reader))
                        continue;

                    if (reader.NodeType == XmlNodeType.EndElement)
                        return function;

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string name = reader.Name;
                        switch (name)
                        {
                            case "Parameter":
                                function.Parameters.Add(ReadParameter(reader));
                                continue;
                            case "ElementType":
                                function.Types.Add(ReadType(reader));
                                continue;
                            case "Return":
                                function.Return = ReadReturn(reader);
                                num++;
                                if (num > 1)
                                    throw Error.ElementMoreThanOnceViolation("Return", reader.LineNumber);

                                continue;
                            default:
                                throw Error.SchemaUnexpectedElementViolation(reader.Name, "Function", reader.LineNumber);
                        }
                    }
                }
                return function;
            }

            private static Parameter ReadParameter(XmlTextReader reader)
            {
                ValidateAttributes(reader, _validParameterAttributes);
                var parameter = new Parameter("", "");

                string attribute = reader.GetAttribute("Name");
                if (attribute == null)
                    throw Error.RequiredAttributeMissingViolation("Name", reader.LineNumber);
                parameter.Name = attribute;

                parameter.ParameterName = reader.GetAttribute("Parameter");

                string typeAttribute = reader.GetAttribute("Type");
                if (typeAttribute == null)
                    throw Error.RequiredAttributeMissingViolation("Type", reader.LineNumber);
                parameter.Type = typeAttribute;

                parameter.DbType = reader.GetAttribute("DbType");

                string dicection = reader.GetAttribute("Direction");
                try
                {
                    if (dicection == null)
                        parameter.Direction = null;
                    else
                        parameter.Direction =
                            (ParameterDirection) Enum.Parse(typeof (ParameterDirection), dicection, true);
                }
                catch (ArgumentException)
                {
                    throw Error.InvalidEnumAttributeValueViolation(dicection, reader.LineNumber);
                }
                AssertEmptyElement(reader);
                return parameter;
            }

            private static Return ReadReturn(XmlTextReader reader)
            {
                ValidateAttributes(reader, _validReturnAttributes);
                var r = new Return("");

                string attribute = reader.GetAttribute("Type");
                if (attribute == null)
                    throw Error.RequiredAttributeMissingViolation("Type", reader.LineNumber);

                r.Type = attribute;
                r.DbType = reader.GetAttribute("DbType");
                AssertEmptyElement(reader);
                return r;
            }

            private Table ReadTable(XmlTextReader reader)
            {
                ValidateAttributes(reader, _validTableAttributes);
                var table = new Table("", new Type(""));

                string attribute = reader.GetAttribute("Name");
                if (attribute == null)
                    throw Error.RequiredAttributeMissingViolation("Name", reader.LineNumber);

                table.Name = attribute;
                table.Member = reader.GetAttribute("Member");

                string convertAttribute = reader.GetAttribute("AccessModifier");
                try
                {
                    if (convertAttribute == null)
                        table.AccessModifier = null;
                    else
                        table.AccessModifier =
                            (AccessModifier) Enum.Parse(typeof (AccessModifier), convertAttribute, true);

                    convertAttribute = reader.GetAttribute("Modifier");
                    if (convertAttribute == null)
                        table.Modifier = null;
                    else
                        table.Modifier = (MemberModifier) Enum.Parse(typeof (MemberModifier), convertAttribute, true);
                }
                catch (ArgumentException)
                {
                    throw Error.InvalidEnumAttributeValueViolation(convertAttribute, reader.LineNumber);
                }

                if (reader.IsEmptyElement)
                    return table;

                int typeCount = 0;
                int insertCount = 0;
                int updateCount = 0;
                int deleteCount = 0;

                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Whitespace) || !IsInNamespace(reader))
                        continue;

                    XmlNodeType nodeType = reader.NodeType;
                    if (nodeType != XmlNodeType.Element)
                    {
                        if (nodeType == XmlNodeType.EndElement)
                        {
                            if (typeCount == 0)
                                throw Error.RequiredElementMissingViolation("Type", reader.LineNumber);

                            return table;
                        }
                        continue;
                    }

                    string name = reader.Name;
                    switch (name)
                    {
                        case "Type":
                            isTableType = true;
                            table.Type = ReadType(reader);
                            typeCount++;
                            if (typeCount > 1)
                                throw Error.ElementMoreThanOnceViolation("Type", reader.LineNumber);

                            continue;
                        case "InsertFunction":
                            table.InsertFunction = ReadTableFunction(reader);
                            insertCount++;
                            if (insertCount > 1)
                                throw Error.ElementMoreThanOnceViolation("InsertFunction", reader.LineNumber);

                            continue;
                        case "UpdateFunction":
                            table.UpdateFunction = ReadTableFunction(reader);
                            updateCount++;
                            if (updateCount > 1)
                                throw Error.ElementMoreThanOnceViolation("UpdateFunction", reader.LineNumber);

                            continue;
                        case "DeleteFunction":
                            table.DeleteFunction = ReadTableFunction(reader);
                            deleteCount++;
                            if (deleteCount > 1)
                                throw Error.ElementMoreThanOnceViolation("DeleteFunction", reader.LineNumber);

                            continue;
                        default:
                            throw Error.SchemaUnexpectedElementViolation(reader.Name, "Table", reader.LineNumber);
                    }
                }
                return table;
            }

            private TableFunction ReadTableFunction(XmlTextReader reader)
            {
                ValidateAttributes(reader, _validTableFunctionAttributes);
                var item = new TableFunction();

                string attribute = reader.GetAttribute("FunctionId");
                if (attribute == null)
                    throw Error.RequiredAttributeMissingViolation("FunctionId", reader.LineNumber);

                if (!functionIds.Contains(attribute))
                    throw Error.SchemaInvalidIdRefToNonexistentId("TableFunction", "FunctionId", attribute,
                                                                  reader.LineNumber);

                if (cudFunctionIds.ContainsKey(attribute))
                {
                    cudFunctionIds[attribute].Add(item);
                }
                else
                {
                    var list = new List<TableFunction> {item};
                    cudFunctionIds.Add(attribute, list);
                }

                string modifier = reader.GetAttribute("AccessModifier");
                try
                {
                    if (modifier == null)
                        item.AccessModifier = null;
                    else
                        item.AccessModifier = (AccessModifier) Enum.Parse(typeof (AccessModifier), modifier, true);
                }
                catch (FormatException)
                {
                    throw Error.InvalidBooleanAttributeValueViolation(modifier, reader.LineNumber);
                }
                catch (ArgumentException)
                {
                    throw Error.InvalidEnumAttributeValueViolation(modifier, reader.LineNumber);
                }

                if (reader.IsEmptyElement)
                    return item;

                int returnCount = 0;
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Whitespace) || !IsInNamespace(reader))
                        continue;

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string name = reader.Name;
                        switch (name)
                        {
                            case "Argument":
                                item.Arguments.Add(ReadTableFunctionParameter(reader));
                                continue;
                            case "Return":
                                item.Return = ReadTableFunctionReturn(reader);
                                returnCount++;
                                if (returnCount > 1)
                                    throw Error.ElementMoreThanOnceViolation("Return", reader.LineNumber);

                                continue;
                            default:
                                throw Error.SchemaUnexpectedElementViolation(reader.Name, "TableFunction",
                                                                             reader.LineNumber);
                        }
                    }

                    if (reader.NodeType == XmlNodeType.EndElement)
                        return item;
                }
                return item;
            }

            private static TableFunctionParameter ReadTableFunctionParameter(XmlTextReader reader)
            {
                ValidateAttributes(reader, _validTableFunctionParameterAttributes);
                var parameter = new TableFunctionParameter("", "");

                string attribute = reader.GetAttribute("Parameter");
                if (attribute == null)
                    throw Error.RequiredAttributeMissingViolation("Parameter", reader.LineNumber);

                parameter.ParameterName = attribute;

                attribute = reader.GetAttribute("Member");
                if (attribute == null)
                    throw Error.RequiredAttributeMissingViolation("Member", reader.LineNumber);

                parameter.Member = attribute;
                attribute = reader.GetAttribute("Version");
                try
                {
                    if (attribute == null)
                        parameter.Version = null;
                    else
                        parameter.Version = (Version) Enum.Parse(typeof (Version), attribute, true);
                }
                catch (ArgumentException)
                {
                    throw Error.InvalidEnumAttributeValueViolation(attribute, reader.LineNumber);
                }
                AssertEmptyElement(reader);
                return parameter;
            }

            private static TableFunctionReturn ReadTableFunctionReturn(XmlTextReader reader)
            {
                ValidateAttributes(reader, _validTableFunctionReturnAttributes);
                var r = new TableFunctionReturn();
                string attribute = reader.GetAttribute("Member");
                if (attribute == null)
                    throw Error.RequiredAttributeMissingViolation("Member", reader.LineNumber);

                r.Member = attribute;
                AssertEmptyElement(reader);
                return r;
            }

            private Type ReadType(XmlTextReader reader)
            {
                ValidateAttributes(reader, _validTypeAttributes);

                string attribute = reader.GetAttribute("IdRef");
                if (attribute != null)
                {
                    if (HasOtherTypeAttributes(reader))
                        throw Error.SchemaUnexpectedAdditionalAttributeViolation("IdRef", "Type", reader.LineNumber);

                    if (!dbTypes.ContainsKey(attribute))
                        throw Error.SchemaInvalidIdRefToNonexistentId("Type", "IdRef", attribute, reader.LineNumber);

                    Type child = dbTypes[attribute];
                    if (!isTableType && HasInheritanceRelationship(currentTableType, child))
                        throw Error.SchemaRecursiveTypeReference(attribute, child.Name, reader.LineNumber);

                    return child;
                }

                var type = new Type("");
                if (isTableType)
                {
                    currentTableType = type;
                    isTableType = false;
                }

                string item = reader.GetAttribute("Name");
                if (item == null)
                    throw Error.RequiredAttributeMissingViolation("Name", reader.LineNumber);

                if (typeNames.Contains(item))
                    throw Error.TypeNameNotUnique(item, reader.LineNumber);

                typeNames.Add(item);
                type.Name = item;

                string key = reader.GetAttribute("Id");
                if (!string.IsNullOrEmpty(key))
                {
                    if (dbTypes.ContainsKey(key))
                        throw Error.SchemaDuplicateIdViolation("IdRef", key, reader.LineNumber);

                    dbTypes.Add(key, type);
                }

                type.InheritanceCode = reader.GetAttribute("InheritanceCode");
                string convert = reader.GetAttribute("IsInheritanceDefault");
                try
                {
                    type.IsInheritanceDefault = (convert == null) ? null : new bool?(bool.Parse(convert));

                    convert = reader.GetAttribute("AccessModifier");
                    if (convert == null)
                        type.AccessModifier = null;
                    else
                        type.AccessModifier = (AccessModifier) Enum.Parse(typeof (AccessModifier), convert, true);

                    convert = reader.GetAttribute("Modifier");
                    if (convert == null)
                        type.Modifier = null;
                    else
                        type.Modifier = (ClassModifier) Enum.Parse(typeof (ClassModifier), convert, true);
                }
                catch (FormatException)
                {
                    throw Error.InvalidBooleanAttributeValueViolation(convert, reader.LineNumber);
                }
                catch (ArgumentException)
                {
                    throw Error.InvalidEnumAttributeValueViolation(convert, reader.LineNumber);
                }

                if (reader.IsEmptyElement)
                    return type;

                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Whitespace) || !IsInNamespace(reader))
                        continue;

                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            string name = reader.Name;
                            switch (name)
                            {
                                case "Column":
                                    type.Columns.Add(ReadColumn(reader));
                                    continue;
                                case "Association":
                                    type.Associations.Add(ReadAssociation(reader));
                                    continue;
                                case "Type":
                                    type.SubTypes.Add(ReadType(reader));
                                    continue;
                                default:
                                    throw Error.SchemaUnexpectedElementViolation(reader.Name, "Type", reader.LineNumber);
                            }
                        case XmlNodeType.EndElement:
                            return type;
                    }
                } // while

                return type;
            }

            public Database StreamToDbml(Stream stream)
            {
                functionIds = GetAllFunctionIds(stream);
                Database database = null;

                using (var reader = new XmlTextReader(stream))
                {
                    while (reader.Read())
                    {
                        if (((reader.NodeType == XmlNodeType.Element)
                             && (reader.Name == "Database")) && IsInNamespace(reader))
                        {
                            database = ReadDatabase(reader);
                        }
                    }
                }

                if (database == null)
                    throw Error.DatabaseNodeNotFound("http://schemas.microsoft.com/linqtosql/dbml/2007");

                return database;
            }

            public static void ValidateAttributes(XmlTextReader reader, string[] validAttributes)
            {
                if (!reader.HasAttributes)
                    return;

                var list = new List<string>(validAttributes);
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);
                    if (((reader.LocalName != "xmlns") && IsInNamespace(reader)) && !list.Contains(reader.LocalName))
                    {
                        throw Error.SchemaUnrecognizedAttribute(
                            string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                                          new object[]
                                              {
                                                  reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : ":",
                                                  reader.LocalName
                                              }), reader.LineNumber);
                    }
                }
                reader.MoveToElement();
            }
        }

        #endregion
    }
}