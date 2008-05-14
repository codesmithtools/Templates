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

public class _Main : CodeSmith.BaseTemplates.SqlCodeTemplate
{
	
	private string _outputDirectory = String.Empty;
	
	public _Main() : base()
	{
	}
	  
    #region Output Directory
	[Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))] 
	[Optional]
	[Category("2. Output")]
	[Description("The directory to output the results to.")]
	public string OutputDirectory 
	{
		get
		{
			// default to the directory that the template is located in
			if (_outputDirectory.Length == 0) return this.CodeTemplateInfo.DirectoryName + "output\\";
			
			return _outputDirectory;
		}
		set
		{
			if (!value.EndsWith("\\")) value += "\\";
			_outputDirectory = value;
		} 
	}
    #endregion

    #region ExecuteScriptsOnTarget
    private bool _executeScriptsOnTarget = false;
    [Category("2. Options")]
    [Description("If ExecuteScriptsOnTarget is true, the script will be executed on the database specified in TargetDatabase")]
    public bool ExecuteScriptsOnTarget
    {
        get { return _executeScriptsOnTarget; }
        set { _executeScriptsOnTarget = value; }
    }
    #endregion

    #region TargetDataBase
    private SchemaExplorer.DatabaseSchema _targetDatabase;
    [OptionalAttribute]
    [Category("2. Options")]
    [Description("If ExecuteScriptsOnTarget is true, the script will be executed on this database")]
    public SchemaExplorer.DatabaseSchema TargetDatabase
    {
        get { return _targetDatabase; }
        set { _targetDatabase = value; }
    }
    #endregion
        
	public void OutputTemplate(CodeTemplate template)
	{
		this.CopyPropertiesTo(template);
		template.Render(this.Response);
	}

    protected override void OnPostRender(string result)
    {
        if (this.ExecuteScriptsOnTarget)
        {
            // execute the output on the same database as the source table.
            CodeSmith.BaseTemplates.ScriptResult scriptResult = CodeSmith.BaseTemplates.ScriptUtility.ExecuteScript(this.TargetDatabase.Database.ConnectionString, result, new System.Data.SqlClient.SqlInfoMessageEventHandler(cn_InfoMessage));
            Trace.Write(scriptResult.ToString());
        }

        base.OnPostRender(result);
    }

    private void cn_InfoMessage(object sender, System.Data.SqlClient.SqlInfoMessageEventArgs e)
    {
        Trace.WriteLine(e.Message);
    }

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
}