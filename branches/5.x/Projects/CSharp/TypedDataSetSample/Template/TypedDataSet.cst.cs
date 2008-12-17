using System;
using System.Text;
using System.ComponentModel;
using CodeSmith.Engine;
using System.Data;
using SchemaExplorer;
using System.Collections.Generic;

namespace CodeSmith.BaseTemplates
{
	public class SqlCodeTemplate : CodeTemplate
	{
		
		public string GetCamelCaseName(string value)
		{
			return value.Substring(0, 1).ToLower() + value.Substring(1);
		}
		
		public string GetSpacedName(string value)
		{
			StringBuilder spacedName = new StringBuilder();
			
			for (int i = 0; i < value.Length; i++)
			{
				if (i > 0 && i < value.Length - 1 && value.Substring(i, 1).ToUpper() == value.Substring(i, 1))
				{
					spacedName.Append(" ");
				}
				spacedName.Append(value[i]);
			}
			
			return spacedName.ToString();
		}
		
		public string GetClassName(string value)
		{
			return value.Replace(" ", "");
		}
		
		
		public string GetMemberVariableName(string value)
		{
			string memberVariableName = "_" + GetCamelCaseName(value);
			
			return memberVariableName;
		}
		
		public bool SizeMatters(ColumnSchema column)
		{
			switch (column.DataType)
			{
				case DbType.String:
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.Decimal:
				{
					return true;
				}
				default:
				{
					return false;
				}
			}
		}
		
		public bool PrecisionMatters(ColumnSchema column)
		{
			switch (column.DataType)
			{
				case DbType.Decimal:
				{
					return true;
				}
				default:
				{
					return false;
				}
			}
		}
		
		
		public string GetSqlReaderAssignmentStatement(ColumnSchema column, int index)
		{
			string statement = "if (!reader.IsDBNull(" + index.ToString() + ")) ";
			statement += GetMemberVariableName(column.Name) + " = ";
			
			if (column.Name.EndsWith("TypeCode")) statement += "(" + column.Name + ")";
			
			statement += "reader." + GetReaderMethod(column) + "(" + index.ToString() + ");";
			
			return statement;
		}
		
		public string GetValidateStatements(TableSchema table, string statementPrefix)
		{
			string statements = "";
			
			foreach (ColumnSchema column in table.Columns)
			{
				if (IncludeEmptyCheck(column))
				{
					statements += "\r\n" + statementPrefix + "if (" + GetMemberVariableName(column.Name) + " == " + GetMemberVariableDefaultValue(column) + ") this.ValidationErrors.Add(new ValidationError(ValidationTypeCode.Required, \"" + table.Name + "\", \"" + column.Name + "\", \"" + column.Name + " is required.\"));";
				}
				if (IncludeMaxLengthCheck(column))
				{
					statements += "\r\n" + statementPrefix + "if (" + GetMemberVariableName(column.Name) + ".Length > " + column.Size.ToString() + ") this.ValidationErrors.Add(new ValidationError(ValidationTypeCode.MaxLength, \"" + table.Name + "\", \"" + column.Name + "\", \"" + column.Name + " is too long.\"));";
				}
			}
			
			return statements.Substring(statementPrefix.Length + 2);
		}
		
		public string GetPropertyName(ColumnSchema column)
		{
			string propertyName = column.Name;
			
			if (propertyName == column.Table.Name + "Name") return "Name";
			if (propertyName == column.Table.Name + "Description") return "Description";
			
			if (propertyName.EndsWith("TypeCode")) propertyName = propertyName.Substring(0, propertyName.Length - 4);
			
			return propertyName;
		}
		
		
		
