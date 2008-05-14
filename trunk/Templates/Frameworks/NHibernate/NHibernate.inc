<script runat="template">
private Regex cleanRegEx = new Regex(@"\s+|_|-|\.", RegexOptions.Compiled);
private Regex cleanID = new Regex(@"(_ID|_id|_Id|\.ID|\.id|\.Id|ID|Id)", RegexOptions.Compiled);

public string CleanName(string name)
{
	return cleanRegEx.Replace(name, "");
}

public string CamelCase(string name)
{
	string output = CleanName(name);
	return char.ToLower(output[0]) + output.Substring(1);
}

public string PascalCase(string name)
{
	string output = CleanName(name);
	return char.ToUpper(output[0]) + output.Substring(1);
}

public string MakePlural(string name)
{
	Regex plural1 = new Regex("(?<keep>[^aeiou])y$");
	Regex plural2 = new Regex("(?<keep>[aeiou]y)$");
	Regex plural3 = new Regex("(?<keep>[sxzh])$");
	Regex plural4 = new Regex("(?<keep>[^sxzhy])$");

	if(plural1.IsMatch(name))
		return plural1.Replace(name, "${keep}ies");
	else if(plural2.IsMatch(name))
		return plural2.Replace(name, "${keep}s");
	else if(plural3.IsMatch(name))
		return plural3.Replace(name, "${keep}es");
	else if(plural4.IsMatch(name))
		return plural4.Replace(name, "${keep}s");

	return name;
}

public string MakeSingle(string name)
{
	Regex plural1 = new Regex("(?<keep>[^aeiou])ies$");
	Regex plural2 = new Regex("(?<keep>[aeiou]y)s$");
	Regex plural3 = new Regex("(?<keep>[sxzh])es$");
	Regex plural4 = new Regex("(?<keep>[^sxzhyu])s$");

	if(plural1.IsMatch(name))
		return plural1.Replace(name, "${keep}y");
	else if(plural2.IsMatch(name))
		return plural2.Replace(name, "${keep}");
	else if(plural3.IsMatch(name))
		return plural3.Replace(name, "${keep}");
	else if(plural4.IsMatch(name))
		return plural4.Replace(name, "${keep}");

	return name;
}

public bool IsManyToManyTable(TableSchema table)
{
	if (table.Columns.Count == 2 && table.PrimaryKey != null && table.PrimaryKey.MemberColumns.Count == 2 && table.ForeignKeys.Count == 2)
//	if (table.Columns.Count >= 2 && table.PrimaryKey != null && table.PrimaryKey.MemberColumns.Count == 2 && table.ForeignKeys.Count >= 2)
		return true;
	else
		return false;
}

public bool IsOneToOneTable(TableKeySchema primaryKey)
{
	if (primaryKey.ForeignKeyMemberColumns[0].IsPrimaryKeyMember)
	{
		// Check if the current table is the primary key with one-to-one mappings to several tables.
		// If so, generate polymorphic classes using joined sub-classes & corresponding C# classes.
		int count = 0;
		foreach(TableKeySchema pk in SourceTable.PrimaryKeys)
		{
			// check if other end of the link is the primary key for the table (if count > 1 then it may
			// be a composite primary key and most likely a many-to-many link table which can be ignored)
			if (pk.ForeignKeyMemberColumns[0].IsPrimaryKeyMember && pk.ForeignKeyTable.PrimaryKey.MemberColumns.Count == 1)
				count++;
		}
		if (count > 1)
			return false;

		return true;
	}
	return false;
}

public bool IsSubClassTable(TableKeySchema primaryKey)
{
	if (primaryKey.ForeignKeyMemberColumns[0].IsPrimaryKeyMember)
	{
		// Check if the current table is the primary key with one-to-one mappings to several tables.
		// If so, generate polymorphic classes using joined sub-classes & corresponding C# classes.
		int count = 0;
		foreach(TableKeySchema pk in SourceTable.PrimaryKeys)
		{
			// check if other end of the link is the primary key for the table (if count > 1 then it may
			// be a composite primary key and most likely a many-to-many link table which can be ignored)
			if (pk.ForeignKeyMemberColumns[0].IsPrimaryKeyMember && pk.ForeignKeyTable.PrimaryKey.MemberColumns.Count == 1)
				count++;
		}
		if (count > 1)
			return true;

		return false;
	}
	return false;
}

