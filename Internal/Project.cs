using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Utilities;
using CodeSmith.Engine.Utility;

public class ProjectUpdater
{

    public ProjectUpdater(string projectFile)
    {
        _projectFile = projectFile;
        InitializeProject();
    }

    private Project _project;

    public Project Project
    {
        get { return _project; }
    }

    private string _projectFile;

    public string ProjectFile
    {
        get { return _projectFile; }
    }

    private string _projectDirectory;

    public string ProjectDirectory
    {
        get { return _projectDirectory; }
    }

    public bool IsAvailable
    {
        get { return (!string.IsNullOrEmpty(this.ProjectFile) && _project != null); }
    }

    public void AddEmbeddedResource(string fileName)
    {
        AddItem(fileName, "EmbeddedResource", true);
    }

    public void AddCompileItem(string fileName)
    {
        AddItem(fileName, "Compile", true);
    }

    public void AddNoneItem(string fileName)
    {
        AddItem(fileName, "None", true);
    }

    public void AddCompileItem(string fileName, string dependentFile)
    {
        if (!IsAvailable)
            return;

        fileName = PathUtil.RelativePathTo(
            _projectDirectory, Path.GetFullPath(fileName));
        dependentFile = PathUtil.RelativePathTo(
            _projectDirectory, Path.GetFullPath(dependentFile));

        BuildItem classItem = null;
        BuildItem dependItem = null;

        BuildItemGroup group = _project.GetEvaluatedItemsByName("Compile");

        foreach (BuildItem item in group)
        {
            if (item.FinalItemSpec.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                classItem = item;
            else if (item.FinalItemSpec.Equals(dependentFile, StringComparison.OrdinalIgnoreCase))
                dependItem = item;
        }

        if (classItem == null)
            classItem = _project.AddNewItem("Compile", fileName);
        if (dependItem == null)
            dependItem = _project.AddNewItem("Compile", dependentFile);

        if (!dependItem.HasMetadata("DependentUpon"))
        {
            dependItem.SetMetadata(
                "DependentUpon",
                Path.GetFileName(classItem.FinalItemSpec));
            _project.MarkProjectAsDirty();
        }

    }

    public void AddReferenceItem(string reference, bool searchProjectReferences)
    {
        if (!IsAvailable)
            return;

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
        if (!IsAvailable)
            return;

        if (makeRelative)
        {
            fileName = PathUtil.RelativePathTo(
                _projectDirectory, Path.GetFullPath(fileName));
        }

        BuildItem newItem = null;
        BuildItemGroup group = _project.GetEvaluatedItemsByName(itemType);

        foreach (BuildItem item in group)
        {
            if (fileName.Equals(item.FinalItemSpec,
                StringComparison.OrdinalIgnoreCase))
            {
                newItem = item;
                break;
            }
        }

        if (newItem == null)
        {
            newItem = _project.AddNewItem(itemType, fileName);
            _project.MarkProjectAsDirty();
        }
    }

    public void AddConnectionStringSetting(string name, string connectionString)
    {
        if (!IsAvailable)
            return;

        AddSettingsFile();
        
        string fileName = Path.Combine(ProjectDirectory, SettingsFileName);
        string nameXPath = string.Format("/s:SettingsFile/s:Settings/s:Setting[@Name='{0}']", name);

        XmlDocument document = new XmlDocument();
        document.Load(fileName);

        XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);
        manager.AddNamespace("s", "http://schemas.microsoft.com/VisualStudio/2004/01/settings");

        XmlNode settingsNode = document.SelectSingleNode(nameXPath, manager);
        if (settingsNode != null)
            return;

        const string settingsXPath = "/s:SettingsFile/s:Settings";
        settingsNode = document.SelectSingleNode(settingsXPath, manager);
        if (settingsNode == null)
        {
            settingsNode = document.CreateElement("Settings", document.DocumentElement.NamespaceURI);
            document.DocumentElement.AppendChild(settingsNode);
        }

        XmlElement settingNode = document.CreateElement("Setting", document.DocumentElement.NamespaceURI);
        settingNode.SetAttribute("Name", name);
        settingNode.SetAttribute("Type", "(Connection string)");
        settingNode.SetAttribute("Scope", "Application");

        XmlElement designTimeNode = document.CreateElement("DesignTimeValue", document.DocumentElement.NamespaceURI);
        designTimeNode.SetAttribute("Profile", "(Default)");
        designTimeNode.InnerText = string.Format(DesignTimeFragment, connectionString);
        settingNode.AppendChild(designTimeNode);

        XmlElement valueNode = document.CreateElement("Value", document.DocumentElement.NamespaceURI);
        valueNode.SetAttribute("Profile", "(Default)");
        valueNode.InnerText = connectionString;
        settingNode.AppendChild(valueNode);

        settingsNode.AppendChild(settingNode);

        XmlWriterSettings xws = new XmlWriterSettings();
        xws.Indent = true;
        xws.IndentChars = "  ";
        
        using (XmlWriter xw = XmlWriter.Create(fileName, xws))
        {
            document.WriteTo(xw);
        }
    }

