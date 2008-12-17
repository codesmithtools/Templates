public enum SqlType
{
	StoredProcedures,
	Dynamic
}

public string GetGetSql(TableSchema SourceTable)
{
	return GetGetSql(SourceTable, SqlType.StoredProcedures);
}

public string GetGetSql(TableSchema SourceTable, SqlType sqlType)
{
	string statement = "";
	statement = "SELECT \r\n";
	if(sqlType == SqlType.Dynamic) 
	{
		statement += "\" + " + GetClassName(SourceTable) + ".SelectFieldList + @\"\r\n";
	} 
	else 
	{
		statement += GetTableColumnList(GetValidColumns(SourceTable.Columns)) + " \r\n";
	}
	statement += "FROM [" + SourceTable.Owner + "].[" + SourceTable.Name + "] \r\n";
	statement += "WHERE \r\n";
	statement += GetWhereClauseForColumnsEqualsSqlParameterList(SourceTable.PrimaryKey.MemberColumns);
	return statement;
}

public string GetGetAllSql(TableSchema SourceTable) 
{
	return GetGetAllSql(SourceTable, true, SqlType.StoredProcedures);
}

public string GetGetAllSql(TableSchema SourceTable, SqlType sqlType) 
{
	return GetGetAllSql(SourceTable, true, sqlType);
}

public string GetGetAllSql(TableSchema SourceTable, bool IncludeSelectStatement, SqlType sqlType)
{
	string statement = "";
	
	if(IncludeSelectStatement == true)
	{
	statement += "SELECT "; 
	}
	
	if(sqlType == SqlType.Dynamic) 
	{
		statement += "\" + " + GetClassName(SourceTable) + ".SelectFieldList + \"";
	} 
	else 
	{
		statement += GetTableColumnList(GetValidColumns(SourceTable.Columns)) + " \r\n";
	}
	statement += "FROM [" + SourceTable.Owner + "].[" + SourceTable.Name + "]";
	return statement;
}

public string GetGetByFKSql(TableSchema SourceTable, TableKeySchema FKTable)
{
	return GetGetByFKSql(SourceTable, FKTable, SqlType.StoredProcedures);
}


public string GetGetByFKSql(TableSchema SourceTable, TableKeySchema FKTable, SqlType sqlType)
{
    return GetGetByFKSql(SourceTable, FKTable, sqlType, true);
}

public string GetGetByFKSql(TableSchema SourceTable, TableKeySchema FKTable, SqlType sqlType, bool includeSelect)
{
        string statement = String.Empty;

        if (includeSelect)
        {
         	statement = "SELECT \r\n";

        	if(sqlType == SqlType.Dynamic) 
	        {
		       statement += "\" + " + GetClassName(SourceTable) + ".SelectFieldList + @\"\r\n";
	        } 
 	        else 
	        {
		        statement += GetTableColumnList(SourceTable.Columns) + " \r\n";
	        }
        }

	statement += "FROM \r\n";
	statement += "\t[" + SourceTable.Owner + "].[" + SourceTable.Name + "] \r\n";
	statement += "WHERE \r\n";
	statement += GetWhereClauseForColumnsEqualsSqlParameterList(FKTable.ForeignKeyMemberColumns);
	return statement;

}

