using System;
using System.Text;
using System.ComponentModel;
using CodeSmith.Engine;
using System.Data;
using SchemaExplorer;

namespace CodeSmith.BaseTemplates
{
	public class SqlCodeTemplate : CodeTemplate
	{
		public string GetSqlParameterStatements(string statementPrefix, ColumnSchema column)
		{
			return GetSqlParameterStatements(statementPrefix, column, "sql");
		}
		
		public string GetSqlParameterStatements(string statementPrefix, ColumnSchema column, string sqlObjectName)
		{
			string statements = "\r\n" + statementPrefix + sqlObjectName + ".AddParameter(\"@" + column.Name + "\", SqlDbType." + GetSqlDbType(column) + ", this." + GetPropertyName(column) + GetSqlParameterExtraParams(statementPrefix, column);
			
			return statements.Substring(statementPrefix.Length + 2);
		}
		
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
		
		public string GetFillByIndexName(IndexSchema index)
		{
			StringBuilder fillByIndexName = new StringBuilder();
			
			fillByIndexName.Append("FillBy");
			
			for (int i = 0; i < index.MemberColumns.Count; i++)
			{
				fillByIndexName.Append(index.MemberColumns[i].Name.Replace(" ", ""));
				if (i < index.MemberColumns.Count - 1)
				{
					fillByIndexName.Append("And");
				}
			}
			
			return fillByIndexName.ToString();
		}
		
		public string GetFillByIndexParameters(IndexSchema index)
		{
			StringBuilder fillByIndexParameters = new StringBuilder();
			
			for (int i = 0; i < index.MemberColumns.Count; i++)
			{
				fillByIndexParameters.Append(GetCSharpVariableType(index.MemberColumns[i]));
				fillByIndexParameters.Append(" ");
				fillByIndexParameters.Append(GetCamelCaseName(index.MemberColumns[i].Name));
				
				if (i < index.MemberColumns.Count - 1)
				{
					fillByIndexParameters.Append(", ");
				}
			}
			
			return fillByIndexParameters.ToString();
		}
		
		public string GetMemberVariableName(string value)
		{
			string memberVariableName = "_" + GetCamelCaseName(value);
			
			return memberVariableName;
		}
		
		public string GetSqlParameterExtraParams(string statementPrefix, ColumnSchema column)
		{
			if (SizeMatters(column) && PrecisionMatters(column))
			{
				return ");\r\n" + statementPrefix + "prm.Scale = " + column.Scale + ";\r\n" + statementPrefix + "prm.Precision = " + column.Precision + ";";
			}
			else if (SizeMatters(column))
			{
				return ", " + column.Size + ");";
			}
			else
			{
				return ");";
			}
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
		
		public string GetMemberVariableDeclarationStatement(ColumnSchema column)
		{
			return GetMemberVariableDeclarationStatement("protected", column);
		}
		
		public string GetMemberVariableDeclarationStatement(string protectionLevel, ColumnSchema column)
		{
			string statement = protectionLevel + " ";
			statement += GetCSharpVariableType(column) + " " + GetMemberVariableName(column.Name);
			
			string defaultValue = GetMemberVariableDefaultValue(column);
			if (defaultValue != "")
			{
				statement += " = " + defaultValue;
			}
			
			statement += ";";	
			
			return statement;
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
		
		public string GetCSharpVariableType(ColumnSchema column)
		{
			if (column.Name.EndsWith("TypeCode")) return column.Name;
			
			switch (column.DataType)
			{
				case DbType.AnsiString: return "string";
				case DbType.AnsiStringFixedLength: return "string";
				case DbType.Binary: return "byte[]";
				case DbType.Boolean: return "bool";
				case DbType.Byte: return "byte";
				case DbType.Currency: return "decimal";
				case DbType.Date: return "DateTime";
				case DbType.DateTime: return "DateTime";
				case DbType.Decimal: return "decimal";
				case DbType.Double: return "double";
				case DbType.Guid: return "Guid";
				case DbType.Int16: return "short";
				case DbType.Int32: return "int";
				case DbType.Int64: return "long";
				case DbType.Object: return "object";
				case DbType.SByte: return "sbyte";
				case DbType.Single: return "float";
				case DbType.String: return "string";
				case DbType.StringFixedLength: return "string";
				case DbType.Time: return "TimeSpan";
				case DbType.UInt16: return "ushort";
				case DbType.UInt32: return "uint";
				case DbType.UInt64: return "ulong";
				case DbType.VarNumeric: return "decimal";
				default:
				{
					return "__UNKNOWN__" + column.NativeType;
				}
			}
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
		
		public string GetSqlDbType(ColumnSchema column)
		{
			switch (column.NativeType)
			{
				case "bigint": return "BigInt";
				case "binary": return "Binary";
				case "bit": return "Bit";
				case "char": return "Char";
				case "datetime": return "DateTime";
				case "decimal": return "Decimal";
				case "float": return "Float";
				case "image": return "Image";
				case "int": return "Int";
				case "money": return "Money";
				case "nchar": return "NChar";
				case "ntext": return "NText";
				case "numeric": return "Decimal";
				case "nvarchar": return "NVarChar";
				case "real": return "Real";
				case "smalldatetime": return "SmallDateTime";
				case "smallint": return "SmallInt";
				case "smallmoney": return "SmallMoney";
				case "sql_variant": return "Variant";
				case "sysname": return "NChar";
				case "text": return "Text";
				case "timestamp": return "Timestamp";
				case "tinyint": return "TinyInt";
				case "uniqueidentifier": return "UniqueIdentifier";
				case "varbinary": return "VarBinary";
				case "varchar": return "VarChar";
				default: return "__UNKNOWN__" + column.NativeType;
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
	}
}
