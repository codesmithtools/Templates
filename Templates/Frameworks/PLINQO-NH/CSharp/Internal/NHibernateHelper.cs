using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;
using CodeSmith.SchemaHelper.NHibernate;

public class NHibernateHelper : CodeTemplate
{
    #region Master Helpers
    
	public OutputFile GetOutputFile(string fileName, string dependentUpon, params object[] metaData)
    {
        OutputFile outputFile = new OutputFile(fileName);
        
        if(!String.IsNullOrEmpty(dependentUpon))
            outputFile.DependentUpon = Path.GetFullPath(dependentUpon);
        
        if(metaData.Length % 2 != 0)
            throw new Exception("Invalid Metadata: Provide 2 objects per entry, a String (key) followed by an Object.");
            
        for(int x=0; x<metaData.Length; x+=2)
            outputFile.Metadata.Add(metaData[x].ToString(), metaData[x+1]);
            
        return outputFile;
    }
    
    public string GetFolder(string folder)
    {
        string current = Directory.GetCurrentDirectory();
        string path = Path.GetFullPath(current);
        
        if (String.IsNullOrEmpty(folder))
            return path;
            
        string newPath = Path.Combine(path, folder);
        if(!Directory.Exists(newPath))
            Directory.CreateDirectory(newPath);
            
        return newPath;
    }
    
    public string GetFile(string localPath)
    {
        var path = Path.Combine(this.CodeTemplateInfo.DirectoryName, localPath);
        return Path.GetFullPath(path);
    }
    
    #endregion
    
    #region Sub Helpers
    
    public IEnumerable<string> GetEntityNamespaces(EntityManager manager)
    {
        return manager.Entities
            .Select(e => e.Namespace)
            .Distinct();
    }
    
    public static readonly string GeneratedCodeAttribute =  string.Format(
        "[System.CodeDom.Compiler.GeneratedCode(\"CodeSmith\", \"{0}\")]",
        typeof(CodeTemplate).Assembly.GetName().Version.ToString());
    
    #endregion
}
