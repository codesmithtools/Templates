using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;

public static class Generator
{
    public static TypeScriptInterface CreateInterface(string assemblyFile, string rootType, bool odata = false)
    {
        var moduleDefinition = ModuleDefinition.ReadModule(assemblyFile);
        if (moduleDefinition == null)
            throw new ArgumentException(string.Format("The assembly '{0}' could not be loaded.", assemblyFile), "assemblyFile");

        var typeDefintion = moduleDefinition.GetType(rootType);
        if (typeDefintion == null)
            throw new ArgumentException(string.Format("The type '{0}' could not be found.", rootType), "rootType");

        var scriptInterface = new TypeScriptInterface();
        scriptInterface.Name = typeDefintion.Name;

        // get all properties
        var propertyDefinitions = typeDefintion
            .Traverse(t => t.BaseType == null ? null : t.BaseType.Resolve())
            .SelectMany(t => t.Properties);

        foreach (var propertyDefinition in propertyDefinitions)
        {
            var scriptPropery = new TypeScriptProperty();
            scriptPropery.Name = propertyDefinition.Name;

            var propertyType = propertyDefinition.PropertyType;
            scriptPropery.Type = propertyType.ToScriptType(odata);
            scriptPropery.IsNullable = propertyDefinition.PropertyType.IsNullable();
            scriptPropery.IsArray = propertyDefinition.PropertyType.IsScriptArray();
             scriptInterface.Properties.Add(scriptPropery);
        }

        return scriptInterface;
    }
}

public class TypeScriptInterface
{
    public TypeScriptInterface()
    {
        Properties = new List<TypeScriptProperty>();
    }

    public string Name { get; set; }

    public string Extends { get; set; }

    public List<TypeScriptProperty> Properties { get; set; }
}

public class TypeScriptProperty
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsNullable { get; set; }
    public bool IsArray { get; set; }

    public override string ToString()
    {
        return string.Format("Name: {0}, Type: {1}, Nullable: {2}, Array: {3}", Name, Type, IsNullable, IsArray);
    }
}

public static class Extensions
{
    public static TypeReference GetUnderlyingType(this TypeReference type)
    {
        if (!type.IsGenericInstance)
            return type;

        var genericType = type as GenericInstanceType;
        if (genericType == null)
            return type;

        var genericDefinition = genericType.Resolve();
        if (genericDefinition.GenericParameters.Count != 1 && genericDefinition.FullName != "System.Nullable`1")
            return type;

        return genericType.GenericArguments.Single();
    }

    public static bool HasInterface(this TypeReference typeReference, string fullName)
    {
        TypeDefinition definition = typeReference.Resolve();

        return definition.Interfaces
            .Select(i => i.Resolve())
            .Any(d => d.FullName == fullName);
    }

    public static bool IsNullable(this TypeReference type)
    {
        bool isNullable = type.IsValueType == false;
        if (!type.IsGenericInstance)
            return isNullable;

        var genericType = type as GenericInstanceType;
        if (genericType == null)
            return isNullable;

        var genericDefinition = genericType.Resolve();
        if (genericDefinition.GenericParameters.Count == 1 && genericDefinition.FullName == "System.Nullable`1")
            return true;

        return isNullable;
    }

    public static bool IsScriptArray(this TypeReference typeReference)
    {
        var baseType = typeReference.GetUnderlyingType();
        if (baseType.FullName == "System.String" || baseType.FullName == "System.Byte[]")
            return false;

        if (baseType.IsArray)
            return true;

        TypeReference elementType = null;
        var isEnumerable = IsEnumerable(baseType, out elementType);
        if (isEnumerable)
            return true;

        return false;
    }
    
    public static bool IsEnumerable(this TypeReference typeReference, out TypeReference elementType)
    {
        elementType = null;
        
        var collectionType = typeReference
            .Traverse(t => t.Resolve().BaseType)
            .FirstOrDefault(t => t.HasInterface("System.Collections.Generic.IEnumerable`1"));

        var genericCollectionType = collectionType as GenericInstanceType;
        if (genericCollectionType == null)
            return false;

        //NOTE issue with custom implementation and matching up argument position
        elementType = genericCollectionType.GenericArguments.Single();
        return true;
    }

    public static bool IsCollection(this TypeReference typeReference, out TypeReference elementType)
    {
        elementType = null;

        var collectionType = typeReference
            .Traverse(t => t.Resolve().BaseType)
            .FirstOrDefault(t => t.HasInterface("System.Collections.Generic.ICollection`1"));

        var genericCollectionType = collectionType as GenericInstanceType;
        if (genericCollectionType == null)
            return false;

        //NOTE issue with custom implementation and matching up argument position
        elementType = genericCollectionType.GenericArguments.Single();
        return true;
    }

    public static bool IsDictionary(this TypeReference typeReference, out TypeReference keyType, out TypeReference elementType)
    {
        keyType = null;
        elementType = null;

        var dictionaryType = typeReference
            .Traverse(t => t.Resolve().BaseType)
            .FirstOrDefault(t => t.HasInterface("System.Collections.Generic.IDictionary`2"));

        var genericDictionaryType = dictionaryType as GenericInstanceType;
        if (genericDictionaryType == null)
            return false;

        //NOTE issue with custom implementation and matching up argument position
        keyType = genericDictionaryType.GenericArguments.First();
        elementType = genericDictionaryType.GenericArguments.Last();

        return true;
    }

    public static string ToScriptType(this TypeReference typeReference, bool odata = false)
    {
        var t = typeReference.GetUnderlyingType();

        switch (t.Name)
        {
            case "Int16":
            case "Int32":
            case "Byte":
            case "Double":
            case "SByte":
            case "Single":
            case "UInt16":
            case "UInt32":
                return "number";
            case "Decimal":
            case "Int64":
            case "UInt64":
                return odata ? "string" : "number";
            case "Boolean":
                return "boolean";
            case "DateTime":
            case "DateTimeOffset":
                return "Date";
            case "String":
            case "Guid":
            case "TimeSpan":
                return "string";
            case "Byte[]":
                return "string";
            default:
                return "any";
        }
    }


    public static IEnumerable<T> Traverse<T>(this T root, Func<T, IEnumerable<T>> childrenSelector)
    {
        yield return root;

        IEnumerable<T> descendants = childrenSelector(root);
        if (descendants == null)
            yield break;

        foreach (T descendant in descendants.SelectMany(element => element.Traverse(childrenSelector)))
            yield return descendant;
    }

    public static IEnumerable<T> Traverse<T>(this T root, Func<T, T> childrenSelector) where T : class
    {
        T descendant = root;
        while (descendant != null)
        {
            yield return descendant;
            descendant = childrenSelector(descendant);
        }
    }


    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        return source.DistinctBy(keySelector, EqualityComparer<TKey>.Default);
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        if (keySelector == null)
            throw new ArgumentNullException("keySelector");
        if (comparer == null)
            throw new ArgumentNullException("comparer");

        var knownKeys = new HashSet<TKey>(comparer);
        return source.Where(element => knownKeys.Add(keySelector(element)));
    }
}