		public string GetReaderMethod(ColumnSchema column)
		{
			switch (column.DataType)
			{
				case DbType.Byte:
				{
					return "GetByte";
				}
				case DbType.Int16:
				{
					return "GetInt16";
				}
				case DbType.Int32:
				{
					return "GetInt32";
				}
				case DbType.Int64:
				{
					return "GetInt64";
				}
				case DbType.AnsiStringFixedLength:
				case DbType.AnsiString:
				case DbType.String:
				case DbType.StringFixedLength:
				{
					return "GetString";
				}
				case DbType.Boolean:
				{
					return "GetBoolean";
				}
				case DbType.Guid:
				{
					return "GetGuid";
				}
				case DbType.Currency:
				case DbType.Decimal:
				{
					return "GetDecimal";
				}
				case DbType.DateTime:
				case DbType.Date:
				{
					return "GetDateTime";
				}
				default:
				{
					return "__SQL__" + column.DataType;
				}
			}
		}
		
		
		
		public string GetMemberVariableDefaultValue(ColumnSchema column)
		{
			switch (column.DataType)
			{
				case DbType.Guid:
				{
					return "Guid.Empty";
				}
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
				{
					return "String.Empty";
				}
				default:
				{
					return "";
				}
			}
		}
		
		public bool IncludeMaxLengthCheck(ColumnSchema column)
		{
			switch (column.DataType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
				{
					return true;
				}
				default:
				{
					return false;
				}
			}
		}
		
		public bool IncludeEmptyCheck(ColumnSchema column)
		{
			if (column.IsPrimaryKeyMember || column.AllowDBNull || column.Name.EndsWith("TypeCode")) return false;
	
			switch (column.DataType)
			{
				case DbType.Guid:
				{
					return true;
				}
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
				{
					return true;
				}
				default:
				{
					return false;
				}
			}
		}
		
		public string GetSqlParameterStatement(ColumnSchema column)
		{
			return GetSqlParameterStatement(column, false);
		}
		
		public string GetSqlParameterStatement(ColumnSchema column, bool isOutput)
		{
			string param = "@" + column.Name + " " + column.NativeType;
			
			switch (column.DataType)
			{
				case DbType.Decimal:
				{
					param += "(" + column.Precision + ", " + column.Scale + ")";
					break;
				}
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
				{
					if (column.Size > 0)
					{
						param += "(" + column.Size + ")";
					}
					break;
				}
			}
			
			if (isOutput)
			{
				param += " OUTPUT";
			}
			
			return param;
		}

        #region SearchCriteria Class

        public class SearchCriteria
        {
            #region Static Content

            public static List<SearchCriteria> GetAllSearchCriteria(TableSchema table, string extendedProperty)
            {
                TableSearchCriteria tsc = new TableSearchCriteria(table, extendedProperty);
                return tsc.GetAllSearchCriteria();
            }
            public static List<SearchCriteria> GetAllSearchCriteria(TableSchema table)
            {
                TableSearchCriteria tsc = new TableSearchCriteria(table);
                return tsc.GetAllSearchCriteria();
            }

            public static List<SearchCriteria> GetPrimaryKeySearchCriteria(TableSchema table, string extendedProperty)
            {
                TableSearchCriteria tsc = new TableSearchCriteria(table, extendedProperty);
                return tsc.GetPrimaryKeySearchCriteria();
            }
            public static List<SearchCriteria> GetPrimaryKeySearchCriteria(TableSchema table)
            {
                TableSearchCriteria tsc = new TableSearchCriteria(table);
                return tsc.GetPrimaryKeySearchCriteria();
            }

            public static List<SearchCriteria> GetForeignKeySearchCriteria(TableSchema table, string extendedProperty)
            {
                TableSearchCriteria tsc = new TableSearchCriteria(table, extendedProperty);
                return tsc.GetForeignKeySearchCriteria();
            }
            public static List<SearchCriteria> GetForeignKeySearchCriteria(TableSchema table)
            {
                TableSearchCriteria tsc = new TableSearchCriteria(table);
                return tsc.GetForeignKeySearchCriteria();
            }

            public static List<SearchCriteria> GetIndexSearchCriteria(TableSchema table, string extendedProperty)
            {
                TableSearchCriteria tsc = new TableSearchCriteria(table, extendedProperty);
                return tsc.GetIndexSearchCriteria();
            }
            public static List<SearchCriteria> GetIndexSearchCriteria(TableSchema table)
            {
                TableSearchCriteria tsc = new TableSearchCriteria(table);
                return tsc.GetIndexSearchCriteria();
            }

