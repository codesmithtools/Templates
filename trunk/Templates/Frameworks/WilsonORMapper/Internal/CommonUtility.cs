using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

public enum LangaugeEnum
{
	CSharp,
	VB
}

public enum FrameworkEnum
{
	v1,
	v2
}

public enum TestFrameworkEnum
{
	NUnit,
	TeamTest
}

public struct ManyToManyInfo
{
    public string Field;
    public string Type;
    public string Member;
    public string SourceField;
    public string DestField;        
}

public sealed class CommonUtility
{
	private static readonly Regex _cleanNameRegex = new Regex(@"[\W_]+", RegexOptions.Compiled);
	private static readonly Regex _cleanIdRegex = new Regex(@"(_ID|_id|_Id|\.ID|\.id|\.Id|ID|Id)$", RegexOptions.Compiled);

	private static readonly Regex _endNumberRegex = new Regex(@"\d+$", RegexOptions.Compiled);

	private static string[] _csharpKeywords;
	public const string MemberPrefix = "_";

	static CommonUtility()
	{
		_csharpKeywords = new string[] {
                "__arglist","__makeref","__reftype","__refvalue",
                "abstract","as",
                "base","bool","break","byte",
                "case","catch","char","checked","class","const","continue",
                "decimal","default","delegate","do","double",
                "else","enum","event","explicit","extern",
                "false","finally","fixed","float","for","foreach",
                "goto",
                "if","implicit","in","int","interface","internal","is",
                "lock","long",
                "namespace","new","null",
                "object","operator","out","override",
                "params","partial","private","protected","public",
                "readonly","ref","return",
                "sbyte","sealed","short","sizeof","stackalloc","static","string","struct","switch",
                "this","throw","true","try","typeof",
                "uint","ulong","unchecked","unsafe","ushort","using",
                "virtual","void","volatile",
                "where","while",
                "yield"};
	}
	
	public static bool IsCSharpKeyword(string value)
	{
		if(value.Length > 10)
			return false;
		
		int index = Array.BinarySearch(_csharpKeywords, value);
        return (index >= 0);
	}
 
	public static string CreateEscapedIdentifier(string name)
	{
		if (!IsCSharpKeyword(name))
		{
				return name;
		}
		return ("@" + name);
	}

	
	public static string RemoveID(string text)
	{
		return _cleanIdRegex.Replace(text, "");	
	}

	public static string GetUniqueName(StringDictionary list, string name)
	{
		// duplicate check
		int counter = 2;
		while(list.ContainsValue(name))
		{
			name = _endNumberRegex.Replace(name, "");
			name += counter.ToString();
			counter++;
		}
		return name;
	}
	
	public static bool IsNullableType(string nativeType)
	{
		if (nativeType.StartsWith("System."))
		{
			Type myType = Type.GetType(nativeType, false);
			if (myType != null)
			{
				return myType.IsValueType;
			}			
		}
		return false;		
	}
	
	public static string GetNullValue(string type)
	{
		switch (type)
		{
			case "System.Int16":
			case "System.Int32":
			case "System.Int64":
			case "System.Single":
			case "System.Double":
			case "System.Decimal":
				return "-1";
			case "System.Byte":
				return "0";
			case "System.Boolean":
				return "false";
			case "System.DateTime":
				return DateTime.MinValue.ToString();
			case "System.Guid":
				return Guid.Empty.ToString();
			default :
				return "";
		}
	}
	
	public static string GetNullDefault(string type, string value)
	{
		switch (type)
		{
			case "System.String": 
				return "\"" + value + "\"";
			case "System.Char": 
				return "'" + value + "'";
			case "System.DateTime":
				if (value == DateTime.MinValue.ToString())
					return "DateTime.MinValue";
				else
					return string.Format("DateTime.Parse(\"{0}\")", value);
			case "System.Guid":
				if (value == Guid.Empty.ToString())
					return "Guid.Empty";
				else
					return string.Format("new Guid(\"{0}\")", value);					
			case "System.Byte[]":
				return "new byte[0]";				
			default :
				return value;
		}
	}
	
