public bool IsReadOnlyEntity(TableSchema table)
{
	return !table.HasPrimaryKey;	
}

public void SafeCreateDirectory(string path)
{
	if (!System.IO.Directory.Exists(path))
	{
		System.IO.Directory.CreateDirectory(path);
	}
}

public bool ShouldUseTable(TableSchema table, TableSchemaCollection excludedTables)
{
	bool useTable = true;
	
	if(excludedTables != null)
	{
		if(excludedTables.Count > 0)
		{
			useTable = !excludedTables.Contains(table);
		}
	}	
	return useTable;
}

public bool ShouldUseColumn(ColumnSchema column)
{
	bool useColumn = true;
	
	if(column.NativeType == "timestamp")
	{
		useColumn = false;	
	}
	
	return useColumn;
}

public ColumnSchemaCollection GetValidColumns(ColumnSchemaCollection allColumns)
{
	ColumnSchemaCollection columns = new ColumnSchemaCollection();
	
	foreach(ColumnSchema column in allColumns)
	{
		if(ShouldUseColumn(column))
		{
			columns.Add(column);	
		}
	}
	
	return columns;
}

public ColumnSchemaCollection GetValidColumns(TableSchema table)
{
	ColumnSchemaCollection columns = new ColumnSchemaCollection();
	
	foreach(ColumnSchema column in table.Columns)
	{
		if(ShouldUseColumn(column))
		{
			columns.Add(column);	
		}
	}
	
	return columns;
}

/// <summary>
/// Copy the specified file.
/// </summary>
public void SafeCopyFile(string path, string destination)
{
	System.IO.FileInfo file1 = new System.IO.FileInfo(path);
	file1.CopyTo(destination, true);
}

public string GetFriendlyColumnName(ColumnSchema column)
{
	if(column.ExtendedProperties["NSFx_FriendlyName"] != null) {
		return (string)column.ExtendedProperties["NSFx_FriendlyName"].Value;
	}
	return GetPropertyName(column);
}

public string GetPropertyName(ColumnSchema column)
{
	// If there is an extended property the overrides the property name, use it.
	if(column.ExtendedProperties["NSFx_EntityPropertyName"] != null) {
		return (string)column.ExtendedProperties["NSFx_EntityPropertyName"].Value;
	}
	// Remove extra spaces that may exist in the DB column name.
	string propertyName = column.Name.Replace(" ", "");
	// If the property is a non-composite FK to another table, and it doesn't end with "Id", then it's possible that
	// the id property and entity property will both have the same name. So, add "Id" to the end of the id property. 
	if ((!propertyName.ToLower().EndsWith("id")) && IsColumnANonCompositeFK(column))
	{
		propertyName += "Id";
	}
	// If the DB has a column that is named the same as the table name, then C# will have a problem with a property having the 
	// same name as the class. Thus, append "value" to the property.
	if(propertyName == GetClassName(column.Table)) 
	{
		propertyName += "Value";	
	}
	
	return propertyName;
}

public bool IsColumnANonCompositeFK(ColumnSchema columnSchema)
{
	bool returnValue = false;
	if (columnSchema.IsForeignKeyMember)
	{
		foreach(SchemaExplorer.TableKeySchema foreignKey in columnSchema.Table.ForeignKeys) 
		{
			if (foreignKey.ForeignKeyMemberColumns.Count == 1)
			{
				if (foreignKey.ForeignKeyMemberColumns[0].Name == columnSchema.Name)
				{
					returnValue = true;
					break;
				}
			}
		}
	}
	return returnValue;	
}


public string GetClassName(TableSchema table)
{
	if(table.ExtendedProperties["NSFx_EntityName"] != null) 
	{
		return (string)table.ExtendedProperties["NSFx_EntityName"].Value;
	}

	string className = table.Name;
	
	if(StringUtil.IsPlural(className))
	{
		className = StringUtil.ToSingular(className);
	}
	className = TrimTablePrefix(className);
	
	className = className.Replace(" ", "");

	return className;
}

public string TrimTablePrefix(string name) 
{
	string returnName = name;	
	
	if(TablePrefixes != null) 
	{
		string [] tablePrefixes = TablePrefixes.Split(',');
		foreach(string tablePrefix in tablePrefixes)
		{
			if(returnName.StartsWith(tablePrefix))
			{
				returnName = returnName.Substring(tablePrefix.Length, returnName.Length - tablePrefix.Length);
				break;
			}
		}
	}
	
	return returnName;
}

public string GetPluralClassName(TableSchema table)
{
	if(table.ExtendedProperties["NSFx_EntityPluralName"] != null) 
	{
		return (string)table.ExtendedProperties["NSFx_EntityPluralName"].Value;
	}
	
	string className = table.Name;
	className = TrimTablePrefix(className);
	
	className = className.Replace(" ", "");
	
	if(StringUtil.IsSingular(className))
	{
		className = StringUtil.ToPlural(className);	
	}
	
	return className;
}