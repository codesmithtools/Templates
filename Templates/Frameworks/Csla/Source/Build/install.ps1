param($installPath, $toolsPath, $package, $project)

Write-Host "Running install.ps1"

$ReferencedAssemblies = ( 
    "System.Xml"
) 
$UpdaterSource = @" 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;

public static class GeneratorHelper {
    public static readonly HashSet<string> _updatableTemplateOutputs = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "CommandObject.cst",
            "Criteria.cst",
            "DynamicListBase.cst",
            "DynamicRoot.cst",
            "DynamicRootList.cst",
            "EditableChild.cst",
            "EditableChildList.cst",
            "EditableRoot.cst",
            "EditableRootList.cst",
            "Entities.cst",
            "NameValueList.cst",
            "ReadOnlyChild.cst",
            "ReadOnlyChildList.cst",
            "ReadOnlyList.cst",
            "ReadOnlyRoot.cst",
            "SwitchableObject.cst",
        };

    public static readonly HashSet<string> _updatableInternalTemplateOutputs = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "ADOHelper.cst",
            "AsyncChildLoader.cst",
            "ExistsCommand.cst",
            "FactoryNames.cst",
            "IGeneratedCriteria.cst"
        };

    public static void ReplaceVariables(string filePath, string projectName, string solutionName, string version) {
        if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return;

        var content = File.ReadAllText(filePath);

		if(!String.Equals(version, "v35") && !String.Equals(version, "v40"))
			version = "v40";

        content = content.Replace("%projectname%", projectName)
        			.Replace("%solutionname%", solutionName)
        			.Replace("%version%", version);
        
        File.WriteAllText(filePath, content);
    }

    public static void UpdateProject(string filePath, string toolPath, string projectName, string solutionName, string version) {
        if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return;

        string projectDirectory = Path.GetDirectoryName(filePath);
        if (String.IsNullOrEmpty(projectDirectory))
            return;

        string language = null;
        string extension = Path.GetExtension(filePath);
        if (String.Equals(extension, ".csproj", StringComparison.OrdinalIgnoreCase))
            language = "CSharp\\";
        else if (String.Equals(extension, ".vbproj", StringComparison.OrdinalIgnoreCase))
            language = "VisualBasic\\";

        string[] files = Directory.GetFiles(projectDirectory, "*.csp", SearchOption.AllDirectories);
        foreach (string file in files) {
            if (string.Equals(Path.GetExtension(file), ".csp", StringComparison.OrdinalIgnoreCase)) {
                ReplaceVariables(file, projectName, solutionName, version);
                UpdatePath(file, toolPath, language);
            }
        }

    }

    public static void UpdatePath(string projectPath, string toolPath, string language) {
        int updated = 0;

        var document = new XmlDocument();
        document.Load(projectPath);

        XPathNavigator navigator = document.CreateNavigator();
        var manager = new XmlNamespaceManager(navigator.NameTable);
        manager.AddNamespace("d", "http://www.codesmithtools.com/schema/csp.xsd");

        XPathExpression expression = XPathExpression.Compile("/d:codeSmith/d:propertySets/d:propertySet/@template", manager);
        foreach (XPathNavigator pathNavigator in navigator.Select(expression)) {
            string template = pathNavigator.Value;
            string templateName = Path.GetFileName(template);

            if(String.IsNullOrEmpty(templateName))
                continue;

            bool isValidTemplate = _updatableTemplateOutputs.Contains(templateName);
            bool isValidInternalTemplate = _updatableInternalTemplateOutputs.Contains(templateName);
            if (!isValidTemplate && !isValidInternalTemplate)
                continue;

            string templatePath = !String.IsNullOrEmpty(language) ? Path.Combine(toolPath, language, "BusinessLayer\\") : Path.Combine(toolPath, "BusinessLayer\\");
            if(isValidInternalTemplate)
                templatePath = Path.Combine(templatePath, "Internal\\");

            templatePath = RelativePathTo(Path.GetDirectoryName(projectPath), Path.Combine(templatePath, templateName));
            pathNavigator.SetValue(templatePath);

            updated++;
        }

        if (updated > 0)
            document.Save(projectPath);
    }

    public static string RelativePathTo(string fromDirectory, string toPath) {
        if (fromDirectory == null)
            throw new ArgumentNullException("fromDirectory");

        if (toPath == null)
            throw new ArgumentNullException("toPath");

        bool isRooted = Path.IsPathRooted(fromDirectory)
            && Path.IsPathRooted(toPath);

        if (isRooted) {
            bool sameRoot = String.Equals(
                Path.GetPathRoot(fromDirectory),
                Path.GetPathRoot(toPath),
                StringComparison.OrdinalIgnoreCase);

            if (!sameRoot)
                return toPath;
        }

        var relativePath = new List<string>();
        string[] fromDirectories = fromDirectory.Split(
            Path.DirectorySeparatorChar);

        string[] toDirectories = toPath.Split(
            Path.DirectorySeparatorChar);

        int length = Math.Min(
            fromDirectories.Length,
            toDirectories.Length);

        int lastCommonRoot = -1;

        // find common root
        for (int x = 0; x < length; x++) {
            bool sameDirectory = String.Equals(fromDirectories[x],
                toDirectories[x],
                StringComparison.OrdinalIgnoreCase);
            if (!sameDirectory)
                break;

            lastCommonRoot = x;
        }

        if (lastCommonRoot == -1)
            return toPath;

        // add relative folders in from path
        for (int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
            if (fromDirectories[x].Length > 0)
                relativePath.Add("..");

        // add to folders to path
        for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
            relativePath.Add(toDirectories[x]);

        // create relative path
        var relativeParts = new string[relativePath.Count];
        relativePath.CopyTo(relativeParts, 0);

        string newPath = String.Join(
            Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture),
            relativeParts);

        return newPath;
    }
}
"@ 

Add-Type -ReferencedAssemblies $ReferencedAssemblies -TypeDefinition $UpdaterSource -Language CSharp

Write-Host "Searching $($project.Name) for CodeSmith project files..."
$solutionName = [System.IO.Path]::GetFileNameWithoutExtension($DTE.Solution.FileName)
$framework = New-Object -TypeName System.Runtime.Versioning.FrameworkName $project.Properties.Item("TargetFrameworkMoniker").Value.ToString()
$version = [string]::Format("v{0}{1}", $framework.Version.Major, $framework.Version.Minor)
[GeneratorHelper]::UpdateProject($project.FullName, $toolsPath, $project.Name, $solutionName, $version)