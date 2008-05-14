using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Utilities;

public class ProjectUtility
{	
	private Project _project;
	private string _projectDirectory;

	public ProjectUtility(string projectFile, string internalPath)
	{
		_projectFile = projectFile;
		_internalPath = internalPath;
		InitializeProject();
	}
	
	private string _projectFile;
	
	public string ProjectFile
	{
		get {return _projectFile;}
		set {_projectFile = value;}
	}

	private string _internalPath;
	
	public string InternalPath
	{
		get {return _internalPath;}
		set {_internalPath = value;}
	}
	
	public void AddEmbeddedResource(string fileName)
	{
		AddItem(fileName, "EmbeddedResource", true);	
	}
	
	public void AddCompileItem(string fileName)
	{
		AddItem(fileName, "Compile", true);	
	}
	
	public void AddReferenceItem(string reference, bool searchProjectReferences)
	{
		if (searchProjectReferences)
		{
			BuildItemGroup group = _project.GetEvaluatedItemsByName("ProjectReference");
			foreach (BuildItem item in group)
			{
				if (item.Include.Contains(reference) || 
					(item.HasMetadata("Name") && item.GetMetadata("Name").Contains(reference)))
				{
					return;
				}
			}
		}
		AddItem(reference, "Reference", false);	
	}
	
	public void AddItem(string fileName, string itemType, bool makeRelative)
	{
		if (string.IsNullOrEmpty(this.ProjectFile) || _project == null)
			return;

		string pn = fileName;
		if (makeRelative)
			pn = CommonUtility.RelativePathTo(_projectDirectory, Path.GetFullPath(pn));
		
		BuildItem pItem = null;
		
		BuildItemGroup group = _project.GetEvaluatedItemsByName(itemType);
		
		foreach (BuildItem item in group)
		{
			if (string.Compare(item.FinalItemSpec, pn, true) == 0)
			{
				pItem = item;
				break;
			}
		}
		
		if (pItem == null)
			pItem = _project.AddNewItem(itemType, pn);
	}

	public void AddDependentClassItem(string classFile, string dependentFile)
	{
		if (string.IsNullOrEmpty(this.ProjectFile) || _project == null)
			return;
		
		string pn = CommonUtility.RelativePathTo(_projectDirectory, Path.GetFullPath(classFile));
		string gn = CommonUtility.RelativePathTo(_projectDirectory, Path.GetFullPath(dependentFile));
		
		BuildItem pItem = null;
		BuildItem gItem = null;
		
		BuildItemGroup group = _project.GetEvaluatedItemsByName("Compile");
		
		foreach (BuildItem item in group)
		{
			if (string.Compare(item.FinalItemSpec, pn, true) == 0)
				pItem = item;
			else if (string.Compare(item.FinalItemSpec, gn, true) == 0)
				gItem = item;
		}
		
		if (pItem == null)
			pItem = _project.AddNewItem("Compile", pn);
		if (gItem == null)
			gItem = _project.AddNewItem("Compile", gn);
		
		gItem.SetMetadata("DependentUpon", Path.GetFileName(pItem.FinalItemSpec));
	}
	
	public void InitializeProject()
	{
		if (string.IsNullOrEmpty(this.ProjectFile))
			return;
			
		_projectDirectory = Path.GetDirectoryName(Path.GetFullPath(_projectFile));
		if (!Directory.Exists(_projectDirectory))
			Directory.CreateDirectory(_projectDirectory);
			
		Engine.GlobalEngine.BinPath = ToolLocationHelper.GetPathToDotNetFramework(TargetDotNetFrameworkVersion.Version20);
		_project = new Project();
		if (!File.Exists(_projectFile))
		{
			// create project file
			Guid g = Guid.NewGuid();
			string projectName = Path.GetFileNameWithoutExtension(_projectFile);
			
			StringBuilder buffer = new StringBuilder();
			buffer.Append(File.ReadAllText(Path.Combine(_internalPath, @"Project\classlibrary.csproj")));
			buffer.Replace("$safeprojectname$", projectName);
			buffer.Replace("$guid1$", g.ToString());
			File.WriteAllText(_projectFile, buffer.ToString());
			
			string propertiesDirectory = Path.Combine(_projectDirectory, "Properties");
			if (!Directory.Exists(propertiesDirectory))
				Directory.CreateDirectory(propertiesDirectory);
			
			string infoFile = Path.Combine(propertiesDirectory, "AssemblyInfo.cs");
			if (!File.Exists(infoFile))
			{
				buffer.Length = 0;
				buffer.Append(File.ReadAllText(Path.Combine(_internalPath, @"Project\assemblyinfo.cs")));
				buffer.Replace("$projectname$", projectName);
				buffer.Replace("$guid1$", g.ToString());
				File.WriteAllText(infoFile, buffer.ToString());
			}		
		}
		// loading project file
		_project.Load(_projectFile);
		// add default references
	}
	
	public void SaveProject()
	{
		if (string.IsNullOrEmpty(this.ProjectFile) || _project == null)
			return;
			
		_project.Save(this.ProjectFile);
	}

}