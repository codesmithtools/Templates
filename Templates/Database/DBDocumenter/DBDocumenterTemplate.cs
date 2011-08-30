//-------------------------------------------------------------
// CodeSmith DBDocumenter Templates v3.0
// Author:  Jason Alexander (jalexander@telligent.com), Eric J. Smith
//-------------------------------------------------------------

using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.ComponentModel.Design;
using CodeSmith.Engine;
using SchemaExplorer;

public class DBDocumenterTemplate : CodeTemplate
{
	// Number of columns that should be displayed on the summary lists.
	public const int NUM_OF_COLUMNS = 3;
	
	private string _outputDirectory = String.Empty;
	
	public DBDocumenterTemplate() : base()
	{
	}
	  
	[Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))] 
	[Optional]
	[Category("Output")]
	[Description("The directory to output the results to.")]
	public string OutputDirectory 
	{
		get
		{
			// default to the directory that the template is located in
			if (_outputDirectory.Length == 0)
        return Path.Combine(CodeTemplateInfo.DirectoryName, "output\\");
			
			return _outputDirectory;
		}
		set
		{
			if (!value.EndsWith("\\"))
        value += "\\";
			_outputDirectory = value;
		} 
	}
	
	public void OutputTemplate(CodeTemplate template)
	{
		this.CopyPropertiesTo(template);
		template.Render(this.Response);
	}
	
	public SqlDataReader GetSystemInformation(string connectionString)
	{
		SqlConnection cn = new SqlConnection(connectionString);
		SqlCommand cmd = new SqlCommand();
		
		cmd.Connection = cn;
		cmd.CommandText = "master.dbo.xp_msver";
		cmd.CommandType = CommandType.StoredProcedure;
		
		cn.Open();
		SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
		
		return reader;
	}
	
	public void OutputExceptionInformation(Exception exception, int indentLevel)
	{
		int originalIndentLevel = Response.IndentLevel;
		Response.IndentLevel = indentLevel;
		Response.WriteLine("<table width=\"95%\">");
		Response.WriteLine("<tr>");
		Response.WriteLine("	<td>");
		Response.WriteLine("		<span class=\"exceptionText\">");
		Response.WriteLine("		An exception occurred while attempting to execute the template:" );
		Response.WriteLine("		" + exception.Message);
		Response.WriteLine("		</span>");
		Response.WriteLine("	</td>");
		Response.WriteLine("</tr>");
		Response.WriteLine("</table>");
		Response.IndentLevel = originalIndentLevel;
	}
	
	public void OutputSystemInformation(string connectionString, int indentLevel)
	{
		SqlDataReader info = null;
		
		try
		{
			info = this.GetSystemInformation(connectionString);
			
			int originalIndentLevel = Response.IndentLevel;
			Response.IndentLevel = indentLevel;
			Response.WriteLine("<table width=\"95%\">");
			
			while (info.Read())
			{
				Response.WriteLine("<tr>");
				Response.WriteLine("	<td width=\"40\">&nbsp;</td>");
				Response.WriteLine("	<td width=\"100\">");
				Response.WriteLine("		<b><span class=\"bodyText\">" + info["Name"] + ":</span></b>");
				Response.WriteLine("	</td>");
				Response.WriteLine("	<td width=\"100%\" align=\"left\">");
				Response.WriteLine("		<span class=\"bodyText\">" + info["Character_Value"].ToString().Trim() + "</span>");
				Response.WriteLine("    </td>");
				Response.WriteLine("</tr>");
			}
			
			Response.WriteLine("</table>");
			Response.IndentLevel = originalIndentLevel;
		}
		catch (Exception ex)
		{
			this.OutputExceptionInformation(ex, indentLevel);
		}
		finally
		{
			if (info != null) info.Close();
		}
	}
	
	public string GetParameterSize(ParameterSchema parameter)
	{
		string parameterSize = parameter.Size.ToString();
		
		if (parameter.NativeType == "numeric" && parameter.Precision != 0)
		{
			parameterSize += "(" + parameter.Precision.ToString() + "," + parameter.Scale + ")";
		}
		
		return parameterSize;
	}
	
	public string GetColumnSize(ColumnSchema column)
	{
		string columnSize = column.Size.ToString();
		
		if (column.NativeType == "numeric" && column.Precision != 0)
		{
			columnSize += "(" + column.Precision.ToString() + "," + column.Scale + ")";
		}
		
		return columnSize;
	}
	
	public string GetColumnSize(ViewColumnSchema column)
	{
		string columnSize = column.Size.ToString();
		
		if (column.NativeType == "numeric" && column.Precision != 0)
		{
			columnSize += "(" + column.Precision.ToString() + "," + column.Scale + ")";
		}
		
		return columnSize;
	}
	
	public void DeleteFiles(string directory, string searchPattern)
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