public string CSharpType(ColumnSchema column)
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

public string NHibernateType(ColumnSchema column)
{
	if (column.Name.EndsWith("TypeCode")) return column.Name;

	switch (column.DataType)
	{
		case DbType.AnsiString: return "String";
		case DbType.AnsiStringFixedLength: return "String";
		case DbType.Binary: return "Byte[]";
		case DbType.Boolean: return "Boolean";
		case DbType.Byte: return "Byte";
		case DbType.Currency: return "Decimal";
		case DbType.Date: return "DateTime";
		case DbType.DateTime: return "DateTime";
		case DbType.Decimal: return "Decimal";
		case DbType.Double: return "Double";
		case DbType.Guid: return "Guid";
		case DbType.Int16: return "Int16";
		case DbType.Int32: return "Int32";
		case DbType.Int64: return "Int64";
		case DbType.Object: return "BinaryBlob";
		case DbType.SByte: return "Byte";
		case DbType.Single: return "Single";
		case DbType.String: return "String";
		case DbType.StringFixedLength: return "String";
		case DbType.Time: return "DateTime";
		case DbType.UInt16: return "Int16";
		case DbType.UInt32: return "Int32";
		case DbType.UInt64: return "Int64";
		case DbType.VarNumeric: return "Decimal";
		default:
		{
			return "__UNKNOWN__" + column.NativeType;
		}
	}
}

