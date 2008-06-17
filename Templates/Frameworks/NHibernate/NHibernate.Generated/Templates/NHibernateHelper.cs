using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Text;

using CodeSmith.Engine;
using SchemaExplorer;

public enum NHibernateVersion
{
    OnePointTwo,
    TwoPointZero
}

public class NHibernateHelper : CodeTemplate
{
	public string GetCriterionNamespace(NHibernateVersion version)
	{
		switch(version)
		{
			case NHibernateVersion.OnePointTwo:
				return "NHibernate.Expression";
					
			case NHibernateVersion.TwoPointZero:
				return "NHibernate.Criterion";
				
			default:
				throw new Exception("Invalid NHibernateVersion");
			
		}
	}
	
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
		if (!String.IsNullOrEmpty(tablePrefix) && tableName.StartsWith(tablePrefix))
            tableName = tableName.Remove(0, tablePrefix.Length);
				
		if(tableName.EndsWith("es"))
			tableName = tableName.Substring(0, tableName.Length-2);
		else if(tableName.EndsWith("s") && !tableName.EndsWith("ss"))
			tableName = tableName.Substring(0, tableName.Length-1);
			
		return StringUtil.ToPascalCase(tableName);
	}
	protected string tablePrefix = String.Empty;
	
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

	/* REMOVE ME
	public string GetPrimaryKeyMethodParameters(MemberColumnSchemaCollection mcsc)
	{
		StringBuilder result = new StringBuilder();
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
	*/

	#endregion
	
	#region Method Creation Methods
	
	public string GetMethodParameters(List<MemberColumnSchema> mcsList, bool isDeclaration)
	{
		StringBuilder result = new StringBuilder();
		bool isFirst = true;
		foreach(MemberColumnSchema mcs in mcsList)
		{
			if(isFirst)
				isFirst = false;
			else
				result.Append(", ");
			if(isDeclaration)
			{
				result.Append(mcs.SystemType.ToString());
				result.Append(" ");
			}
			result.Append(GetVariableName(mcs.Name));
		}
		return result.ToString();
	}
	public string GetMethodParameters(MemberColumnSchemaCollection mcsc, bool isDeclaration)
	{
		List<MemberColumnSchema> mcsList = new List<MemberColumnSchema>();
        for (int x = 0; x < mcsc.Count; x++)
            mcsList.Add(mcsc[x]);
        return GetMethodParameters(mcsList, isDeclaration);
	}
	public string GetMethodDeclaration(SearchCriteria sc)
	{
		StringBuilder result = new StringBuilder();
		result.Append("GetBy");
		foreach(MemberColumnSchema mcs in sc.Items)
			result.Append(GetPropertyName(mcs.Name));
		result.Append("(");
		result.Append(GetMethodParameters(sc.Items, true));
		result.Append(")");
		return result.ToString();
	}
	public string GetPrimaryKeyCallParameters(List<MemberColumnSchema> mcsList)
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		for(int x=0; x<mcsList.Count; x++)
		{
			if(x>0)
				result.Append(", ");
			result.Append(String.Format("{0}.Parse(keys[{1}])", mcsList[x].SystemType, x));
		}
		return result.ToString();
	}
	
	#endregion
	
	public string GetForeignTableName(MemberColumnSchema mcs, TableSchema table)
	{
		foreach(TableKeySchema tks in table.ForeignKeys)
			if(tks.ForeignKeyMemberColumns.Contains(mcs))
				return tks.PrimaryKeyTable.Name;
		throw new Exception(String.Format("Could not find Column {0} in Table {1}'s ForeignKeys.", mcs.Name, table.Name));
	}
}

#region SearchCriteria Class

public class SearchCriteria
{
    #region Static Content

    public static List<SearchCriteria> GetAllSearchCriteria(TableSchema table)
    {
        TableSearchCriteria tsc = new TableSearchCriteria(table);
        return tsc.GetAllSearchCriteria();
    }
    public static List<SearchCriteria> GetPrimaryKeySearchCriteria(TableSchema table)
    {
        TableSearchCriteria tsc = new TableSearchCriteria(table);
        return tsc.GetPrimaryKeySearchCriteria();
    }
    public static List<SearchCriteria> GetForeignKeySearchCriteria(TableSchema table)
    {
        TableSearchCriteria tsc = new TableSearchCriteria(table);
        return tsc.GetForeignKeySearchCriteria();
    }
    public static List<SearchCriteria> GetIndexSearchCriteria(TableSchema table)
    {
        TableSearchCriteria tsc = new TableSearchCriteria(table);
        return tsc.GetIndexSearchCriteria();
    }

    #endregion

    #region Declarations

    protected List<MemberColumnSchema> mcsList;

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

    protected void Add(MemberColumnSchema item)
    {
        mcsList.Add(item);
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        bool isFirst = true;

        foreach (MemberColumnSchema mcs in mcsList)
        {
            sb.Append(mcs.Name);
            if (isFirst)
                isFirst = false;
            else
                sb.Append("|");
        }

        return sb.ToString();
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
    protected string Key
    {
        get { return this.ToString(); }
    }

    #endregion

    #region Internal Classes

    internal class TableSearchCriteria
    {
        #region Declarations

        protected TableSchema table;

        #endregion

        #region Constructor

        public TableSearchCriteria(TableSchema sourceTable)
        {
            this.table = sourceTable;
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