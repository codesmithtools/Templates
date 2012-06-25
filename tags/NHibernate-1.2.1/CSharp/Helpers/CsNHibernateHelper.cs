using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Text;
using CodeSmith.Engine;
using SchemaExplorer;
using NHibernateHelper;

public class CsNHibernateHelper : NHibernateHelper.NHibernateHelper
{
	private MapCollection _keyWords;
	public MapCollection KeyWords
	{
		get
		{
			if (_keyWords == null)
			{
				string path;
				Map.TryResolvePath("CSharpKeyWordEscape", this.CodeTemplateInfo.DirectoryName, out path);
				_keyWords = Map.Load(path);
			}
			return _keyWords;
		}
	}
	
	public string GetInitialization(ColumnSchema column)
    {
		if(column.AllowDBNull)
			return "null";
		
		Type type = column.SystemType;
        string result;

        if (type.Equals(typeof(String)))
            result = "String.Empty";
        else if (type.Equals(typeof(DateTime)))
            result = "new DateTime()";
        else if (type.Equals(typeof(Decimal)))
            result = "default(Decimal)";
        else if (type.Equals(typeof(Guid)))
            result = "Guid.Empty";
        else if (type.IsPrimitive)
            result = String.Format("default({0})", type.Name.ToString());
        else
            result = "null";
        return result;
    }
	
	public string GetMethodParameters(EntityManager entityManager, List<MemberColumnSchema> mcsList, bool isDeclaration)
	{
		StringBuilder result = new StringBuilder();
		bool isFirst = true;
		foreach (MemberColumnSchema mcs in mcsList)
		{
			if (isFirst)
				isFirst = false;
			else
				result.Append(", ");
			if (isDeclaration)
			{
				result.Append(mcs.SystemType.ToString());
				result.Append(" ");
			}
			result.Append(KeyWords[entityManager.GetEntityBaseFromColumn(mcs).VariableName]);
		}
		return result.ToString();
	}
	public string GetMethodParameters(EntityManager entityManager, MemberColumnSchemaCollection mcsc, bool isDeclaration)
	{
		List<MemberColumnSchema> mcsList = new List<MemberColumnSchema>();
		for (int x = 0; x < mcsc.Count; x++)
			mcsList.Add(mcsc[x]);
		return GetMethodParameters(entityManager, mcsList, isDeclaration);
	}
	public string GetMethodDeclaration(EntityManager entityManager, SearchCriteria sc)
	{
		StringBuilder result = new StringBuilder();
		result.Append(sc.MethodName);
		result.Append("(");
		result.Append(GetMethodParameters(entityManager, sc.Items, true));
		result.Append(")");
		return result.ToString();
	}
	public string GetPrimaryKeyCallParameters(List<MemberColumnSchema> mcsList)
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		for (int x = 0; x < mcsList.Count; x++)
		{
			if (x > 0)
				result.Append(", ");
	
			if (mcsList[x].SystemType == typeof(Guid))
				result.AppendFormat("new {0}(keys[{1}])", mcsList[x].SystemType, x);
			else if (mcsList[x].SystemType == typeof(string))
				result.AppendFormat("keys[{0}]", x);
			else
				result.AppendFormat("{0}.Parse(keys[{1}])", mcsList[x].SystemType, x);
		}
		return result.ToString();
	}
}