private Regex sqlCharacters = new Regex(@"[\s|~|-|!|{|%|}|\^|'|&|.|\(|\\|\)|`]", RegexOptions.Compiled);
private Regex sqlReserved = new Regex(@"^(ADD|EXCEPT|PERCENT|ALL|EXEC|PLAN|ALTER|EXECUTE|PRECISION|AND|EXISTS|PRIMARY|ANY|EXIT|PRINT|AS|FETCH|PROC|ASC|FILE|PROCEDURE|AUTHORIZATION|FILLFACTOR|PUBLIC|BACKUP|FOR|RAISERROR|BEGIN|FOREIGN|READ|BETWEEN|FREETEXT|READTEXT|BREAK|FREETEXTTABLE|RECONFIGURE|BROWSE|FROM|REFERENCES|BULK|FULL|REPLICATION|BY|FUNCTION|RESTORE|CASCADE|GOTO|RESTRICT|CASE|GRANT|RETURN|CHECK|GROUP|REVOKE|CHECKPOINT|HAVING|RIGHT|CLOSE|HOLDLOCK|ROLLBACK|CLUSTERED|IDENTITY|ROWCOUNT|COALESCE|IDENTITY_INSERT|ROWGUIDCOL|COLLATE|IDENTITYCOL|RULE|COLUMN|IF|SAVE|COMMIT|IN|SCHEMA|COMPUTE|INDEX|SELECT|CONSTRAINT|INNER|SESSION_USER|CONTAINS|INSERT|SET|CONTAINSTABLE|INTERSECT|SETUSER|CONTINUE|INTO|SHUTDOWN|CONVERT|IS|SOME|CREATE|JOIN|STATISTICS|CROSS|KEY|SYSTEM_USER|CURRENT|KILL|TABLE|CURRENT_DATE|LEFT|TEXTSIZE|CURRENT_TIME|LIKE|THEN|CURRENT_TIMESTAMP|LINENO|TO|CURRENT_USER|LOAD|TOP|CURSOR|NATIONAL||TRAN|DATABASE|NOCHECK|TRANSACTION|DBCC|NONCLUSTERED|TRIGGER|DEALLOCATE|NOT|TRUNCATE|DECLARE|NULL|TSEQUAL|DEFAULT|NULLIF|UNION|DELETE|OF|UNIQUE|DENY|OFF|UPDATE|DESC|OFFSETS|UPDATETEXT|DISK|ON|USE|DISTINCT|OPEN|USER|DISTRIBUTED|OPENDATASOURCE|VALUES|DOUBLE|OPENQUERY|VARYING|DROP|OPENROWSET|VIEW|DUMMY|OPENXML|WAITFOR|DUMP|OPTION|WHEN|ELSE|OR|WHERE|END|ORDER|WHILE|ERRLVL|OUTER|WITH|ESCAPE|OVER|WRITETEXT)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
private Regex sqlFuture = new Regex(@"^(ABSOLUTE|FOUND|PRESERVE|ACTION|FREE|PRIOR|ADMIN|GENERAL|PRIVILEGES|AFTER|GET|READS|AGGREGATE|GLOBAL|REAL|ALIAS|GO|RECURSIVE|ALLOCATE|GROUPING|REF|ARE|HOST|REFERENCING|ARRAY|HOUR|RELATIVE|ASSERTION|IGNORE|RESULT|AT|IMMEDIATE|RETURNS|BEFORE|INDICATOR|ROLE|BINARY|INITIALIZE|ROLLUP|BIT|INITIALLY|ROUTINE|BLOB|INOUT|ROW|BOOLEAN|INPUT|ROWS|BOTH|INT|SAVEPOINT|BREADTH|INTEGER|SCROLL|CALL|INTERVAL|SCOPE|CASCADED|ISOLATION|SEARCH|CAST|ITERATE|SECOND|CATALOG|LANGUAGE|SECTION|CHAR|LARGE|SEQUENCE|CHARACTER|LAST|SESSION|CLASS|LATERAL|SETS|CLOB|LEADING|SIZE|COLLATION|LESS|SMALLINT|COMPLETION|LEVEL|SPACE|CONNECT|LIMIT|SPECIFIC|CONNECTION|LOCAL|SPECIFICTYPE|CONSTRAINTS|LOCALTIME|SQL|CONSTRUCTOR|LOCALTIMESTAMP|SQLEXCEPTION|CORRESPONDING|LOCATOR|SQLSTATE|CUBE|MAP|SQLWARNING|CURRENT_PATH|MATCH|START|CURRENT_ROLE|MINUTE|STATE|CYCLE|MODIFIES|STATEMENT|DATA|MODIFY|STATIC|DATE|MODULE|STRUCTURE|DAY|MONTH|TEMPORARY|DEC|NAMES|TERMINATE|DECIMAL|NATURAL|THAN|DEFERRABLE|NCHAR|TIME|DEFERRED|NCLOB|TIMESTAMP|DEPTH|NEW|TIMEZONE_HOUR|DEREF|NEXT|TIMEZONE_MINUTE|DESCRIBE|NO|TRAILING|DESCRIPTOR|NONE|TRANSLATION|DESTROY|NUMERIC|TREAT|DESTRUCTOR|OBJECT|TRUE|DETERMINISTIC|OLD|UNDER|DICTIONARY|ONLY|UNKNOWN|DIAGNOSTICS|OPERATION|UNNEST|DISCONNECT|ORDINALITY|USAGE|DOMAIN|OUT|USING|DYNAMIC|OUTPUT|VALUE|EACH|PAD|VARCHAR|END-EXEC|PARAMETER|VARIABLE|EQUALS|PARAMETERS|WHENEVER|EVERY|PARTIAL|WITHOUT|EXCEPTION|PATH|WORK|EXTERNAL|POSTFIX|WRITE|FALSE|PREFIX|YEAR|FIRST|PREORDER|ZONE|FLOAT|PREPARE)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

public string SqlIdentifier(string sqlIdentifier)
{
	if (sqlCharacters.IsMatch(sqlIdentifier) || sqlReserved.IsMatch(sqlIdentifier) || sqlFuture.IsMatch(sqlIdentifier))
		return String.Format("`{0}`", sqlIdentifier);
	else
		return sqlIdentifier;
}

public string TableClass(TableSchema table)
{
	string className = table.Name;
	if (className.StartsWith(RemoveTablePrefix))
		className = className.Substring(RemoveTablePrefix.Length);
	return String.Format("{0}", MakeSingle(PascalCase(className)));
}

public string TableClassFull(TableSchema table)
{
	return String.Format("{0}.{1}, {2}", Namespace, TableClass(table), Assembly);
}

public string TableCollection(TableSchema table)
{
	string className = table.Name;
	if (className.StartsWith(RemoveTablePrefix))
		className = className.Substring(RemoveTablePrefix.Length);
	return String.Format("{0}", MakePlural(PascalCase(className)));
}
public string ClassName(TableSchema table)
{
	return TableClass(table);
}
public string ClassNameAtt(TableSchema table)
{
	return String.Format(" name=\"{0}\"", TableClassFull(table));
}
public string ClassTable(TableSchema table)
{
	return table.Name;
}
public string ClassTableAtt(TableSchema table)
{
	return String.Format(" table=\"{0}\"", SqlIdentifier(table.Name));
}

public string IdMemberName(TableSchema table)
{
	if (ForceId)
		return "_id";
	else
		return MemberName(table.PrimaryKey.MemberColumns[0]);
}
public string IdName(TableSchema table)
{
	if (ForceId)
		return "Id";
	else
		return PropertyName(table.PrimaryKey.MemberColumns[0]);
}
public string IdNameAtt(TableSchema table)
{
	return String.Format(" name=\"{0}\"", IdName(table));
}
public string IdMemberType(TableSchema table)
{
	return MemberType(table.PrimaryKey.MemberColumns[0]);
}
public string IdType(TableSchema table)
{
	return PropertyType(table.PrimaryKey.MemberColumns[0]);
}
public string IdTypeAtt(TableSchema table)
{
	return String.Format(" type=\"{0}\"", PropertyType(table.PrimaryKey.MemberColumns[0]));
}
public string IdUnsavedValueAtt(TableSchema table)
{
	ColumnSchema column = table.PrimaryKey.MemberColumns[0];
	if (column.Size == 0)
		return String.Format(" unsaved-value=\"{0}\"", 0);
	else
		return String.Format(" unsaved-value=\"{0}\"", "null");
}

public string PropertyName(ColumnSchema column)
{
	return PascalCase(column.Name);
}
public string MemberName(ColumnSchema column)
{
	return "_" + CamelCase(column.Name);
}
public string ParameterName(ColumnSchema column)
{
	return CamelCase(column.Name);
}
public string PropertyNameAtt(ColumnSchema column)
{
	return String.Format(" name=\"{0}\"", PropertyName(column));
}
public string PropertyType(ColumnSchema column)
{
	return NHibernateType(column);
}
public string MemberType(ColumnSchema column)
{
	return CSharpType(column);
}
public string PropertyTypeAtt(ColumnSchema column)
{
	return String.Format(" type=\"{0}\"", PropertyType(column));
}

public string ColumnName(ColumnSchema column)
{
	return column.Name;
}
public string ColumnNameAtt(ColumnSchema column)
{
	return String.Format(" name=\"{0}\"", SqlIdentifier(ColumnName(column)));
}
public string ColumnLength(ColumnSchema column)
{
	if (column.Size > 0)
		return column.Size.ToString();
	else
		return String.Empty;
}
public string ColumnLengthAtt(ColumnSchema column)
{
	if (column.Size > 0)
		return String.Format(" length=\"{0}\"", column.Size);
	else
		return String.Empty;
}
public string ColumnSqlTypeAtt(ColumnSchema column)
{
	return String.Format(" sql-type=\"{0}\"", column.NativeType);
}
public string ColumnNotNullAtt(ColumnSchema column)
{
	return String.Format(" not-null=\"{0}\"", (!column.AllowDBNull).ToString().ToLower());
}
public string ColumnUniqueAtt(ColumnSchema column)
{
	if (column.IsUnique)
		return String.Format(" unique=\"{0}\"", column.IsUnique.ToString().ToLower());
	else
		return String.Empty;
}
public string ColumnIndexAtt(TableSchema table, ColumnSchema column)
{
	foreach(IndexSchema index in table.Indexes)
	{
		if (index.MemberColumns.Contains(column))
		{
			return String.Format(" index=\"{0}\"", index.Name);
		}
	}
	return String.Empty;
}

public string ManyToOneName(TableKeySchema foreignKey)
{
	string className = TableClass(foreignKey.PrimaryKeyTable);

	string thiskey = foreignKey.ForeignKeyMemberColumns[0].Name;
	string primarykey = foreignKey.PrimaryKeyMemberColumns[0].Name;

	string differentiator = thiskey.Replace(primarykey, "").Replace("ID","");
	
	string returnName = (differentiator == "" ? className : differentiator);

	return returnName;
}
public string ManyToOneMemberName(TableKeySchema foreignKey)
{
	return "_" + CamelCase(ManyToOneName(foreignKey));
}
public string ManyToOneParameterName(TableKeySchema foreignKey)
{
	return CamelCase(ManyToOneName(foreignKey));
}
public string ManyToOneNameAtt(TableKeySchema foreignKey)
{
	return String.Format(" name=\"{0}\"", ManyToOneName(foreignKey));
}
public string ManyToOneClass(TableKeySchema foreignKey)
{
	string className = TableClass(foreignKey.PrimaryKeyTable);

	return className;
}
public string ManyToOneClassAtt(TableKeySchema foreignKey)
{
	string className = TableClassFull(foreignKey.PrimaryKeyTable);

	return String.Format(" class=\"{0}\"", className);
}

public string OneToOneName(TableKeySchema primaryKey)
{
	string className = TableClass(primaryKey.ForeignKeyTable);

	string thiskey = primaryKey.PrimaryKeyMemberColumns[0].Name;
	string primarykey = primaryKey.ForeignKeyMemberColumns[0].Name;

	string differentiator = thiskey.Replace(primarykey, "");

	return className + differentiator;
}
public string OneToOneMemberName(TableKeySchema primaryKey)
{
	return "_" + CamelCase(OneToOneName(primaryKey));
}
public string OneToOneNameAtt(TableKeySchema primaryKey)
{
	return String.Format(" name=\"{0}\"", OneToOneName(primaryKey));
}
public string OneToOneClass(TableKeySchema primaryKey)
{
	string className = TableClass(primaryKey.ForeignKeyTable);

	return className;
}
public string OneToOneClassAtt(TableKeySchema primaryKey)
{
	string className = TableClassFull(primaryKey.ForeignKeyTable);

	return String.Format(" class=\"{0}\"", className);
}

public string JoinedSubclassName(TableKeySchema primaryKey)
{
	string className = TableClass(primaryKey.ForeignKeyTable);

	string thiskey = primaryKey.PrimaryKeyMemberColumns[0].Name;
	string primarykey = primaryKey.ForeignKeyMemberColumns[0].Name;

	string differentiator = thiskey.Replace(primarykey, "");

	return className + differentiator;
}
public string JoinedSubclassNameAtt(TableKeySchema primaryKey)
{
	string className = TableClassFull(primaryKey.ForeignKeyTable);
	return String.Format(" name=\"{0}\"", className);
}
public string JoinedSubclassTable(TableKeySchema primaryKey)
{
	return primaryKey.ForeignKeyTable.Name;
}
public string JoinedSubclassTableAtt(TableKeySchema primaryKey)
{
	return String.Format(" table=\"{0}\"", SqlIdentifier(primaryKey.ForeignKeyTable.Name));
}

public string CollectionName(TableKeySchema primaryKey)
{
//	string className = TableCollection(primaryKey.ForeignKeyTable);
	string className = primaryKey.ForeignKeyTable.Name;
	string thiskey = primaryKey.PrimaryKeyMemberColumns[0].Name;
	string primarykey = primaryKey.ForeignKeyMemberColumns[0].Name;

	string differentiator = primarykey.Replace(thiskey, "").Replace("ID","");

	return MakePlural(differentiator + className);
}
public string CollectionMemberName(TableKeySchema primaryKey)
{
	return "_" + CamelCase(CollectionName(primaryKey));
}
public string CollectionNameAtt(TableKeySchema primaryKey)
{
	string className = TableCollection(primaryKey.ForeignKeyTable);

	string thiskey = primaryKey.PrimaryKeyMemberColumns[0].Name;
	string primarykey = primaryKey.ForeignKeyMemberColumns[0].Name;

	string differentiator = primarykey.Replace(thiskey, "");

	return String.Format(" name=\"{0}\"", CollectionName(primaryKey));
}
public string CollectionType(TableKeySchema primaryKey)
{
	return "IList";
}
public string NewCollectionType(TableKeySchema primaryKey)
{
	return "new ArrayList()";
}
public string CollectionKeyColumnAtt(TableKeySchema primaryKey)
{
	ColumnSchema column = primaryKey.PrimaryKeyMemberColumns[0];
	return String.Format(" column=\"{0}\"", SqlIdentifier(column.Name));
}
public string CollectionSelfKeyColumnAtt(TableKeySchema primaryKey)
{
	ColumnSchema column = primaryKey.ForeignKeyMemberColumns[0];
	return String.Format(" column=\"{0}\"", SqlIdentifier(column.Name));
}
public string CollectionOneToManyClass(TableKeySchema primaryKey)
{
	return TableClass(primaryKey.ForeignKeyTable);
}
public string CollectionOneToManyClassAtt(TableKeySchema primaryKey)
{
	return String.Format(" class=\"{0}\"", TableClassFull(primaryKey.ForeignKeyTable));
}
public string CollectionManyToManyName(TableKeySchema primaryKey)
{
//	string className = String.Empty;

//	foreach(TableKeySchema tableKey in primaryKey.ForeignKeyTable.ForeignKeys)
//	{
//		className = TableCollection(tableKey.ForeignKeyTable);
//		if (tableKey.PrimaryKeyTable != SourceTable)
//		{
//			className = TableCollection(tableKey.PrimaryKeyTable);
//		}
//	}

//	string thiskey = primaryKey.PrimaryKeyMemberColumns[0].Name;
	string primarykey = primaryKey.ForeignKeyMemberColumns[0].Name;

//	string differentiator = primarykey.Replace(thiskey, "");
	
	string otherkey = String.Empty;
	foreach(ColumnSchema column in primaryKey.ForeignKeyTable.PrimaryKey.MemberColumns)
	{
		if (column.Name != primarykey)
		{
			otherkey = column.Name;
		}
	}
	
	string returnName = MakePlural(primarykey.Replace("ID","")+otherkey.Replace("ID",""));

	return returnName;
}
public string CollectionManyToManyMemberName(TableKeySchema primaryKey)
{
	return "_" + CamelCase(CollectionManyToManyName(primaryKey));
}
public string CollectionManyToManyNameAtt(TableKeySchema primaryKey)
{
	return String.Format(" name=\"{0}\"", CollectionManyToManyName(primaryKey));
}
public string CollectionManyToManyClass(TableKeySchema primaryKey)
{
	return TableClass(primaryKey.ForeignKeyTable);
}
public string CollectionManyToManyClassAtt(TableKeySchema primaryKey)
{
	return String.Format(" class=\"{0}\"", TableClassFull(primaryKey.PrimaryKeyTable));
}
public string CollectionTableAtt(TableKeySchema primaryKey)
{
	return String.Format(" table=\"{0}\"", SqlIdentifier(primaryKey.ForeignKeyTable.Name));
}
</script>