            #endregion

            #region Declarations

            protected List<MemberColumnSchema> mcsList;
            protected MethodNameGenerationMode methodNameGenerationMode = MethodNameGenerationMode.Default;
            protected string methodName = String.Empty;

            #endregion

            #region Constructors

            protected SearchCriteria()
            {
                mcsList = new List<MemberColumnSchema>();
            }
            protected SearchCriteria(List<MemberColumnSchema> mcsList)
            {
                this.mcsList = mcsList;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Sets MethodName to default generation: "GetBy{0}{1}{n}"
            /// </summary>
            public void SetMethodNameGeneration()
            {
                methodNameGenerationMode = MethodNameGenerationMode.Default;

                GenerateMethodName("GetBy", String.Empty, String.Empty);
            }
            /// <summary>
            /// Sets MethodName to be value of the specified Extended Property from the database.
            /// </summary>
            /// <param name="extendedProperty">Value of the Extended Property.</param>
            public void SetMethodNameGeneration(string extendedProperty)
            {
                methodNameGenerationMode = MethodNameGenerationMode.ExtendedProperty;

                methodName = extendedProperty;
            }
            /// <summary>
            /// Sets MethodName to custom generation: "{prefix}{0}{delimeter}{1}{suffix}"
            /// </summary>
            /// <param name="prefix">Method Prefix</param>
            /// <param name="delimeter">Column Delimeter</param>
            /// <param name="suffix">Method Suffix</param>
            public void SetMethodNameGeneration(string prefix, string delimeter, string suffix)
            {
                methodNameGenerationMode = MethodNameGenerationMode.Custom;

                GenerateMethodName(prefix, delimeter, suffix);
            }

            public override string ToString()
            {
                if (String.IsNullOrEmpty(methodName))
                    SetMethodNameGeneration();

                return methodName;
            }

            protected void Add(MemberColumnSchema item)
            {
                mcsList.Add(item);
            }
            protected void GenerateMethodName(string prefix, string delimeter, string suffix)
            {
                StringBuilder sb = new StringBuilder();
                bool isFirst = true;

                sb.Append(prefix);
                foreach (MemberColumnSchema mcs in mcsList)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        sb.Append(delimeter);
                    sb.Append(mcs.Name);
                }
                sb.Append(suffix);

                methodName = sb.ToString();
            }

            #endregion

            #region Properties

            public List<MemberColumnSchema> Items
            {
                get { return mcsList; }
            }
            public bool IsAllPrimaryKeys
            {
                get
                {
                    bool result = true;
                    foreach (MemberColumnSchema msc in mcsList)
                        if (!msc.IsPrimaryKeyMember)
                        {
                            result = false;
                            break;
                        }
                    return result;
                }
            }
            public string MethodName
            {
                get { return this.ToString(); }
            }
            public MethodNameGenerationMode MethodNameGeneration
            {
                get { return methodNameGenerationMode; }
            }

            protected string Key
            {
                get
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (MemberColumnSchema mcs in mcsList)
                        sb.Append(mcs.Name);

                    return sb.ToString();
                }
            }

            #endregion

            #region Enums & Classes

            public enum MethodNameGenerationMode
            {
                Default,
                ExtendedProperty,
                Custom
            }

            internal class TableSearchCriteria
            {
                #region Declarations

                protected TableSchema table;
                protected string extendedProperty = "cs_CriteriaName";

                #endregion

                #region Constructor

                public TableSearchCriteria(TableSchema sourceTable)
                {
                    this.table = sourceTable;
                }
                public TableSearchCriteria(TableSchema sourceTable, string extendedProperty)
                    : this(sourceTable)
                {
                    this.extendedProperty = extendedProperty;
                }

                #endregion

                #region Methods