public string GetInsertSql(TableSchema SourceTable)
{
	string statement = "";
	
	bool isIdentity = SourceTable.PrimaryKey.MemberColumns.Count == 1 && (((bool)SourceTable.PrimaryKey.MemberColumns[0].ExtendedProperties["CS_IsIdentity"].Value) == true) &&(SourceTable.PrimaryKey.MemberColumns[0].DataType == DbType.Guid || SourceTable.PrimaryKey.MemberColumns[0].DataType == DbType.Int16 || SourceTable.PrimaryKey.MemberColumns[0].DataType == DbType.Int32 || SourceTable.PrimaryKey.MemberColumns[0].DataType == DbType.Int64);
	if (!isIdentity)
	{
		ColumnSchema primaryKey = SourceTable.PrimaryKey.MemberColumns[0];
		if (primaryKey.DataType == DbType.Guid) 
		{
			statement += "IF len(@" + primaryKey.Name + ") = 0 \r\n";
			statement += "BEGIN \r\n";
			statement += "\tSET @" + primaryKey.Name + " = NEWID(); \r\n";
			statement += "END \r\n\r\n";
		}
	}

	statement += "DECLARE @table TABLE(\r\n" + GetTableColumnListWithTypes(GetValidColumns(SourceTable.Columns)) + "\r\n);\r\n\r\n";
	
	statement += "INSERT INTO [" + SourceTable.Owner + "].[" + SourceTable.Name + "] (\r\n";
	if(isIdentity)
	{
		statement += GetTableColumnList(FilterIdentityColumns(SourceTable.NonPrimaryKeyColumns)) + "\r\n) \r\n";
	}
	else 
	{
		statement += GetTableColumnList(FilterIdentityColumns(GetValidColumns(SourceTable.Columns))) + "\r\n) \r\n";		
	}
	statement += "output \r\n" + GetTableColumnList(GetValidColumns(SourceTable.Columns), "INSERTED") + "\r\ninto @table\r\n";
	statement += "VALUES ( \r\n";
	if(isIdentity)
	{
		statement += GetInsertParameterList(FilterIdentityColumns(SourceTable.NonPrimaryKeyColumns)) + " \r\n";
	}
	else 
	{
		statement += GetInsertParameterList(FilterIdentityColumns(GetValidColumns(SourceTable.Columns))) + " \r\n";
	}
	statement += "); \r\n\r\n";
	
	statement += "SELECT \r\n" + GetTableColumnList(GetValidColumns(SourceTable.Columns), "") + " \r\nFROM @table;\r\n";
	return statement;
}

public ColumnSchemaCollection FilterIdentityColumns(ColumnSchemaCollection columns) 
{
	ColumnSchemaCollection filteredColumns = new ColumnSchemaCollection();
	
	foreach(ColumnSchema column in columns) 
	{
		if(!(bool)column.ExtendedProperties["CS_IsIdentity"].Value && column.NativeType != "timestamp") 
		{
			filteredColumns.Add(column);
		}
	}
	
	return filteredColumns;
}	

public string GetUpdateSql(TableSchema SourceTable)
{
	string statement = "DECLARE @table TABLE(\r\n" + GetTableColumnListWithTypes(GetValidColumns(SourceTable.Columns)) + "\r\n);\r\n\r\n";
	statement += "UPDATE [" + SourceTable.Owner + "].[" + SourceTable.Name + "] SET \r\n";
	statement += GetUpdateClauseForColumnsEqualsSqlParameterList(FilterIdentityColumns(SourceTable.NonPrimaryKeyColumns)) + " \r\n";
	statement += "output \r\n" + GetTableColumnList(GetValidColumns(SourceTable.Columns), "INSERTED") + "\r\ninto @table\r\n";
	statement += "WHERE \r\n";
	statement += GetWhereClauseForColumnsEqualsSqlParameterList(SourceTable.PrimaryKey.MemberColumns);
	statement += "\r\n\r\n";
	statement += "SELECT \r\n" + GetTableColumnList(GetValidColumns(SourceTable.Columns), "") + " \r\nFROM @table;\r\n";
	return statement;
}

public string GetDeleteSql(TableSchema SourceTable)
{
	string statement = "DELETE FROM [" + SourceTable.Owner + "].[" + SourceTable.Name + "]\r\n";
	statement += "WHERE \r\n";
	statement += GetWhereClauseForColumnsEqualsSqlParameterList(SourceTable.PrimaryKey.MemberColumns);
	return statement;
}

public string GetDeleteByFKSql(TableSchema SourceTable, TableKeySchema FKTable) 
{
	string statement = "DELETE \r\n";
	statement += "FROM \r\n";
	statement += "\t[" + SourceTable.Owner + "].[" + SourceTable.Name + "] \r\n";
	statement += "WHERE \r\n";
	statement += GetWhereClauseForColumnsEqualsSqlParameterList(FKTable.ForeignKeyMemberColumns);
	return statement;
}

