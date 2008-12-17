using System;
using System.ComponentModel;
using System.Text;
using CodeSmith.BaseTemplates;
using CodeSmith.CustomProperties;
using CodeSmith.Engine;
using SchemaExplorer;

namespace CodeSmith.Csla {

	#region SqlStatementType enum

	public enum SqlStatementType {
		Select,
		SelectBy,
		Insert,
		Update,
		Delete
	}

	#endregion

	public class CslaSqlTemplateBase : SqlCodeTemplate {

		#region Constants

		private const string SelectStatementTemplate = "[SELECTCOLUMNS] [FROM] [WHERE]";
		private const string InsertStatementTemplate = "[INSERTCOLUMNS] [VALUES] [SETNEWPK] [SETNEWTIMESTAMP]";
		private const string UpdateStatementTemplate = "[UPDATE] [SETCOLUMNS] [WHERE] [SETNEWTIMESTAMP]";
		private const string DeleteStatementTemplate = "[DELETE] [WHERE]";
		private const string FetchCommandFormat = "dbo.usp_Sel{0}";
		private const string FetchByCommandFormat = "dbo.usp_Sel{0}By{1}";
		private const string InsertCommandFormat = "dbo.usp_{0}INS";
		private const string UpdateCommandFormat = "dbo.usp_{0}UPD";
		private const string DeleteCommandFormat = "dbo.usp_{0}DEL";
		private const string TimeStampColumn = "TableTimeStamp";
		//number of spaces to use for indentation, set to 0 to use tab indentation
		private const int IndentLevelSpaces = 0;

		#endregion

		#region Template Properties

		private TableSchema _sourceTable;
		[Category("1. General")]
		[Description("Required - Table that the procs should be generated for.")]
		public TableSchema SourceTable
		{
			get { return _sourceTable; }
			set { _sourceTable = value; }
		}

		private TableSchema _parentTable;
		[Category("1. General")]
		[Description("Optional - Default database that objects should be based on.")]
		public TableSchema ParentTable
		{
			get { return _parentTable; }
			set { _parentTable = value; }
		}

		private StringCollection _executeRoles = new StringCollection();
		[Category("1. General")]
		[Description("Optional - Database roles that proc execute rights should be granted to.")]
		public StringCollection ExecuteRoles
		{
			get { return _executeRoles; }
			set { _executeRoles = value; }
		}

		#endregion

		#region Public Methods

		public string GetProcDeclaration(SqlStatementType statementType)
		{
			return GetProcDeclaration(statementType, 0);
		}

		public string GetProcDeclaration(SqlStatementType statementType, int indentLevel)
		{
			string procName = GetProcName(statementType);
			StringBuilder sb = new StringBuilder(GetProcDropStatement(procName, indentLevel));
			
			sb.Append(Indent(indentLevel, true));
			sb.Append(GetProcCreateStatement(procName, statementType, indentLevel));

			if(ExecuteRoles.Count > 0) {
				sb.Append(Indent(indentLevel, true));
				foreach(string role in ExecuteRoles) {
					sb.Append(GetProcGrantExecuteStatement(procName, role, indentLevel));
				}
				sb.Append(Indent(indentLevel, true));
				sb.Append("GO");
			}
			
			return sb.ToString();
		}

		public static string GetProcDropStatement(string procName, int indentLevel)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Indent(indentLevel++, true));
			sb.Append(string.Format("IF objectProperty(object_id('{0}'), 'IsProcedure') = 1", procName));
			sb.Append(Indent(indentLevel, true));
			sb.Append(string.Format("DROP PROCEDURE {0}", procName));
			sb.Append(Indent(--indentLevel, true));
			sb.Append("GO");