                public List<SearchCriteria> GetAllSearchCriteria()
                {
                    Dictionary<string, SearchCriteria> map = new Dictionary<string, SearchCriteria>();

                    GetPrimaryKeySearchCriteria(map);
                    GetForeignKeySearchCriteria(map);
                    GetIndexSearchCriteria(map);

                    return GetResultsFromMap(map);
                }
                public List<SearchCriteria> GetPrimaryKeySearchCriteria()
                {
                    Dictionary<string, SearchCriteria> map = new Dictionary<string, SearchCriteria>();

                    GetPrimaryKeySearchCriteria(map);

                    return GetResultsFromMap(map);
                }
                public List<SearchCriteria> GetForeignKeySearchCriteria()
                {
                    Dictionary<string, SearchCriteria> map = new Dictionary<string, SearchCriteria>();

                    GetForeignKeySearchCriteria(map);

                    return GetResultsFromMap(map);
                }
                public List<SearchCriteria> GetIndexSearchCriteria()
                {
                    Dictionary<string, SearchCriteria> map = new Dictionary<string, SearchCriteria>();

                    GetIndexSearchCriteria(map);

                    return GetResultsFromMap(map);
                }

                protected void GetPrimaryKeySearchCriteria(Dictionary<string, SearchCriteria> map)
                {
                    List<MemberColumnSchema> mcsList = new List<MemberColumnSchema>(table.PrimaryKey.MemberColumns.ToArray());
                    SearchCriteria searchCriteria = new SearchCriteria(mcsList);

                    if (table.PrimaryKey.ExtendedProperties.Contains(extendedProperty))
                        if (!String.IsNullOrEmpty(extendedProperty) && table.PrimaryKey.ExtendedProperties.Contains(extendedProperty) && table.PrimaryKey.ExtendedProperties[extendedProperty].Value != null)
                            searchCriteria.SetMethodNameGeneration(table.PrimaryKey.ExtendedProperties[extendedProperty].Value.ToString());

                    AddToMap(map, searchCriteria);
                }
                protected void GetForeignKeySearchCriteria(Dictionary<string, SearchCriteria> map)
                {
                    foreach (TableKeySchema tks in table.ForeignKeys)
                    {
                        SearchCriteria searchCriteria = new SearchCriteria();
                        foreach (MemberColumnSchema mcs in tks.ForeignKeyMemberColumns)
                            if (mcs.Table.Equals(table))
                                searchCriteria.Add(mcs);

                        if (!String.IsNullOrEmpty(extendedProperty) && tks.ExtendedProperties.Contains(extendedProperty) && tks.ExtendedProperties[extendedProperty].Value != null)
                            searchCriteria.SetMethodNameGeneration(tks.ExtendedProperties[extendedProperty].Value.ToString());

                        AddToMap(map, searchCriteria);
                    }
                }
                protected void GetIndexSearchCriteria(Dictionary<string, SearchCriteria> map)
                {
                    foreach (IndexSchema indexSchema in table.Indexes)
                    {
                        SearchCriteria searchCriteria = new SearchCriteria();
                        foreach (MemberColumnSchema mcs in indexSchema.MemberColumns)
                            if (mcs.Table.Equals(table))
                                searchCriteria.Add(mcs);

                        if (!String.IsNullOrEmpty(extendedProperty) && indexSchema.ExtendedProperties.Contains(extendedProperty) && indexSchema.ExtendedProperties[extendedProperty].Value != null)
                            searchCriteria.SetMethodNameGeneration(indexSchema.ExtendedProperties[extendedProperty].Value.ToString());

                        AddToMap(map, searchCriteria);
                    }
                }

                protected bool AddToMap(Dictionary<string, SearchCriteria> map, SearchCriteria searchCriteria)
                {
                    string key = searchCriteria.Key;
                    bool result = (searchCriteria.Items.Count > 0 && !map.ContainsKey(key));

                    if (result)
                        map.Add(key, searchCriteria);

                    return result;
                }
                protected List<SearchCriteria> GetResultsFromMap(Dictionary<string, SearchCriteria> map)
                {
                    List<SearchCriteria> result = new List<SearchCriteria>();
                    foreach (KeyValuePair<string, SearchCriteria> kvp in map)
                    {
                        result.Add(kvp.Value);
                    }
                    return result;
                }

                #endregion

                #region Properties

                public TableSchema Table
                {
                    get { return table; }
                }

                #endregion
            }

            #endregion
        }

        #endregion

	}
}
