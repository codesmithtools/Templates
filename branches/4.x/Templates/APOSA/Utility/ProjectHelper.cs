using System;
using System.IO;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Text;
using CodeSmith.Engine;
using System.Text.RegularExpressions;
using SchemaExplorer;
using System.ComponentModel;
using CodeSmith.BaseTemplates;
using CodeSmith.CustomProperties;
using System.Diagnostics;

namespace Utility
{	
	public enum UIType
	{
		Web,
		MDIForm,
		WinForm
	}
	
	public class ProjectHelper : CodeSmith.BaseTemplates.SqlCodeTemplate
	{
		public ProjectHelper() : base()
		{
		}
		
		
		#region Name Helper Methods
		public static string GetSingularName(string name)
		{
			if(StringUtil.IsPlural(name))
			{
				return StringUtil.ToSingular(name);
			}
			else
			{
				return name;
			}
			
		}		
		public static string GetPluralName(string name)
		{
			if(StringUtil.IsSingular(name))
			{
				return StringUtil.ToPlural(name);
			}
			else
			{
				return name;
			}
			
		}		
		#endregion
		
		#region Comment Builders
		public static string BuildXmlClassComment(SchemaExplorer.CommandSchema command)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			
			builder.Append("\t\t/// <summary>\r\n");
			builder.Append("\t\t/// Date Created:");
			if(command != null) builder.Append(command.DateCreated);	
			builder.Append("\r\n\t\t/// <remarks>");
			builder.Append("\r\n\t\t/// ");
			if(command != null) builder.Append(command.Description);
			builder.Append("\r\n\t\t/// </remarks>\r\n");
			builder.Append("\t\t/// </summary>");
			
			
			return builder.ToString();
		}
		public static string BuildXmlParameterComment(CommandResultColumnSchema column)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			
			builder.Append("/// <summary>\r\n");
			builder.Append("\t\t/// <param name=\"");
			builder.Append(column.Name);
			builder.Append("\">");
			builder.Append("Type:");
			builder.Append(column.DataType);
			builder.Append("\r\n\t\t/// </param>\r\n");
			builder.Append("\t\t/// <remarks>");
			builder.Append("\r\n\t\t/// " + column.Description);
			builder.Append("\r\n\t\t/// </remarks>\r\n");
			builder.Append("\t\t/// </summary>");
			
			
			return builder.ToString();
		}
		public static string BuildXmlParameterComment(ParameterSchema parameter)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			
			builder.Append("/// <summary>\r\n");
			builder.Append("\t\t/// <param name=\"");
			builder.Append(parameter.Name);
			builder.Append("\">");
			builder.Append("Type:");
			builder.Append(parameter.DataType);
			builder.Append("\r\n\t\t/// </param>\r\n");
			builder.Append("\t\t/// <remarks>");
			builder.Append("\r\n\t\t/// " + parameter.Description);
			builder.Append("\r\n\t\t/// </remarks>\r\n");
			builder.Append("\t\t/// </summary>");
			
			
			return builder.ToString();
		}
		
		
		
		#endregion
		
		#region Name Format Methods	
		public static new string GetCamelCaseName(string value)
		{
			value = value.Replace(" ", "");
			if (value.StartsWith("@")) value = value.Substring(1);		
			
			return StringUtil.ToCamelCase(value.Substring(0, 1).ToLower() + value.Substring(1));
		}
		/*
		public static string GetParameterName2(string value)
		{
			value = value.Replace(" ", "");
			if (value.StartsWith("@")) value = value.Substring(1);
			return "prm" + value;
		}*/
		#endregion
				
		#region Property Creation Methods
		
		public static string GetMemberVariableName(CommandResultColumnSchema column)
		{
			string propertyName = GetPropertyName(column);
			string memberVariableName = "_" + GetCamelCaseName(propertyName);
			
			return memberVariableName;
		}
		
		public static string GetMemberVariableName(ParameterSchema parameter)
		{
			string propertyName = GetPropertyName(parameter);
			if (propertyName.StartsWith("@")) propertyName = propertyName.Substring(1);
			string memberVariableName = "_" + GetCamelCaseName(propertyName);
			
			return memberVariableName;
		}
		
		public static string GetPublicMemberVariableName(CommandResultColumnSchema column)
		{
			string propertyName = GetPropertyName(column);
			string memberVariableName = GetCamelCaseName(propertyName);
			
			return memberVariableName;
		}
		
		public static string GetPublicMemberVariableName(ParameterSchema parameter)
		{
			string propertyName = GetPropertyName(parameter);
			if (propertyName.StartsWith("@")) propertyName = propertyName.Substring(1);
			string memberVariableName = GetCamelCaseName(propertyName);
			
			return memberVariableName;
		}
		
		public static string GetPropertyName(CommandResultColumnSchema column)
		{
			string propertyName = column.Name.Replace(" ", "");				
			return StringUtil.ToPascalCase(propertyName);
		}
		
		public static string GetPropertyName(ParameterSchema parameter)
		{
			string propertyName = parameter.Name.Replace(" ", "");
			if (propertyName.StartsWith("@")) propertyName = propertyName.Substring(1);			
			
			return StringUtil.ToPascalCase(propertyName);
		}	
		
		public static string GetOriginalPropertyName(CommandResultColumnSchema column)
		{
			string propertyName = column.Name;				
			return propertyName;
		}
		
		public static string GetOriginalPropertyName(ParameterSchema parameter)
		{
			string propertyName = parameter.Name;
			if (propertyName.StartsWith("@")) propertyName = propertyName.Substring(1);			
			
			return propertyName;
		}	

		public static string GetMemberVariableDeclarationStatement(CommandResultColumnSchema column, bool canBeNullable)
		{
			return GetMemberVariableDeclarationStatement("private", column, canBeNullable);
		}
		
		public static string GetMemberVariableDeclarationStatement(string protectionLevel, CommandResultColumnSchema column, bool canBeNullable)
		{
			string statement = protectionLevel + " ";
			statement += GetCSharpVariableType(column.DataType, canBeNullable) + " " + GetMemberVariableName(column);
			
			string defaultValue = GetMemberVariableDefaultValue(column.DataType, canBeNullable, column.Name == "TransactionType");
			if (defaultValue != "")
			{
				statement += " = " + defaultValue;
			}
			
			statement += ";";
			
			return statement;
		}
		
		public static string GetMemberVariableDeclarationStatement(ParameterSchema parameter, bool canBeNullable)
		{
			return GetMemberVariableDeclarationStatement("private", parameter, canBeNullable);
		}
		
		public static string GetMemberVariableDeclarationStatement(string protectionLevel, ParameterSchema parameter, bool canBeNullable)
		{
			string statement = protectionLevel + " ";
			statement += GetCSharpVariableType(parameter.DataType, canBeNullable) + " " + GetMemberVariableName(parameter);
			
			string defaultValue = GetMemberVariableDefaultValue(parameter.DataType, canBeNullable, parameter.Name == "@TransactionType");
			if (defaultValue != "")
			{
				statement += " = " + defaultValue;
			}
			
			statement += ";";
			
			return statement;
		}
		
		public static string GetCSharpVariableType(DbType dbType, bool canBeNullable)
		{
			switch (dbType)
			{
				case DbType.AnsiString: return "string";
				case DbType.AnsiStringFixedLength: return "string";
				case DbType.Binary: return canBeNullable ? "byte?[]" : "byte[]";
				case DbType.Boolean: return canBeNullable ? "bool?" : "bool";
				case DbType.Byte: return canBeNullable ? "byte?" : "byte";
				case DbType.Currency: return canBeNullable ? "decimal?" : "decimal";
				case DbType.Date: return canBeNullable ? "DateTime?" : "DateTime";
				case DbType.DateTime: return canBeNullable ? "DateTime?" : "DateTime";
				case DbType.Decimal: return canBeNullable ? "decimal?" : "decimal";
				case DbType.Double: return canBeNullable ? "double?" : "double";
				case DbType.Guid: return canBeNullable ? "Guid?" : "Guid";
				case DbType.Int16: return canBeNullable ? "short?" : "short";
				case DbType.Int32: return canBeNullable ? "int?" : "int";
				case DbType.Int64: return canBeNullable ? "long?" : "long";
				case DbType.Object: return "object";
				case DbType.SByte: return canBeNullable ? "sbyte?" : "sbyte";
				case DbType.Single: return canBeNullable ? "float?" : "float";
				case DbType.String: return "string";
				case DbType.StringFixedLength: return "string";
				case DbType.Time: return canBeNullable ? "TimeSpan?" : "TimeSpan";
				case DbType.UInt16: return canBeNullable ? "ushort?" : "ushort";
				case DbType.UInt32: return canBeNullable ? "uint?" : "uint";
				case DbType.UInt64: return canBeNullable ? "ulong?" : "ulong";
				case DbType.VarNumeric: return canBeNullable ? "decimal?" : "decimal";
				default:
				{
					return "__UNKNOWN__" + dbType.ToString();
				}
			}
		}
		public static string GetSqlType(DbType dbType)
		{
			switch (dbType)
			{
				case DbType.AnsiString: return "SqlDbType.VarChar";
				case DbType.AnsiStringFixedLength: return "SqlDbType.VarChar";
				case DbType.Binary: return "SqlDbType.Binary";
				case DbType.Boolean: return "SqlDbType.Bit";
				case DbType.Byte: return "SqlDbType.Bit";
				case DbType.Currency: return "SqlDbType.Money";
				case DbType.Date: return "SqlDbType.SmallDateTime";
				case DbType.DateTime: return "SqlDbType.DateTime";
				case DbType.Decimal: return "SqlDbType.Decimal";
				case DbType.Double: return "SqlDbType.Decimal";
				case DbType.Guid: return "SqlDbType.UniqueIdentifier";
				case DbType.Int16: return "SqlDbType.SmallInt";
				case DbType.Int32: return "SqlDbType.Int";
				case DbType.Int64: return "SqlDbType.BigInt";
				case DbType.Object: return "SqlDbType.Variant";
				case DbType.Single: return "SqlDbType.Real";
				case DbType.String: return "SqlDbType.VarChar";
				case DbType.StringFixedLength: return "SqlDbType.Char";
				case DbType.VarNumeric: return "SqlDbType.Numeric";
				default:
				{
					return "__UNSUPPORTED__" + dbType.ToString();
				}
			}
		}
		public static string GetMemberVariableDefaultValue(DbType dbType, bool canBeNullable, bool isTransactionType)
		{
			if(isTransactionType) return "2";
			switch (dbType)
			{
				case DbType.AnsiString: return canBeNullable ? "null" : "string.Empty";
				case DbType.AnsiStringFixedLength: return  canBeNullable ? "null" : "string.Empty";
				case DbType.Binary: return  canBeNullable ? "null" : "new byte[]{}";
				case DbType.Boolean: return  canBeNullable ? "null" : "false";
				case DbType.Byte: return  canBeNullable ? "null" : "byte[]{}";
				case DbType.Currency: return  canBeNullable ? "null" : "0";
				case DbType.Date: return  canBeNullable ? "null" : "DateTime.Now";
				case DbType.DateTime: return  canBeNullable ? "null" : "DateTime.Now";;
				case DbType.Decimal: return  canBeNullable ? "null" : "decimal.Zero";
				case DbType.Double: return  canBeNullable ? "null" : "double.NaN";
				case DbType.Guid: return   canBeNullable ? "null" : "Guid.Empty";
				case DbType.Int16: return  canBeNullable ? "null" : "short.Parse(\"0\")";
				case DbType.Int32: return  canBeNullable ? "null" : "0";
				case DbType.Int64: return  canBeNullable ? "null" : "long.Parse(\"0\")";
				case DbType.Object: return "null";
				case DbType.Single: return  canBeNullable ? "null" : "0";
				case DbType.String: return  canBeNullable ? "null" : "string.Empty";
				case DbType.StringFixedLength: return  canBeNullable ? "null" : "string.Empty";
				case DbType.VarNumeric: return  canBeNullable ? "null" : "0";
				default:
				{
					return "__UNSUPPORTED__" + dbType.ToString();
				}
			}
		}
		#endregion
		
		#region Rule Setup Methods
		protected bool IsParameterNullable(ParameterSchema parameter) 
		{ 
			ExtendedProperty defaultValueProperty = parameter.ExtendedProperties["CS_Default"];
			if (defaultValueProperty == null)
				return false;	
		
			string defaultValue = defaultValueProperty.Value.ToString();
		
			if (defaultValue == "NULL")
				return true;
		
			return false;
		
		}
		
		public static string WriteValidationStatement(ParameterSchema parameter, bool canBeNullable)
		{		
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string booleanCatchStatement = string.Empty;
			string message = string.Empty;
			WriteValidationQuestion(parameter, canBeNullable, out booleanCatchStatement, out message);
			
			if(!string.IsNullOrEmpty(booleanCatchStatement))
			{
				sb.Append(string.Format("if ({0})\r\n",booleanCatchStatement));
				sb.Append("\t\t\t{\r\n");				
				sb.Append("\t\t\t 	Data.ValidationMessages.Add( \r\n");
				sb.Append("\t\t\t 		new ValidationMessage(ValidationLevel.Exception, \r\n");
				sb.Append(string.Format("\t\t\t 			\"{0}\", \"{1}\"));\r\n", GetPropertyName(parameter), message));
				sb.Append("\t\t\t}\r\n");		
			}
			else
			{
				ExtendedProperty defaultValueProperty = parameter.ExtendedProperties["CS_Default"];
				string defaultValue = string.Empty;
				if (defaultValueProperty != null)
					defaultValue = defaultValueProperty.Value.ToString();
				sb.Append(string.Format("//No rules for parameter {0} since this parameter has a default parameter : {1}.",parameter.Name,defaultValue));
			}
			return sb.ToString();
		}
		public static void WriteValidationQuestion(ParameterSchema parameter, bool canBeNullable, out string booleanCatchStatement, out string message)
		{		
			booleanCatchStatement = string.Empty;
			message = string.Empty;
			switch (parameter.DataType)
			{
				case DbType.AnsiString: 
					booleanCatchStatement = string.Format("string.IsNullOrEmpty(Data.{0})",GetPropertyName(parameter));
					message = "Value cannot be an empty string or null.";
					break;
				case DbType.AnsiStringFixedLength: 
					booleanCatchStatement =  string.Format("string.IsNullOrEmpty(Data.{0})",GetPropertyName(parameter));			
					message = "Value cannot be an empty string or null.";
					break;
				case DbType.Binary: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} < 0",GetPropertyName(parameter)) : string.Format("Data.{0} < 0",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than zero." : "Value cannot be less than zero.";
					break;	
				case DbType.Boolean: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null",GetPropertyName(parameter)) : string.Empty;
					message = canBeNullable ? "Value cannot be null." : string.Empty;
					break;
				case DbType.Byte: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} < 0",GetPropertyName(parameter)) : string.Format("Data.{0} < 0",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than zero." : "Value cannot be less than zero.";
					break;
				case DbType.Currency: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} <= decimal.Zero",GetPropertyName(parameter)) : string.Format("Data.{0} <= decimal.Zero",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than '0.01'." : "Value cannot be less than '0.01'.";
					break;
				case DbType.Date: 
					booleanCatchStatement =  canBeNullable ? string.Format("!Data.{0}.HasValue",GetPropertyName(parameter)) : string.Empty ;
					message = canBeNullable ? "Value must be a valid date and not null." : string.Empty ;
					break;
				case DbType.DateTime: 
					booleanCatchStatement =  canBeNullable ? string.Format("!Data.{0}.HasValue",GetPropertyName(parameter)) : string.Empty ;
					message = canBeNullable ? "Value must be a valid date and not null." : string.Empty ;
					break;
				case DbType.Decimal: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} <= decimal.Zero",GetPropertyName(parameter)) : string.Format("Data.{0} <= decimal.Zero",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than '0.01'." : "Value cannot be less than '0.01'.";
					break;
				case DbType.Double: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} <= decimal.Zero",GetPropertyName(parameter)) : string.Format("Data.{0} <= decimal.Zero",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than '0.01'." : "Value cannot be less than '0.01'.";
					break;
				case DbType.Guid: 
					booleanCatchStatement =  string.Format("Guid.Empty.Equals(Data.{0})",GetPropertyName(parameter));
					message = "Value cannot be equal to Guid.Empty.";
					break;
				case DbType.Int16: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} <= -1",GetPropertyName(parameter)) : string.Format("Data.{0} <= -1",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than '1'." : "Value cannot be less than '0'.";
					break;
				case DbType.Int32: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} <= -1",GetPropertyName(parameter)) : string.Format("Data.{0} <= -1",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than '1'." : "Value cannot be less than '0'.";
					break;
				case DbType.Int64: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} <= -1",GetPropertyName(parameter)) : string.Format("Data.{0} <= -1",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than '1'." : "Value cannot be less than '0'.";
					break;
				case DbType.Object: 
					booleanCatchStatement =  string.Format("Data.{0} == null",GetPropertyName(parameter));
					message = "Value cannot be null.";
					break;
				case DbType.SByte: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} < -1",GetPropertyName(parameter)) : string.Format("Data.{0} < -1",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than zero." : "Value cannot be less than -1.";
					break;
				case DbType.Single: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || float.IsNaN(Data.{0})",GetPropertyName(parameter)) : string.Format("float.IsNaN(Data.{0})",GetPropertyName(parameter));
					message = canBeNullable ? "Value must be a number and cannot be null." : "Value must be a number.";
					break;
				case DbType.String: 
					booleanCatchStatement =  string.Format("string.IsNullOrEmpty(Data.{0})",GetPropertyName(parameter));
					message = "Value cannot be an empty string or null.";
					break;
				case DbType.StringFixedLength: 
					booleanCatchStatement =  string.Format("string.IsNullOrEmpty(Data.{0})",GetPropertyName(parameter));
					message = "Value cannot be an empty string or null.";
					break;
				case DbType.Time: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} == TimeSpan.Zero",GetPropertyName(parameter)) : string.Format(" Data.{0} == TimeSpan.Zero",GetPropertyName(parameter));
					message = canBeNullable ? "Value must be a valid TimeSpan and cannot be null." : "Value must be a valid TimeSpan.";					
					break;
				case DbType.UInt16: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} <= -1",GetPropertyName(parameter)) : string.Format("Data.{0} <= -1",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than '1'." : "Value cannot be less than '0'.";
					break;
				case DbType.UInt32: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} <= -1",GetPropertyName(parameter)) : string.Format("Data.{0} <= -1",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than '1'." : "Value cannot be less than '0'.";
					break;
				case DbType.UInt64: 	
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} <= -1",GetPropertyName(parameter)) : string.Format("Data.{0} <= -1",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than '1'." : "Value cannot be less than '0'.";
					break;
				case DbType.VarNumeric: 
					booleanCatchStatement =  canBeNullable ? string.Format("Data.{0} == null || Data.{0} <= -1",GetPropertyName(parameter)) : string.Format("Data.{0} <= -1",GetPropertyName(parameter));
					message = canBeNullable ? "Value cannot be null or less than '1'." : "Value cannot be less than '0'.";
					break;
				case DbType.Xml: 
					booleanCatchStatement =  string.Format("Data.{0} == null",GetPropertyName(parameter));
					message = "Value cannot be null.";
					break;
				default:
				{
					break;
				}
			}
		}
		#endregion
		
		#region User Interface Helpers
		public static string GetUIControlName(ParameterSchema parameter, bool withInput)
		{
			switch (parameter.DataType)
			{
				case DbType.AnsiString: return withInput ? string.Format("txt{0}.Text",GetPropertyName(parameter)) : string.Format("txt{0}",GetPropertyName(parameter));
				case DbType.AnsiStringFixedLength: return withInput ? string.Format("txt{0}.Text",GetPropertyName(parameter)) : string.Format("txt{0}",GetPropertyName(parameter));
				case DbType.Binary: return withInput ? string.Format("txt{0}.Text",GetPropertyName(parameter)) : string.Format("txt{0}",GetPropertyName(parameter));
				case DbType.Boolean: return withInput ? string.Format("chk{0}.Checked",GetPropertyName(parameter)) : string.Format("chk{0}",GetPropertyName(parameter));
				case DbType.Byte: return withInput ? string.Format("chk{0}.Checked",GetPropertyName(parameter)) : string.Format("chk{0}",GetPropertyName(parameter));
				case DbType.Currency: return withInput ? string.Format("mtb{0}.Text",GetPropertyName(parameter)) : string.Format("mtb{0}",GetPropertyName(parameter));
				case DbType.Date: return withInput ? string.Format("dtp{0}.Value",GetPropertyName(parameter)) : string.Format("dtp{0}",GetPropertyName(parameter));
				case DbType.DateTime: return withInput ? string.Format("dtp{0}.Value",GetPropertyName(parameter)) : string.Format("dtp{0}",GetPropertyName(parameter));
				case DbType.Decimal: return withInput ? string.Format("mtb{0}.Text",GetPropertyName(parameter)) : string.Format("mtb{0}",GetPropertyName(parameter));
				case DbType.Double: return withInput ? string.Format("mtb{0}.Text",GetPropertyName(parameter)) : string.Format("mtb{0}",GetPropertyName(parameter));
				case DbType.Guid: return withInput ? string.Format("txt{0}.Text",GetPropertyName(parameter)) : string.Format("txt{0}",GetPropertyName(parameter));
				case DbType.Int16: return withInput ? string.Format("mtb{0}.Text",GetPropertyName(parameter)) : string.Format("mtb{0}",GetPropertyName(parameter));
				case DbType.Int32: return withInput ? string.Format("mtb{0}.Text",GetPropertyName(parameter)) : string.Format("mtb{0}",GetPropertyName(parameter));
				case DbType.Int64: return withInput ? string.Format("mtb{0}.Text",GetPropertyName(parameter)) : string.Format("mtb{0}",GetPropertyName(parameter));
				case DbType.Object: return withInput ? string.Format("txt{0}.Text",GetPropertyName(parameter)) : string.Format("txt{0}",GetPropertyName(parameter));
				case DbType.SByte: return withInput ? string.Format("chk{0}.Checked",GetPropertyName(parameter)) : string.Format("chk{0}",GetPropertyName(parameter));
				case DbType.Single: return withInput ? string.Format("mtb{0}.Text",GetPropertyName(parameter)) : string.Format("mtb{0}",GetPropertyName(parameter));
				case DbType.String: return withInput ? string.Format("txt{0}.Text",GetPropertyName(parameter)) : string.Format("txt{0}",GetPropertyName(parameter));
				case DbType.StringFixedLength: return withInput ? string.Format("txt{0}.Text",GetPropertyName(parameter)) : string.Format("txt{0}",GetPropertyName(parameter));
				//case DbType.Time: return canBeNullable ? "TimeSpan?" : "TimeSpan";
				case DbType.UInt16: return withInput ? string.Format("mtb{0}.Text",GetPropertyName(parameter)) : string.Format("mtb{0}",GetPropertyName(parameter));
				case DbType.UInt32: return withInput ? string.Format("mtb{0}.Text",GetPropertyName(parameter)) : string.Format("mtb{0}",GetPropertyName(parameter));
				case DbType.UInt64: return withInput ? string.Format("mtb{0}.Text",GetPropertyName(parameter)) : string.Format("mtb{0}",GetPropertyName(parameter));
				case DbType.VarNumeric: return withInput ? string.Format("mtb{0}.Text",GetPropertyName(parameter)) : string.Format("mtb{0}",GetPropertyName(parameter));
				default:
				{
					return string.Format("txt{0}",GetPropertyName(parameter));
				}
			}
		}
		public static string GetUIControlValueStatementOUT(ParameterSchema parameter)
		{
			switch (parameter.DataType)
			{
				case DbType.AnsiString: return string.Format("txt{0}.Text",GetPropertyName(parameter));
				case DbType.AnsiStringFixedLength: return string.Format("txt{0}.Text",GetPropertyName(parameter));
				case DbType.Binary: return string.Format("new byte[]{Convert.ToByte(txt{0}.Text)}",GetPropertyName(parameter));
				case DbType.Boolean: return string.Format("chk{0}.Checked",GetPropertyName(parameter)); 
				case DbType.Byte: return string.Format("Convert.ToByte(chk{0}.Checked)",GetPropertyName(parameter)); 
				case DbType.Currency: return string.Format("Convert.ToDecimal(mtb{0}.Text)",GetPropertyName(parameter));
				case DbType.Date: return string.Format("dtp{0}.Value",GetPropertyName(parameter));
				case DbType.DateTime: return string.Format("dtp{0}.Value",GetPropertyName(parameter));
				case DbType.Decimal: return string.Format("Convert.ToDecimal(mtb{0}.Text)",GetPropertyName(parameter));
				case DbType.Double: return string.Format("Convert.ToDouble(mtb{0}.Text)",GetPropertyName(parameter));
				case DbType.Guid: return string.Format("new Guid(txt{0}.Text)",GetPropertyName(parameter));
				case DbType.Int16: return string.Format("Convert.ToInt16(mtb{0}.Text)",GetPropertyName(parameter));
				case DbType.Int32: return string.Format("Convert.ToInt32(mtb{0}.Text)",GetPropertyName(parameter));
				case DbType.Int64: return string.Format("Convert.ToInt64(mtb{0}.Text)",GetPropertyName(parameter));
				case DbType.Object: return string.Format("txt{0}.Text",GetPropertyName(parameter));
				case DbType.SByte: return string.Format("Convert.ToSByte(chk{0}.Checked)",GetPropertyName(parameter)); 
				case DbType.Single: return string.Format("Convert.ToSingle(mtb{0}.Text)",GetPropertyName(parameter));
				case DbType.String: return string.Format("txt{0}.Text",GetPropertyName(parameter));
				case DbType.StringFixedLength: return string.Format("txt{0}.Text",GetPropertyName(parameter));
				//case DbType.Time: return canBeNullable ? "TimeSpan?" : "TimeSpan";
				case DbType.UInt16: return string.Format("Convert.ToUInt16(mtb{0}.Text)",GetPropertyName(parameter));
				case DbType.UInt32: return string.Format("Convert.ToUInt32(mtb{0}.Text)",GetPropertyName(parameter));
				case DbType.UInt64: return string.Format("Convert.ToUInt64(mtb{0}.Text)",GetPropertyName(parameter));
				case DbType.VarNumeric: return string.Format("Convert.ToDecimal(mtb{0}.Text)",GetPropertyName(parameter));
				default:
				{
					return string.Format("txt{0}.Text;",GetPropertyName(parameter));
				}
			}
		}
		public static string GetUIControlConversionType(ParameterSchema parameter, string value)
		{
			switch (parameter.DataType)
			{
				case DbType.AnsiString: return string.Format("Convert.ToString({0})",value);
				case DbType.AnsiStringFixedLength: return string.Format("Convert.ToString({0})",value);
				case DbType.Binary: return string.Format("Convert.ToString({0})",value);
				case DbType.Boolean: return string.Format("Convert.ToBoolean({0})",value);
				case DbType.Byte: return string.Format("Convert.ToBoolean({0})",value);
				case DbType.Currency: return string.Format("Convert.ToString({0})",value);
				case DbType.Date: return string.Format("Convert.ToDateTime({0})",value);
				case DbType.DateTime: return string.Format("Convert.ToDateTime({0})",value);
				case DbType.Decimal: return string.Format("Convert.ToString({0})",value);
				case DbType.Double: return string.Format("Convert.ToString({0})",value);
				case DbType.Guid: return string.Format("Convert.ToString({0})",value);
				case DbType.Int16: return string.Format("Convert.ToString({0})",value);
				case DbType.Int32: return string.Format("Convert.ToString({0})",value);
				case DbType.Int64: return string.Format("Convert.ToString({0})",value);
				case DbType.Object: return string.Format("Convert.ToString({0})",value);
				case DbType.SByte: return string.Format("Convert.ToBoolean({0})",value);
				case DbType.Single: return string.Format("Convert.ToString({0})",value);
				case DbType.String: return string.Format("Convert.ToString({0})",value);
				case DbType.StringFixedLength: return string.Format("Convert.ToString({0})",value);
				//case DbType.Time: return canBeNullable ? "TimeSpan?" : "TimeSpan";
				case DbType.UInt16: return string.Format("Convert.ToString({0})",value);
				case DbType.UInt32: return string.Format("Convert.ToString({0})",value);
				case DbType.UInt64: return string.Format("Convert.ToString({0})",value);
				case DbType.VarNumeric: return string.Format("Convert.ToString({0})",value);
				default:
				{
					return string.Format("txt{0}.Text;",GetPropertyName(parameter));
				}
			}
		}
		public static string GetUIControlDeclarationStatement(ParameterSchema parameter)
		{
			switch (parameter.DataType)
			{				
				case DbType.AnsiString: return string.Format("private System.Windows.Forms.TextBox txt{0};",GetPropertyName(parameter));
				case DbType.AnsiStringFixedLength: return string.Format("private System.Windows.Forms.TextBox txt{0};",GetPropertyName(parameter));
				case DbType.Binary: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				case DbType.Boolean: return string.Format("private System.Windows.Forms.CheckBox chk{0};",GetPropertyName(parameter)); 
				case DbType.Byte: return string.Format("private System.Windows.Forms.CheckBox chk{0};",GetPropertyName(parameter)); 
				case DbType.Currency: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				case DbType.Date: return string.Format("private System.Windows.Forms.DateTimePicker dtp{0};",GetPropertyName(parameter));
				case DbType.DateTime: return string.Format("private System.Windows.Forms.DateTimePicker dtp{0};",GetPropertyName(parameter));
				case DbType.Decimal: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				case DbType.Double: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				case DbType.Guid: return string.Format("private System.Windows.Forms.TextBox txt{0};",GetPropertyName(parameter));
				case DbType.Int16: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				case DbType.Int32: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				case DbType.Int64: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				case DbType.Object: return string.Format("private System.Windows.Forms.TextBox txt{0};",GetPropertyName(parameter));
				case DbType.SByte: return string.Format("private System.Windows.Forms.CheckBox chk{0};",GetPropertyName(parameter)); 
				case DbType.Single: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				case DbType.String: return string.Format("private System.Windows.Forms.TextBox txt{0};",GetPropertyName(parameter));
				case DbType.StringFixedLength: return string.Format("private System.Windows.Forms.TextBox txt{0};",GetPropertyName(parameter));
				//case DbType.Time: return canBeNullable ? "TimeSpan?" : "TimeSpan";
				case DbType.UInt16: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				case DbType.UInt32: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				case DbType.UInt64: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				case DbType.VarNumeric: return string.Format("private System.Windows.Forms.MaskedTextBox mtb{0};",GetPropertyName(parameter));
				default:
				{
					return string.Format("private System.Windows.Forms.txt{0};",GetPropertyName(parameter));
				}
			}
		}
		public static string GetUIControlInstanceStatement(ParameterSchema parameter)
		{
			switch (parameter.DataType)
			{
				case DbType.AnsiString: return string.Format("this.txt{0} = new System.Windows.Forms.TextBox();",GetPropertyName(parameter));
				case DbType.AnsiStringFixedLength: return string.Format("this.txt{0} = new System.Windows.Forms.TextBox();",GetPropertyName(parameter));
				case DbType.Binary: return string.Format("this.mtb{0} = new System.Windows.Forms.TextBox();",GetPropertyName(parameter));
				case DbType.Boolean: return string.Format("this.chk{0} = new System.Windows.Forms.CheckBox();",GetPropertyName(parameter)); 
				case DbType.Byte: return string.Format("this.chk{0} = new System.Windows.Forms.CheckBox();",GetPropertyName(parameter)); 
				case DbType.Currency: return string.Format("this.mtb{0} = new System.Windows.Forms.MaskedTextBox();",GetPropertyName(parameter));
				case DbType.Date: return string.Format("this.dtp{0} = new System.Windows.Forms.DateTimePicker();",GetPropertyName(parameter));
				case DbType.DateTime: return string.Format("this.dtp{0} = new System.Windows.Forms.DateTimePicker();",GetPropertyName(parameter));
				case DbType.Decimal: return string.Format("this.mtb{0} = new System.Windows.Forms.MaskedTextBox();",GetPropertyName(parameter));
				case DbType.Double: return string.Format("this.mtb{0} = new System.Windows.Forms.MaskedTextBox();",GetPropertyName(parameter));
				case DbType.Guid: return string.Format("this.txt{0} = new System.Windows.Forms.TextBox();",GetPropertyName(parameter));
				case DbType.Int16: return string.Format("this.mtb{0} = new System.Windows.Forms.MaskedTextBox();",GetPropertyName(parameter));
				case DbType.Int32: return string.Format("this.mtb{0} = new System.Windows.Forms.MaskedTextBox();",GetPropertyName(parameter));
				case DbType.Int64: return string.Format("this.mtb{0} = new System.Windows.Forms.MaskedTextBox();",GetPropertyName(parameter));
				case DbType.Object: return string.Format("this.txt{0} = new System.Windows.Forms.TextBox();",GetPropertyName(parameter));
				case DbType.SByte: return string.Format("this.chk{0} = new System.Windows.Forms.CheckBox();",GetPropertyName(parameter)); 
				case DbType.Single: return string.Format("this.mtb{0} = new System.Windows.Forms.MaskedTextBox();",GetPropertyName(parameter));
				case DbType.String: return string.Format("this.txt{0} = new System.Windows.Forms.TextBox();",GetPropertyName(parameter));
				case DbType.StringFixedLength: return string.Format("this.txt{0} = new System.Windows.Forms.TextBox();",GetPropertyName(parameter));
				//case DbType.Time: return canBeNullable ? "TimeSpan?" : "TimeSpan";
				case DbType.UInt16: return string.Format("this.mtb{0} = new System.Windows.Forms.MaskedTextBox();",GetPropertyName(parameter));
				case DbType.UInt32: return string.Format("this.mtb{0} = new System.Windows.Forms.MaskedTextBox();",GetPropertyName(parameter));
				case DbType.UInt64: return string.Format("this.mtb{0} = new System.Windows.Forms.MaskedTextBox();",GetPropertyName(parameter));
				case DbType.VarNumeric: return string.Format("this.mtb{0} = new System.Windows.Forms.MaskedTextBox();",GetPropertyName(parameter));
				default:
				{
					return string.Format("this.txt{0} = new System.Windows.Forms.TextBox();",GetPropertyName(parameter));
				}
			}
		}		
		public static string GetUIControlSetupStatement(ParameterSchema parameter, System.Drawing.Point point, System.Drawing.Size size, int tabIndex)
		{
			switch (parameter.DataType)
			{
				case DbType.AnsiString: return GetUITextBoxSetupDeclaration(parameter, point, size, tabIndex);
				case DbType.AnsiStringFixedLength: return GetUITextBoxSetupDeclaration(parameter, point, size, tabIndex);
				case DbType.Binary: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#","0");
				case DbType.Boolean: return GetUICheckBoxSetupDeclaration(parameter, point, size, tabIndex);
				case DbType.Byte: return GetUICheckBoxSetupDeclaration(parameter, point, size, tabIndex);
				case DbType.Currency: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#######0.00","0000000000");
				case DbType.Date: return GetUIDateTimePickerSetupDeclaration(parameter, point, size, tabIndex);
				case DbType.DateTime: return GetUIDateTimePickerSetupDeclaration(parameter, point, size, tabIndex);
				case DbType.Decimal: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#######0.00","0000000000");
				case DbType.Double: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#######0.00","0000000000");
				case DbType.Guid: return GetUITextBoxSetupDeclaration(parameter, point, size, tabIndex);
				case DbType.Int16: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#######0","0000000)");
				case DbType.Int32: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#######0","0000000)");
				case DbType.Int64: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#######0","0000000)");
				case DbType.Object: return GetUITextBoxSetupDeclaration(parameter, point, size, tabIndex);
				case DbType.SByte: return GetUICheckBoxSetupDeclaration(parameter, point, size, tabIndex);
				case DbType.Single: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#######0","0000000)");
				case DbType.String: return GetUITextBoxSetupDeclaration(parameter, point, size, tabIndex);
				case DbType.StringFixedLength: return GetUITextBoxSetupDeclaration(parameter, point, size, tabIndex);
				//case DbType.Time: return canBeNullable ? "TimeSpan?" : "TimeSpan";
				case DbType.UInt16: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#######0","0000000)");
				case DbType.UInt32: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#######0","0000000)");
				case DbType.UInt64: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#######0","0000000)");
				case DbType.VarNumeric: return GetUIMaskedTextBoxSetupDeclaration(parameter, point, size, tabIndex, "#######0","0000000)");
				default:
				{
					return string.Format("this.txt{0} = new System.Windows.Forms.TextBox();",GetPropertyName(parameter));
				}
			}
		}
		
		public static string GetUITextBoxSetupDeclaration(ParameterSchema parameter, System.Drawing.Point point, System.Drawing.Size size, int tabIndex)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			
			builder.Append("\t\t// \r\n");
			builder.Append(string.Format("\t\t// txt{0}\r\n",GetPropertyName(parameter)));
			builder.Append("\t\t// \r\n");
			builder.Append(string.Format("\t\t this.txt{0}.Enabled = true;\r\n",GetPropertyName(parameter)));
            builder.Append(string.Format("\t\t this.txt{0}.Location = new System.Drawing.Point({1}, {2});\r\n",GetPropertyName(parameter),point.X,point.Y));
            builder.Append(string.Format("\t\t this.txt{0}.Name = \"txt{0}\";\r\n",GetPropertyName(parameter)));
			builder.Append(string.Format("\t\t this.txt{0}.Size = new System.Drawing.Size({1}, {2});\r\n",GetPropertyName(parameter),size.Width,size.Height));
            builder.Append(string.Format("\t\t this.txt{0}.TabIndex = {1};\r\n",GetPropertyName(parameter),tabIndex));
			
			
			return builder.ToString();
			
		}
		public static string GetUIMaskedTextBoxSetupDeclaration(ParameterSchema parameter, System.Drawing.Point point, System.Drawing.Size size, int tabIndex, string mask, string text)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			
			builder.Append("// \r\n");
			builder.Append(string.Format("\t\t// mtb{0}\r\n",GetPropertyName(parameter)));
			builder.Append("\t\t// \r\n");
			builder.Append(string.Format("\t\t this.mtb{0}.AllowPromptAsInput = true;\r\n",GetPropertyName(parameter)));
			builder.Append(string.Format("\t\t this.mtb{0}.Enabled = true;\r\n",GetPropertyName(parameter)));
			builder.Append(string.Format("\t\t this.mtb{0}.HidePromptOnLeave = true;\r\n",GetPropertyName(parameter)));
            builder.Append(string.Format("\t\t this.mtb{0}.Location = new System.Drawing.Point({1}, {2});\r\n",GetPropertyName(parameter),point.X,point.Y));
            builder.Append(string.Format("\t\t this.mtb{0}.Mask = \"{1}\";\r\n",GetPropertyName(parameter),mask));
			builder.Append(string.Format("\t\t this.mtb{0}.Name = \"mtb{0}\";\r\n",GetPropertyName(parameter)));
			builder.Append(string.Format("\t\t this.mtb{0}.Size = new System.Drawing.Size({1}, {2});\r\n",GetPropertyName(parameter),size.Width,size.Height));
            builder.Append(string.Format("\t\t this.mtb{0}.TabIndex = {1};\r\n",GetPropertyName(parameter),tabIndex));
			builder.Append(string.Format("\t\t this.mtb{0}.Text = \"{1}\";\r\n",GetPropertyName(parameter),text));			
			
			return builder.ToString();
			
		}
		
		public static string GetUICheckBoxSetupDeclaration(ParameterSchema parameter, System.Drawing.Point point, System.Drawing.Size size, int tabIndex)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			
			builder.Append("\t\t// \r\n");
			builder.Append(string.Format("\t\t// chk{0}\r\n",GetPropertyName(parameter)));
			builder.Append("\t\t// \r\n");
			builder.Append(string.Format("\t\t this.chk{0}.AutoSize = true;\r\n",GetPropertyName(parameter)));
			builder.Append(string.Format("\t\t this.chk{0}.Checked = true;\r\n",GetPropertyName(parameter)));
			builder.Append(string.Format("\t\t this.chk{0}.CheckState = System.Windows.Forms.CheckState.Checked;\r\n",GetPropertyName(parameter)));
            builder.Append(string.Format("\t\t this.chk{0}.Location = new System.Drawing.Point({1}, {2});\r\n",GetPropertyName(parameter),point.X,point.Y));
            builder.Append(string.Format("\t\t this.chk{0}.Name = \"chk{0}\";\r\n",GetPropertyName(parameter)));
			builder.Append(string.Format("\t\t this.chk{0}.Size = new System.Drawing.Size({1}, {2});\r\n",GetPropertyName(parameter),size.Width,size.Height));
            builder.Append(string.Format("\t\t this.chk{0}.TabIndex = {1}\r\n;",GetPropertyName(parameter),tabIndex));			
			
			return builder.ToString();
			
		}
		public static string GetUIDateTimePickerSetupDeclaration(ParameterSchema parameter, System.Drawing.Point point, System.Drawing.Size size, int tabIndex)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			
			builder.Append("\t\t// \r\n");
			builder.Append(string.Format("\t\t// dtp{0}\r\n",GetPropertyName(parameter)));
			builder.Append("\t\t// \r\n");
			builder.Append(string.Format("\t\t this.dtp{0}.CustomFormat = \"MM/dd/yyyy hh:mm tt\";\r\n",GetPropertyName(parameter)));
			builder.Append(string.Format("\t\t this.dtp{0}.Checked = true;\r\n",GetPropertyName(parameter)));
			builder.Append(string.Format("\t\t this.dtp{0}.Format = System.Windows.Forms.DateTimePickerFormat.Custom;\r\n",GetPropertyName(parameter)));
            builder.Append(string.Format("\t\t this.dtp{0}.Location = new System.Drawing.Point({1}, {2});\r\n",GetPropertyName(parameter),point.X,point.Y));
            builder.Append(string.Format("\t\t this.dtp{0}.Name = \"dtp{0}\";\r\n",GetPropertyName(parameter)));
			builder.Append(string.Format("\t\t this.dtp{0}.Size = new System.Drawing.Size({1}, {2});\r\n",GetPropertyName(parameter),size.Width,size.Height));
            builder.Append(string.Format("\t\t this.dtp{0}.TabIndex = {1};\r\n",GetPropertyName(parameter),tabIndex));			
			//builder.Append(string.Format("\t\t this.dtp{0}.ShowUpDown = true;\r\n",GetPropertyName(parameter)));
			 			
			return builder.ToString();
			
		}
		#endregion
		
		#region File Helpers
		public static string MoveFiles(string directoryToGetFrom, string searchPattern, string directoryToMoveTo )
		{
			string message = string.Empty;
			
			if(!Directory.Exists(directoryToGetFrom)) 
			{
				message += "Directory " + directoryToGetFrom + " does not exist.";
				return message;
			}
			if (!Directory.Exists(directoryToMoveTo) )
				Directory.CreateDirectory(directoryToMoveTo);
				
			string[] files = Directory.GetFiles(directoryToGetFrom, searchPattern);
			
			for (int i = 0; i < files.Length; i++)
			{
				try
				{
					File.Copy(files[i],Path.Combine(directoryToMoveTo,Path.GetFileName(files[i])));
				}
				catch (Exception ex)
				{
					message += "Error while attempting to move file (" + files[i] + ").\r\n" + ex.Message;
				}
			}	
			return message;
		}
		
		#endregion
		
		#region File IO Operations
		public void DeleteFiles(string directory, string searchPattern)
		{
			if ( System.IO.Directory.Exists(directory) )
			{
				string[] files = Directory.GetFiles(directory, searchPattern);
				
				for (int i = 0; i < files.Length; i++)
				{
					try
					{
						File.Delete(files[i]);
					}
					catch (Exception ex)
					{
						Response.WriteLine("Error while attempting to delete file (" + files[i] + ").\r\n" + ex.Message);
					}
				}
			}
		}
		
		public void DeleteSubFolders(string directory)
		{
			if ( System.IO.Directory.Exists(directory) )
			{
				foreach ( string dir in System.IO.Directory.GetDirectories(directory) )    
				{
					DeleteFiles(dir, "*.*");
					DeleteSubFolders(dir);
					System.IO.Directory.Delete(dir);
				}
			}
		}
		#endregion
		
		#region Template Helper Methods
		public static void BuildSubTemplate(CodeTemplate parent, CodeTemplate template, IMergeStrategy strategy, string pathAndFile, string outputDir)
		{
			// instantiate the sub-template	
				
				parent.Response.WriteLine(string.Format("Begin Build SubTemplate for file {0}...",Path.GetFileName(pathAndFile)));
				// Set up the DL project  	
				parent.CopyPropertiesTo(template);
				
				//Render the file
				if(strategy == null)
					template.RenderToFile(Path.Combine(outputDir,pathAndFile), true);
				else
					template.RenderToFile(Path.Combine(outputDir,pathAndFile), strategy);
					
				parent.Response.WriteLine(string.Format("Build of {0} Complete.",Path.GetFileName(pathAndFile)));	
		}
		
		#endregion				
						
		#region 1. Project Properties
		#region TargetDataBase
		private SchemaExplorer.DatabaseSchema _targetDatabase;
		[Category("1. Project Properties")]
		[Description("Set this to indicate the default database. The connection string will be overridden by the XML for generating classes.")]
		public SchemaExplorer.DatabaseSchema TargetDatabase
		{
			get { return _targetDatabase; }
			set { _targetDatabase = value; }
		}
		#endregion
		
		#region Output Directory
		private string _outputDirectory = String.Empty;
		[Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))] 
		[Category("1. Project Properties")]
		[Description("The directory to output the results to.")]
		public string OutPutDirectory 
		{
			get
			{
				// default to the directory that the template is located in
				if (_outputDirectory.Length == 0) return this.CodeTemplateInfo.DirectoryName + "output\\";
				
				return _outputDirectory;
			}
			set
			{
				if (value != null && !value.EndsWith("\\")) value += "\\";
				_outputDirectory = value;
			} 
		}
		#endregion			
		
		#region CompanyNameSpace
		private string _companyNameSpace = string.Empty;
		[Category("1. Project Properties")]
		[Description("CompanyNameSpace indicates which company this code build is for.")]
		public string CompanyNameSpace
		{
			get { return _companyNameSpace; }
			set { _companyNameSpace = value; }
		}
		#endregion
		
		#region ProjectNameSpace
		private string _projectNameSpace = string.Empty;
		[Category("1. Project Properties")]
		[Description("ProjectNameSpace indicates The Global Application Namespace.")]
		public string ProjectNameSpace
		{
			get { return _projectNameSpace; }
			set { _projectNameSpace = value; }
		}
		#endregion
		
		#region ProjectDescription
		private string _projectDescription = String.Empty;
		[Category("1. Project Properties")]
		[Description("General Project Description.")]
		public string ProjectDescription 
		{
			get
			{
				return _projectDescription;
			}
			set
			{
				_projectDescription = value;
			} 
		}
		#endregion	
		
		#region ProjectVersion
		private string _projectVersion = "1.0.0.0";
		[Category("1. Project Properties")]
		[Description("Version of Assembly.")]
		public string ProjectVersion 
		{
			get
			{
				return _projectVersion;
			}
			set
			{
				_projectVersion = value;
			} 
		}
		#endregion	
		
		#region CompanyName
		private string _companyName = String.Empty;
		[Category("1. Project Properties")]
		[Description("Name of Business to which code belongs. CompanyName is the long name of the company to be set in the AssemblyInfo files.")]
		public string CompanyName 
		{
			get
			{
				return _companyName;
			}
			set
			{
				_companyName = value;
			} 
		}
		#endregion	
		#endregion
		
		#region 2. Project Options

		#region CanHaveNullablePrimitives
		private bool _canHaveNullablePrimitives = false;
		[Category("2. Project Options")]
		[Description("CanHaveNullablePrimitives allows the .NET 2.0 nullable primitive types to be set with null.")]
		public bool CanHaveNullablePrimitives
		{
			get { return _canHaveNullablePrimitives; }
			set { _canHaveNullablePrimitives = value; }
		}
		#endregion
		
		#region CleanDirectories
		private bool _cleanDirectories = false;
		[Category("2. Project Options")]
		[Description("CleanDirectories allows the output directory to be cleaned after every build. If you wish to preserve your merges, set this to false.")]
		public bool CleanDirectories
		{
			get { return _cleanDirectories; }
			set { _cleanDirectories = value; }
		}
		#endregion		
		
		#region Generate UI
		private bool _generateUI = false;
		[Category("2. Project Options")]
		[Description("GenerateUI if set to true generates a basic UI from a template.")]
		public bool GenerateUI
		{
			get { return _generateUI; }
			set { _generateUI = value; }
		}
		#endregion		
		
		#region Generate Solution
		private bool _generateSolution = false;
		[Category("2. Project Options")]
		[Description("GenerateSolution if set to true generates the solution files replacing previous files.")]
		public bool GenerateSolution
		{
			get { return _generateSolution; }
			set { _generateSolution = value; }
		}
		#endregion		
		
		#region UI Type
		private Utility.UIType _uiType = UIType.MDIForm;
		[Category("2. Project Options")]
		[Description("TypeOfUserInterface indicates which UI type will be set: Web, MDIForm or WinForm (non-MDI).")]
		public Utility.UIType TypeOfUserInterface
		{
			get { return _uiType; }
			set { _uiType = value; }
		}
		#endregion		
		
		
			
		#endregion
		
		#region 3. Project Generated Data
		#region Project GUID
		private Guid _projectGuid = Guid.Empty;
		[Category("3. Project Generated Data")]
		[NotCheckedAttribute()]		
		[OptionalAttribute]
		[Description("Guid for entire project.")]
		
		public Guid ProjectGuid
		{
			get { 
					if(_projectGuid == Guid.Empty)
						_projectGuid = Guid.NewGuid();
					return _projectGuid; 
				}			
		}
		#endregion		
		
		#region SolutionProjectGuid GUID
		private Guid _solutionProjectGuid= Guid.Empty;
		[Category("3. Project Generated Data")]
		[NotCheckedAttribute()]	
		[OptionalAttribute]
		[Description("Auto generted guid for solution.")]
		public Guid SolutionProjectGuid
		{
			get { 
					if(_solutionProjectGuid == Guid.Empty)
						_solutionProjectGuid = Guid.NewGuid();
					return _solutionProjectGuid; 
				}	
			set { _solutionProjectGuid = value; }
		}
		#endregion		
		
		#region BusinessLayerProjectGuid GUID
		private Guid _businessLayerProjectGuid= Guid.Empty;
		[Category("3. Project Generated Data")]
		[NotCheckedAttribute()]		
		[OptionalAttribute]
		[Description("Auto generated GUID for project.")]
		public Guid BusinessLayerProjectGuid
		{
			get { 
					if(_businessLayerProjectGuid == Guid.Empty)
						_businessLayerProjectGuid = Guid.NewGuid();
					return _businessLayerProjectGuid; 
				}			
			set { _dataLayerProjectGuid = value; }
		}
		#endregion		
				
		#region DataLayerProjectGuid GUID
		private Guid _dataLayerProjectGuid= Guid.Empty;
		[Category("3. Project Generated Data")]
		[NotCheckedAttribute()]		
		[OptionalAttribute]
		[Description("Auto generated GUID for project.")]
		public Guid DataLayerProjectGuid
		{
			get { 
					if(_dataLayerProjectGuid == Guid.Empty)
						_dataLayerProjectGuid = Guid.NewGuid();
					return _dataLayerProjectGuid; 
				}
			set { _dataLayerProjectGuid = value; }
		}
		#endregion	
		
		#region FormsUIComponentProjectGuid GUID
		private Guid _formsUIComponentProjectGuid= Guid.Empty;
		[Category("3. Project Generated Data")]
		[NotCheckedAttribute()]		
		[OptionalAttribute]
		[Description("Auto generated GUID for project.")]
		public Guid FormsUIComponentProjectGuid
		{
			get { 
					if(_formsUIComponentProjectGuid == Guid.Empty)
						_formsUIComponentProjectGuid = Guid.NewGuid();
					return _formsUIComponentProjectGuid; 
				}	
			set { _formsUIComponentProjectGuid = value; }
		}
		#endregion	
		
		#region UserInterfaceProjectGuid GUID
		private Guid _userInterfaceProjectGuid= Guid.Empty;
		[Category("3. Project Generated Data")]
		[NotCheckedAttribute()]		
		[OptionalAttribute]
		[Description("Auto generated GUID for project.")]
		public Guid UserInterfaceProjectGuid
		{
			get { 
					if(_userInterfaceProjectGuid == Guid.Empty)
						_userInterfaceProjectGuid = Guid.NewGuid();
					return _userInterfaceProjectGuid; 
				}			
			set { _userInterfaceProjectGuid = value; }
		}
		#endregion					
		
		#endregion
			
		#region Events
		public void OutputTemplate(CodeTemplate template)
		{
			this.CopyPropertiesTo(template);
			template.Render(this.Response);
		}
	
		protected override void OnPostRender(string result)
		{
			
			// execute the output on the same database as the source table.
			CodeSmith.BaseTemplates.ScriptResult scriptResult = CodeSmith.BaseTemplates.ScriptUtility.ExecuteScript(this.TargetDatabase.Database.ConnectionString, result, new System.Data.SqlClient.SqlInfoMessageEventHandler(cn_InfoMessage));
			Trace.Write(scriptResult.ToString());			
	
			base.OnPostRender(result);
		}
	
		private void cn_InfoMessage(object sender, System.Data.SqlClient.SqlInfoMessageEventArgs e)
		{
			Trace.WriteLine(e.Message);
		}
		#endregion
	}
}