			return sb.ToString();
		}

		public static string GetProcGrantExecuteStatement(string procName, string roleName, int indentLevel)
		{
			StringBuilder sb = new StringBuilder(Indent(indentLevel, true));
			sb.Append(string.Format("GRANT EXECUTE ON {0} TO {1}", procName, roleName));
			return sb.ToString();
		}

		public static bool IsCommaNecessary(ColumnSchemaCollection columns, int columnOrdinal)
		{
			return IsCommaNecessary(columns, columnOrdinal, true);
		}

		public static bool IsCommaNecessary(ColumnSchemaCollection columns, int columnOrdinal,
			bool ignoreAutoSetColumns)
		{
			//show a comma if:
			//	1. this is not the last column, and 
			//	2. this is not the second to last column in a table where the last column is 
			//		an auto set column, and it's to be ignored

			bool showComma = columnOrdinal < columns.Count - 1;

			if(showComma && ignoreAutoSetColumns) {
				showComma = !(columnOrdinal == columns.Count - 2 && IsColumnAutoSet(columns[columnOrdinal + 1]));
			}

			return showComma;
		}

		public static bool IsIdentity(ColumnSchema column)
		{
			return (bool) column.ExtendedProperties["CS_IsIdentity"].Value;
		}

		public static bool IsColumnAutoSet(ColumnSchema column)
		{
			return IsIdentity(column) || string.Compare(column.Name, TimeStampColumn, true) == 0;
		}

		public string GetSqlParameterStatement(ColumnSchema column, SqlStatementType statementType)
		{
			bool isOutput = false;

			if(statementType == SqlStatementType.Insert || statementType == SqlStatementType.Update) {
				isOutput = string.Compare(column.Name, TimeStampColumn, true) == 0;
			}
			if(!isOutput && statementType == SqlStatementType.Insert) {
				isOutput = IsIdentity(column);
			}
			string parameterStatement = GetSqlParameterStatement(column, isOutput);

			if(isOutput) {
				parameterStatement = parameterStatement.Replace("@", "@New");
			}

			return parameterStatement;
		}

		#endregion

		#region Protected Methods

		protected string GetProcCreateStatement(string procName, SqlStatementType statementType, 
			int indentLevel)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Indent(indentLevel, true));
			sb.Append("CREATE PROCEDURE ");
			sb.Append(procName);
			sb.Append(Indent(indentLevel++, true));
			sb.Append("(");

			ColumnSchemaCollection columns;
			bool ignoreAutoSetColumns = true;
			switch(statementType) {
				case SqlStatementType.Insert:
				case SqlStatementType.Update:
					columns = SourceTable.Columns;
					ignoreAutoSetColumns = false;
					break;
				case SqlStatementType.SelectBy:
					CheckForPrimaryKey(ParentTable);
					columns = ParentTable.PrimaryKey.MemberColumns;
					break;
				default:
					CheckForPrimaryKey(SourceTable);
					columns = SourceTable.PrimaryKey.MemberColumns;
					break;
			}
			
			for(int i = 0; i < columns.Count; i++) {
				sb.Append(Indent(indentLevel, true));
				sb.Append(GetSqlParameterStatement(columns[i], statementType));
				if(IsCommaNecessary(columns, i, ignoreAutoSetColumns)) {
					sb.Append(",");
				}
			}

			sb.Append(Indent(--indentLevel, true));
			sb.Append(")");
			sb.Append(Indent(indentLevel++, true));
			sb.Append("AS");
			sb.Append(GetSqlStatement(statementType, indentLevel));
			sb.Append(Indent(--indentLevel, true));
			sb.Append("GO");

			return sb.ToString();
		}

		protected string GetSqlStatement(SqlStatementType statementType, int indentLevel)
		{
			switch(statementType) {
				case SqlStatementType.Select:
					return this.GetSelectStatement(false, indentLevel);
				case SqlStatementType.SelectBy:
					return this.GetSelectStatement(true, indentLevel);
				case SqlStatementType.Insert:
					return GetInsertStatement(indentLevel);
				case SqlStatementType.Update:
					return GetUpdateStatement(indentLevel);
				case SqlStatementType.Delete:
					return GetDeleteStatement(indentLevel);
				default:
					throw new ArgumentOutOfRangeException("statementType", statementType.ToString(), "No statement template has been defined for this SqlStatementType");
			}
		}

		protected string GetSelectStatement(bool isSelectBy, int indentLevel)
		{
			StringBuilder select = new StringBuilder(Indent(indentLevel++, true));
			select.Append("SELECT");
			select.Append(GetColumnList(false, indentLevel));

			StringBuilder statement = new StringBuilder(SelectStatementTemplate);
			statement.Replace("[SELECTCOLUMNS]", select.ToString());

			string from = Indent(--indentLevel, true) + "FROM" + Indent(indentLevel + 1, true) + SourceTable.Name;
			statement.Replace("[FROM]", from);

			TableSchema keyTable;
			if(isSelectBy) {
				keyTable = ParentTable;
			}
			else {
				keyTable = SourceTable;
			}
			statement.Replace("[WHERE]", GetPKWhereClause(keyTable, indentLevel));

			return statement.ToString();
		}

		protected string GetInsertStatement(int indentLevel)
		{
			StringBuilder insert = new StringBuilder(Indent(indentLevel++, true));
			string columnList = GetColumnList(true, indentLevel);
			insert.Append(string.Format("INSERT INTO {0} ({1}", SourceTable.Name, columnList));
			insert.Append(Indent(--indentLevel, true));
			insert.Append(")");

			StringBuilder statement = new StringBuilder(InsertStatementTemplate);
			statement.Replace("[INSERTCOLUMNS]", insert.ToString());

			StringBuilder values = new StringBuilder(Indent(indentLevel++, true));
			values.Append("VALUES(");
			foreach(ColumnSchema column in SourceTable.Columns) {
				if(!IsColumnAutoSet(column)) {
					values.Append(Indent(indentLevel, true));
					values.Append("@" + column.Name);

					if(IsCommaNecessary(SourceTable.Columns, SourceTable.Columns.IndexOf(column), true)) {
						values.Append(",");
					}
				}
			}
			values.Append(Indent(--indentLevel, true));
			values.Append(")");
			statement.Replace("[VALUES]", values.ToString());

			StringBuilder setNewPk = new StringBuilder();
			foreach(ColumnSchema column in SourceTable.PrimaryKey.MemberColumns) {
				if(IsIdentity(column)) {
					setNewPk.Append(Indent(indentLevel, true));
					setNewPk.Append(string.Format("SET @New{0} = SCOPE_IDENTITY()", column.Name));
					break;
				}
			}
			if(setNewPk.Length > 0) setNewPk.Insert(0, Indent(indentLevel, true));
			statement.Replace("[SETNEWPK]", setNewPk.ToString());

			string setTimeStamp = string.Empty;
			if(SourceTable.Columns.Contains(TimeStampColumn)) {
				setTimeStamp = Indent(indentLevel, true) + GetTableTimeStampSelect(SqlStatementType.Insert, indentLevel);
			}
			statement.Replace("[SETNEWTIMESTAMP]", setTimeStamp);

			return statement.ToString();
		}

		protected string GetUpdateStatement(int indentLevel)
		{
			StringBuilder statement = new StringBuilder(Indent(indentLevel, true));
			statement.Append(UpdateStatementTemplate);

			statement.Replace("[UPDATE]", string.Format("UPDATE{0}{1}", Indent(indentLevel + 1, true), SourceTable.Name));
			
			StringBuilder set = new StringBuilder(Indent(indentLevel++, true));
			set.Append("SET");
			foreach(ColumnSchema column in SourceTable.NonPrimaryKeyColumns) {
				if(!IsColumnAutoSet(column)) {
					set.Append(Indent(indentLevel, true));
					set.Append(string.Format("{0} = @{0}", column.Name));
					if(IsCommaNecessary(SourceTable.NonPrimaryKeyColumns, SourceTable.NonPrimaryKeyColumns.IndexOf(column), true)) {
						set.Append(",");
					}
				}
			}
			statement.Replace("[SETCOLUMNS]", set.ToString());

			statement.Replace("[WHERE]", GetPKWhereClause(--indentLevel));

			string setTimeStamp = string.Empty;
			if(SourceTable.Columns.Contains(TimeStampColumn)) {
				setTimeStamp = Indent(indentLevel, true) + GetTableTimeStampSelect(SqlStatementType.Update, indentLevel);
			}
			statement.Replace("[SETNEWTIMESTAMP]", setTimeStamp);
			
			return statement.ToString();
		}

		protected string GetDeleteStatement(int indentLevel)
		{
			StringBuilder statement = new StringBuilder(Indent(indentLevel, true));
			statement.Append(DeleteStatementTemplate);

			statement.Replace("[DELETE]", "DELETE FROM" + Indent(indentLevel + 1, true) + SourceTable.Name);

			statement.Replace("[WHERE]", GetPKWhereClause(indentLevel));

			return statement.ToString();
		}

		protected string GetColumnList(bool ignoreAutoSetColumns, int indentLevel)
		{
			StringBuilder columnList = new StringBuilder();

			foreach(ColumnSchema column in SourceTable.Columns) {
				if((ignoreAutoSetColumns && !IsColumnAutoSet(column)) || !ignoreAutoSetColumns) {
					columnList.Append(Indent(indentLevel, true));
					columnList.Append(column.Name);
					if(IsCommaNecessary(SourceTable.Columns, SourceTable.Columns.IndexOf(column), ignoreAutoSetColumns)) {
						columnList.Append(",");
					}
				}
			}

			return columnList.ToString();
		}

		protected string GetProcName(SqlStatementType statementType)
		{
			switch(statementType) {
				case SqlStatementType.Select:
					return string.Format(FetchCommandFormat, SourceTable.Name);
				case SqlStatementType.SelectBy:
					if(ParentTable == null) throw new InvalidOperationException("ParentTable must be specified for SelectBy procs.");
					return string.Format(FetchByCommandFormat, SourceTable.Name, ParentTable.Name);
				case SqlStatementType.Insert:
					return string.Format(InsertCommandFormat, SourceTable.Name);
				case SqlStatementType.Update:
					return string.Format(UpdateCommandFormat, SourceTable.Name);
				case SqlStatementType.Delete:
					return string.Format(DeleteCommandFormat, SourceTable.Name);
				default:
					throw new ArgumentOutOfRangeException("statementType", statementType.ToString(), "No command format has been defined for this SqlStatementType");
			}
		}

		protected string GetTableTimeStampSelect(SqlStatementType statementType, int indentLevel)
		{
			StringBuilder select = new StringBuilder(Indent(indentLevel, true));
			select.Append(SelectStatementTemplate);
			select.Replace("[SELECTCOLUMNS]", string.Format("SELECT @New{0} = {0}", TimeStampColumn));
			select.Replace("[FROM]", string.Format("FROM {0}", SourceTable.Name));
			
			string parameterName;

			StringBuilder where = new StringBuilder("WHERE ");
			foreach(ColumnSchema column in SourceTable.PrimaryKey.MemberColumns) {
				if(SourceTable.PrimaryKey.MemberColumns.IndexOf(column) > 0) {
					where.Append(" AND ");
				}

				if(statementType == SqlStatementType.Insert && IsIdentity(column)) {
					parameterName = string.Format("@New{0}", column.Name);
				}
				else {
					parameterName = string.Format("@{0}", column.Name);
				}
				where.Append(string.Format("{0} = {1}", column.Name, parameterName));
			}

			select.Replace("[WHERE]", where.ToString());

			return select.ToString();
		}

		#endregion

		#region Private Methods

		private string GetPKWhereClause(int indentLevel)
		{
			return GetPKWhereClause(SourceTable, indentLevel);
		}
		
		private static string GetPKWhereClause(TableSchema table, int indentLevel)
		{
			CheckForPrimaryKey(table);

			StringBuilder where = new StringBuilder(Indent(indentLevel++, true));
			where.Append("WHERE");

			foreach(ColumnSchema column in table.PrimaryKey.MemberColumns) {
				where.Append(Indent(indentLevel, true));
				if(table.PrimaryKey.MemberColumns.IndexOf(column) > 0) where.Append("AND ");
				where.Append(string.Format("{0} = @{0}", column.Name));
			}

			return where.ToString();
		}

		private static void CheckForPrimaryKey(TableSchema table)
		{
			if(!table.HasPrimaryKey) {
				throw new InvalidOperationException(string.Format("The {0} table does not have a primary key defined.", table.Name));
			}
		}


		#endregion

		#region Indent
		public static string Indent()
		{
			return Indent(1);
		}
		public static string Indent(int level)
		{
			return Indent(level, false);
		}

		public static string Indent(int level, bool newLine)
		{
			string str = string.Empty;

			if(newLine) str = "\r\n";
			if(IndentLevelSpaces > 0)
				str += new string(' ', level * IndentLevelSpaces);
			else
				str += new string('\t', level);

			return str;
		}
		#endregion //Indent

	}
}