    public void AddSettingsFile()
    {
        if (!IsAvailable)
            return;

        BuildItemGroup group = _project.GetEvaluatedItemsByName("None");
        BuildItem item = null;

        foreach (BuildItem b in group)
        {
            if (SettingsFileName.Equals(b.FinalItemSpec, StringComparison.OrdinalIgnoreCase))
            {
                item = b;
                break;
            }
        }

        if (item == null)
        {
            item = _project.AddNewItem("None", SettingsFileName);
            item.SetMetadata("Generator", "SettingsSingleFileGenerator");
            _project.MarkProjectAsDirty();
        }

        string fileName = Path.Combine(ProjectDirectory, SettingsFileName);

        if (!File.Exists(fileName))
            File.WriteAllText(fileName, SettingFileTempalte, Encoding.UTF8);

    }

    public string FindFile(string fileName, string itemType)
    {
        if (!IsAvailable)
            return null;

        BuildItem classItem = null;
        BuildItemGroup group = _project.GetEvaluatedItemsByName(itemType);

        foreach (BuildItem item in group)
        {
            if (item.FinalItemSpec.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
            {
                classItem = item;
                break;
            }
        }

        if (classItem == null)
            return null;

        return classItem.FinalItemSpec;
    }

    public string GetRootNamespace()
    {
        if (!IsAvailable)
            return string.Empty;

        string rootNamespace = _project.GetEvaluatedProperty("RootNamespace");
        return rootNamespace;
    }

    private void InitializeProject()
    {
        if (string.IsNullOrEmpty(this.ProjectFile)
            || !File.Exists(this.ProjectFile))
            return;

        _projectDirectory = Path.GetDirectoryName(
            Path.GetFullPath(_projectFile));
        string binPath = ToolLocationHelper.GetPathToDotNetFramework(TargetDotNetFrameworkVersion.VersionLatest);
        Engine engine = new Engine(binPath);
        _project = new Project(engine);
        _project.Load(_projectFile);
    }

    public void SaveProject()
    {
        if (string.IsNullOrEmpty(this.ProjectFile)
            || _project == null
            || !_project.IsDirty)
            return;

        _project.Save(this.ProjectFile);
    }

    private const string SettingsFileName = @"Properties\Settings.settings";

    private const string SettingFileTempalte = @"<?xml version='1.0' encoding='utf-8'?>
<SettingsFile xmlns=""http://schemas.microsoft.com/VisualStudio/2004/01/settings"" CurrentProfile=""(Default)"">
  <Profiles>
    <Profile Name=""(Default)"" />
  </Profiles>
</SettingsFile>";

    private const string DesignTimeFragment = @"<?xml version=""1.0"" encoding=""utf-16""?>
<SerializableConnectionString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <ConnectionString>{0}</ConnectionString>
  <ProviderName>System.Data.SqlClient</ProviderName>
</SerializableConnectionString>";

}