using System;
using System.ComponentModel;
using System.Data;

using CodeSmith.Engine;
using SchemaExplorer;

public class NHibernateHelper : CodeTemplate
{
	#region Variable Name Methods
	
	public string GetPropertyName(string name)
	{
		return StringUtil.ToPascalCase(name);
	}
	public string GetPropertyNamePlural(String name)
	{
		return GetPropertyName(GetNamePlural(name));
	}
	
	public string GetPrivateVariableName(string name)
	{
		return "_" + GetVariableName(name);
	}
	public string GetPrivateVariableNamePlural(string name)
	{
		return GetPrivateVariableName(GetNamePlural(name));
	}
	
	public string GetVariableName(string name)
	{
		return StringUtil.ToCamelCase(name);
	}
	public string GetVariableNamePlural(string name)
	{
		return GetVariableName(GetNamePlural(name));
	}
	
	private string GetNamePlural(string name)
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		result.Append(name);
		
		if(!name.EndsWith("es") && name.EndsWith("s"))
			result.Append("es");
		else
			result.Append("s");
			
		return result.ToString();
	}
	
	#endregion
	
	#region ManyToMany Table Methods
	
	public TableSchema GetToManyTable(TableSchema manyToTable, TableSchema sourceTable)
	{
		TableSchema result = null;
		foreach(TableKeySchema key in manyToTable.ForeignKeys)
			if(!key.PrimaryKeyTable.Equals(sourceTable))
			{
				result = key.PrimaryKeyTable;
				break;
			}
		return result;
	}
	public bool IsManyToMany(TableSchema table)
	{
		// If there are 2 ForeignKeyColumns AND...
		// ...there are only two columns OR
		//    there are 3 columns and 1 is a primary key.
		return (table.ForeignKeyColumns.Count == 2
			&& ((table.Columns.Count == 2)
				|| (table.Columns.Count == 3 && table.PrimaryKey != null)));
	}
	
	#endregion
	
	public string GetCascade(MemberColumnSchema column)
	{
		return column.AllowDBNull ? "all" : "all-delete-orphan";
	}
	
	#region BusinessObject Methods
	
	public string GetInitialization(Type type)
	{
		string result;
		
		if(type.Equals(typeof(String)))
			result = "String.Empty";
		else if(type.Equals(typeof(DateTime)))
			result = "new DateTime()";
		else if(type.Equals(typeof(Decimal)))
			result = "default(Decimal)";
		else if(type.IsPrimitive)
			result = String.Format("default({0})", type.Name.ToString());
		else
			result = "null";
		return result;
	}
	public string GetBusinessBaseIdType(TableSchema table)
	{
		if(IsMutliColumnPrimaryKey(table.PrimaryKey))
			return "string";
		else
			return GetPrimaryKeyColumn(table.PrimaryKey).SystemType.ToString();
	}
	
	#endregion
	
	public string GetClassName(String tableName)
	{
		string result;
		if(tableName.EndsWith("es"))
			result = tableName.Substring(0, tableName.Length-2);
		else if(tableName.EndsWith("s") && !tableName.EndsWith("ss"))
			result = tableName.Substring(0, tableName.Length-1);
		else
			result = tableName;
		return StringUtil.ToPascalCase(result);
	}
	
	#region PrimaryKey Methods
	
	public MemberColumnSchema GetPrimaryKeyColumn(PrimaryKeySchema primaryKey)
	{
		if(primaryKey.MemberColumns.Count != 1)
			throw new System.ApplicationException("This method will only work on primary keys with exactly one member column.");
		return primaryKey.MemberColumns[0];
	}
	public bool IsMutliColumnPrimaryKey(PrimaryKeySchema primaryKey)
	{
		if(primaryKey.MemberColumns.Count == 0)
			throw new System.ApplicationException("This template will only work on primary keys with exactly one member column.");
			
		return (primaryKey.MemberColumns.Count > 1);
	}
	public string GetForeignKeyColumnClassName(MemberColumnSchema mcs, TableSchema table)
	{
		string result = String.Empty;
		foreach(TableKeySchema tks in table.ForeignKeys)
			if(tks.ForeignKeyMemberColumns.Contains(mcs))
			{
				result = GetPropertyName(tks.PrimaryKeyTable.Name);
				break;
			}
		return result;
	}
	public bool IsPrimaryKeyColumn(MemberColumnSchema mcs, TableSchema table)
	{
		bool result = false;
		foreach(MemberColumnSchema primaryKeyColumn in table.PrimaryKey.MemberColumns)
			if(primaryKeyColumn.Equals(mcs))
			{
				result = true;
				break;
			}
		return result;
	}

	public string GetPrimaryKeyMethodParameters(MemberColumnSchemaCollection mcsc)
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		for(int x=0; x<mcsc.Count; x++)
		{
			if(x>0)
				result.Append(", ");
			result.Append(mcsc[x].SystemType.ToString());
			result.Append(" ");
			result.Append(StringUtil.ToCamelCase(mcsc[x].Name));
		}
		return result.ToString();
	}
	public string GetPrimaryKeyCallParameters(MemberColumnSchemaCollection mcsc)
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		for(int x=0; x<mcsc.Count; x++)
		{
			if(x>0)
				result.Append(", ");
			result.Append(String.Format("{0}.Parse(keys[{1}])", mcsc[x].SystemType, x));
		}
		return result.ToString();
	}
	
	#endregion
}