public enum TransactionIsolationLevelEnum
{
	ReadCommitted,
	ReadUncommitted,
	RepeatableRead,
	Serializable
}

public string GetSetTransactionIsolationLevelStatement()
{
	switch (IsolationLevel)
	{
		case TransactionIsolationLevelEnum.ReadCommitted:
		{
			return "SET TRANSACTION ISOLATION LEVEL READ COMMITTED";
		}
		case TransactionIsolationLevelEnum.ReadUncommitted:
		{
			return "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED";
		}
		case TransactionIsolationLevelEnum.RepeatableRead:
		{
			return "SET TRANSACTION ISOLATION LEVEL REPEATABLE READ";
		}
		case TransactionIsolationLevelEnum.Serializable:
		{
			return "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE";
		}
	}
	
	return "SET TRANSACTION ISOLATION LEVEL READ COMMITTED";
}

public string GetTableOwner()
{
	return GetTableOwner(true);
}

public string GetTableOwner(bool includeDot)
{
	if (SourceTable.Owner.Length > 0)
	{
		return "[" + SourceTable.Owner + "].";
	}
	else
	{
		return "";
	}
}

public string GetTableColumnListWithTypes(ColumnSchemaCollection sourceTableColumns)
{
	string returnValue = string.Empty;
	for (int i = 0; i < sourceTableColumns.Count; i++) 
	{
		returnValue += "\t" + GetSqlParameterStatement(sourceTableColumns[i], false, false, true);
		if(i != sourceTableColumns.Count - 1) {
			returnValue += ",\r\n";
		}
	}
	return returnValue;
}

public string GetStoredProcedureParameterList(ColumnSchemaCollection sourceTableColumns)
{
	return GetStoredProcedureParameterList(sourceTableColumns, false);
}

public string GetStoredProcedureParameterList(ColumnSchemaCollection sourceTableColumns, bool prefixWithDelimiter)
{
	string returnValue = string.Empty;
	for (int i = 0; i < sourceTableColumns.Count; i++) 
	{
		returnValue += "\t" + GetSqlParameterStatement(sourceTableColumns[i]);
		if(i != sourceTableColumns.Count - 1) {
			returnValue += ",\r\n";
		}
	}
	return returnValue;
}

public string GetStoredProcedureParameterListWithoutTypes(ColumnSchemaCollection sourceTableColumns)
{
	return GetStoredProcedureParameterListWithoutTypes(sourceTableColumns, false);
}

public string GetStoredProcedureParameterListWithoutTypes(ColumnSchemaCollection sourceTableColumns, bool prefixWithDelimiter)
{
	string returnValue = string.Empty;
	for (int i = 0; i < sourceTableColumns.Count; i++) 
	{
		returnValue += "\t@" + sourceTableColumns[i].Name;
		if (i != sourceTableColumns.Count - 1) 
		{
			returnValue += ",\r\n";
		}
	}
	return returnValue;
}

public string GetInsertParameterList(ColumnSchemaCollection sourceTableColumns)
{
	string returnValue = string.Empty;
	for (int i = 0; i < sourceTableColumns.Count; i++) 
	{
		if(sourceTableColumns[i].ExtendedProperties["CS_Default"].Value.ToString() != "") {
			returnValue += "\tISNULL(@" + sourceTableColumns[i].Name + ", " + sourceTableColumns[i].ExtendedProperties["CS_Default"].Value + ")";
		} else {
			returnValue += "\t@" + sourceTableColumns[i].Name;
		}
		if (i != sourceTableColumns.Count - 1) 
		{
			returnValue += ",\r\n";
		}
	}
	return returnValue;
}

public string GetTableColumnList(ColumnSchemaCollection sourceTableColumns, string prefix)
{
	string returnValue = string.Empty;
	for (int i = 0; i < sourceTableColumns.Count; i++) 
	{
		if(prefix.Length > 0)
		{
			returnValue += "\t" + prefix + ".[" + sourceTableColumns[i].Name + "]";
		} 
		else 
		{
			returnValue += "\t" + "[" + sourceTableColumns[i].Name + "]";
		}
		if(i != sourceTableColumns.Count - 1)
		{
			returnValue += ",\r\n";
		
		}
	}
	return returnValue;	
}

