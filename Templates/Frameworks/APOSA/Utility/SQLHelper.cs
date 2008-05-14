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
	
	
	public class SQLHelper : CodeSmith.BaseTemplates.SqlCodeTemplate
	{
		public static string GetSqlParameterStatementFromColumn(ColumnSchema column, bool isOutPut)
		{		
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("@");
			builder.Append(StringUtil.ToPascalCase(column.Name));	
			switch (column.DataType)
			{
				case DbType.AnsiString: 
					builder.Append(string.Format(" varchar({0})",column.Size));
					break;
				case DbType.AnsiStringFixedLength: 
					builder.Append(string.Format(" char({0})",column.Size));
					break;
				case DbType.Binary: 
					builder.Append(" binary");
					break;	
				case DbType.Boolean: 
					builder.Append(" bit");
					break;
				case DbType.Byte: 
					builder.Append(" bit");
					break;
				case DbType.Currency: 
					builder.Append(" money");
					break;
				case DbType.Date: 
					builder.Append(" smalldatetime");
					break;
				case DbType.DateTime: 
					builder.Append(" datetime");
					break;
				case DbType.Decimal: 
					builder.Append(" decimal");
					break;				
				case DbType.Double: 
					builder.Append(" decimal");
					break;			
				case DbType.Guid: 
					builder.Append(" uniqueidentifier");
					break;			
				case DbType.Int16: 
					builder.Append(" smallint");
					break;
				case DbType.Int32: 
					builder.Append(" int");
					break;
				case DbType.Int64: 
					builder.Append(" bigint");
					break;
				case DbType.Object: 
					builder.Append(" sql_variant");
					break;
				case DbType.Single: 
					builder.Append(" real");
					break;	
				case DbType.String: 
					builder.Append(string.Format(" varchar({0})",column.Size));
					break;		
				case DbType.StringFixedLength: 
					builder.Append(string.Format(" char({0})",column.Size));
					break;		
				case DbType.VarNumeric: 
					builder.Append(" numeric");
					break;		
				default:
				{
					builder.Append(" sql_variant");
					break;
				}
				
			}
			if(isOutPut) builder.Append(" OUTPUT");
			return builder.ToString();
		}
	}
		
}