/*
 * This file contains script types and methods 
 * that are common to all collection templates.
 */

public enum AccessibilityEnum {
    Public,
    Protected,
    Internal,
    ProtectedInternal,
    Private
}

public string GetAccessModifier(AccessibilityEnum accessibility) {
    switch (accessibility) {
        case AccessibilityEnum.Public:            return "public";
        case AccessibilityEnum.Protected:         return "protected";
        case AccessibilityEnum.Internal:          return "internal";
        case AccessibilityEnum.ProtectedInternal: return "protected internal";
        case AccessibilityEnum.Private:           return "private";
        default:                                  return "public";
    }
}

public bool IsInt32(string type) {
    if (type == null || type.Length == 0) return false;
    type = type.ToLower();
    return (type == "int" || type == "integer" || type == "int32" 
        || type == "system.int32" || type == "::system.int32");
}

public bool IsString(string type) {
    if (type == null || type.Length == 0) return false;
    type = type.ToLower();
    return (type == "string" || type == "system.string" 
        || type == "::system.string");
}

private System.Collections.Specialized.StringCollection
    _namespaces = new System.Collections.Specialized.StringCollection();

public void UsingNamespace(string ns) {
    if (!IncludeNamespaces) return;
    if (ns == null || ns.Length == 0) return;
    if (_namespaces.Contains(ns)) return;

    if (_namespaces.Count == 0) Response.WriteLine();
    Response.WriteLine("using " + ns + ";");
    _namespaces.Add(ns);
}

public void StartNamespace(string ns) {
    if (!IncludeNamespaces) return;
    if (ns == null || ns.Length == 0) return;
    Response.WriteLine();
    Response.WriteLine("namespace " + ns + " {");
}

public void EndNamespace(string ns) {
    if (!IncludeNamespaces) return;
    if (ns == null || ns.Length == 0) return;
    Response.WriteLine("}");
}

public CodeTemplate GenerateInterfaces(string file) {
    CodeTemplate template = GetCodeTemplateInstance(file);
    if (template == null) return null;

    CopyPropertiesTo(template);
    template.SetProperty("IncludeNamespaces", false);
    template.Render(Response);

    return template;
}