public string GetTableColumnList(ColumnSchemaCollection sourceTableColumns)
{
	return GetTableColumnList(sourceTableColumns, false);
}

public string GetTableColumnList(ColumnSchemaCollection sourceTableColumns, bool prefixWithDelimiter)
{
	string returnValue = string.Empty;
	for (int i = 0; i < sourceTableColumns.Count; i++) 
	{
		returnValue += "\t" + "[" + sourceTableColumns[i].Table.Name + "].[" + sourceTableColumns[i].Name + "]";
		if(i != sourceTableColumns.Count - 1)
		{
			returnValue += ",\r\n";
		
		}
	}
	return returnValue;
}

public string GetWhereClauseForColumnsEqualsSqlParameterList(ColumnSchemaCollection sourceTableColumns)
{
	string returnValue = string.Empty;
	for (int i = 0; i < sourceTableColumns.Count; i++) 
	{
		returnValue += (i == 0 ? "\t" : "\r\n\tAND ") + "[" + sourceTableColumns[i].Table.Name + "].[" + sourceTableColumns[i].Name + "] = @" + sourceTableColumns[i].Name;
	}
	return returnValue;
}

public string GetUpdateClauseForColumnsEqualsSqlParameterList(ColumnSchemaCollection sourceTableColumns)
{
	string returnValue = string.Empty;
	for (int i = 0; i < sourceTableColumns.Count; i++) 
	{
		returnValue += "\t[" + sourceTableColumns[i].Table.Name + "].[" + sourceTableColumns[i].Name + "] = @" + sourceTableColumns[i].Name;
		if(i != sourceTableColumns.Count - 1) 
		{
			returnValue += ",\r\n";	
		}
	}
	return returnValue;
}

public string GetSqlParameterStatement(ColumnSchema column)
{
	return GetSqlParameterStatement(column, false);
}

public string GetSqlParameterStatement(ColumnSchema column, bool isOutput) 
{
	return 	GetSqlParameterStatement(column, isOutput, true);
}

// this was copied and modified from CodeSmith's SqlCodeTemplate.cs file to fix an error... see markings below.
public string GetSqlParameterStatement(ColumnSchema column, bool isOutput, bool includeAtSign)
{
	return GetSqlParameterStatement(column, isOutput, includeAtSign, false);
}

// Modified to include brackets around column name for output table in case a column name is a SQL reserved word
public string GetSqlParameterStatement(ColumnSchema column, bool isOutput, bool includeAtSign, bool includeBrackets)
{
	string param = "";
	if(includeAtSign)
	{
		param += "@";
	}

	if (includeBrackets)
	{
		param += "[" + column.Name + "]" + " " + column.NativeType;
	}
	else
	{
		param += column.Name + " " + column.NativeType;
	}
	
	if (!this.IsUserDefinedType(column))
	{
		switch (column.DataType)
		{
			case DbType.Decimal:
			{
				if (column.NativeType.Trim().ToLower() != "real")		// mjj.sn
				{														// mjj.en
					param += "(" + column.Precision + ", " + column.Scale + ")";
				}														// mjj.n
				break;
			}
			case DbType.AnsiString:
			case DbType.AnsiStringFixedLength:
			case DbType.String:
			case DbType.StringFixedLength:
			{
				if (column.NativeType != "text" && column.NativeType != "ntext")
				{
					if (column.Size > 0)
					{
						param += "(" + column.Size + ")";
					}
                    else
                    {
                            param += "(MAX)";
                    }
				}
				break;
			}
            case DbType.Binary:
            {
                if (column.NativeType == "varbinary")
                {
                    if (column.Size > 0)
                    {
                        param += "(" + column.Size + ")";
                    }
                    else
                    {
                        param += "(MAX)";
                    }
                }
                break;
            }
		}
	}
	
	if (isOutput)
	{
		param += " OUTPUT";
	}
	
	return param;
}