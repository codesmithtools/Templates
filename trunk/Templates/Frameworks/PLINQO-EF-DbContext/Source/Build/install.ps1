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

public static class Updater
{
    public static void UpdateSearch(string projectPath, string toolPath)
    {
        string projectDirectory = Path.GetDirectoryName(projectPath);
        string[] files = Directory.GetFiles(projectDirectory, "*.csp", SearchOption.AllDirectories);
        
        foreach (string file in files)
        {
            if (string.Equals(Path.GetExtension(file), ".csp", StringComparison.OrdinalIgnoreCase))
                UpdatePath(file, toolPath);
        }
    }

    public static void UpdatePath(string projectPath, string toolPath)
    {
        string projectDirectory = Path.GetDirectoryName(projectPath);

        int updated = 0;
        string newPath = Path.Combine(toolPath, "CSharp");
        newPath = Path.Combine(newPath, "Entity.cst");
        newPath = RelativePathTo(projectDirectory, newPath);

        
        XmlDocument document = new XmlDocument();
        document.Load(projectPath);

        XPathNavigator navigator = document.CreateNavigator();

        XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
        manager.AddNamespace("d", "http://www.codesmithtools.com/schema/csp.xsd");

        XPathExpression expression = XPathExpression.Compile("/d:codeSmith/d:propertySets/d:propertySet/@template", manager);
        foreach (XPathNavigator pathNavigator in navigator.Select(expression))
        {
            string template = pathNavigator.Value;
            string templateName = Path.GetFileName(template);

            if (!String.Equals(templateName, "Entity.cst", StringComparison.OrdinalIgnoreCase))
                continue;

            pathNavigator.SetValue(newPath);
            updated++;
        }

        if (updated > 0)
            document.Save(projectPath);
    }

    public static string RelativePathTo(string fromDirectory, string toPath)
    {
        if (fromDirectory == null)
            throw new ArgumentNullException("fromDirectory");

        if (toPath == null)
            throw new ArgumentNullException("toPath");

        bool isRooted = Path.IsPathRooted(fromDirectory)
                        && Path.IsPathRooted(toPath);

        if (isRooted)
        {
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
        for (int x = 0; x < length; x++)
        {
            bool sameDirectory = String.Equals(fromDirectories[x], toDirectories[x], StringComparison.OrdinalIgnoreCase);
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

ForEach ($p in Get-Project -all) {
    Write-Host "Searching $($p.Name) for CodeSmith project files..."
    [Updater]::UpdateSearch($p.FullName, $toolsPath)
}