	public static string GetAliasVariableType(string type)
	{
		switch (type)
		{
			case "System.String": return "string";
			case "System.Byte": return "byte";
			case "System.Byte[]": return "byte[]";			
			case "System.Int16": return "short";
			case "System.Int32": return "int";
			case "System.Int64": return "long";
			case "System.Char": return "char";
			case "System.Single": return "float";
			case "System.Double": return "double";
			case "System.Boolean": return "bool";
			case "System.Decimal": return "decimal";
			case "System.SByte": return "sbyte";
			case "System.UInt16": return "ushort";
			case "System.UInt32": return "uint";
			case "System.UInt64": return "ulong";
			case "System.Object": return "object";
			default:
				// remove System.
				string nameSpace = GetNamespace(type);				
				if (nameSpace == "System")
					type = GetClassName(type);
					
				return type;
		}
	}
	
	public static string GetClassName(string name)
    {
        string[] namespaces = name.Split(new Char[] {'.'});
        return namespaces[namespaces.Length-1];
    }
	
	public static string GetNamespace(string name)
    {
        string[] namespaces = name.Split(new Char[] {'.'});
        return String.Join(".", namespaces, 0, namespaces.Length-1);		
    }
	
	public static string GetFileText(string fileName)
	{
		string buffer = "";
		using (StreamReader streamReader = File.OpenText(fileName))
		{
			//read the entire file into a string
			buffer = streamReader.ReadToEnd();
		}	
		return buffer;
	}
	public static string RelativePathTo(string fromDirectory, string toPath)
	{
		if (fromDirectory == null)
			throw new ArgumentNullException("fromDirectory");
		
		if (toPath == null)
			throw new ArgumentNullException("toPath");
		
		if (System.IO.Path.IsPathRooted(fromDirectory) && System.IO.Path.IsPathRooted(toPath))
		{
			if (string.Compare(System.IO.Path.GetPathRoot(fromDirectory),
				System.IO.Path.GetPathRoot(toPath), true) != 0)
			{
				throw new ArgumentException(
					string.Format("The paths '{0} and '{1}' have different path roots.",
						fromDirectory, toPath));
			}
		}
		
		StringCollection relativePath = new StringCollection();
		string[] fromDirectories = fromDirectory.Split(System.IO.Path.DirectorySeparatorChar);
		string[] toDirectories = toPath.Split(System.IO.Path.DirectorySeparatorChar);
		int length = Math.Min(fromDirectories.Length, toDirectories.Length);
		int lastCommonRoot = -1;
		
		// find common root
		for (int x = 0; x < length; x++)
		{
			if (string.Compare(fromDirectories[x], toDirectories[x], true) != 0)
				break;
			lastCommonRoot = x;
		}
		
		if (lastCommonRoot == -1)
		{
			throw new ArgumentException(
				string.Format("The paths '{0} and '{1}' do not have a common prefix path.",
					fromDirectory, toPath));
		}
		
		// add relative folders in from path
		for (int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
			if (fromDirectories[x].Length > 0)
				relativePath.Add("..");
		
		// add to folders to path
		for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
			relativePath.Add(toDirectories[x]);
		
		// create relative path
		string[] relativeParts = new string[relativePath.Count];
		relativePath.CopyTo(relativeParts, 0);
		string newPath = string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), relativeParts);
		return newPath;
	}
	
	public static bool WriteFile(string fileContents, string fileName, out string message)
	{
		StreamWriter sw = File.CreateText(fileName);
		try 
		{
			sw.Write(fileContents);
			message = string.Format("File {0} saved succesfully!", fileName);
			return true;
		} 
		catch (Exception e) 
		{
			message = e.Message;
			return false;
		} 
		finally 
		{
			if (sw != null)
			{
				sw.Flush();
				sw.Close();
			}
		}
	}
}