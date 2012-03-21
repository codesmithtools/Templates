using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;

public class Visitor
{
    public Visitor()
    {
        Children = new List<Visitor>();
    }

    public bool IsDictionary { get; set; }
    public bool IsCollection { get; set; }
    public TypeDefinition VisitType { get; set; }
    public PropertyDefinition Property { get; set; }
    public List<Visitor> Children { get; set; }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("Type: ");
        stringBuilder.Append(VisitType == null ? "{null}" : VisitType.Name);
        stringBuilder.Append(" Property: ");
        stringBuilder.Append(Property == null ? "{null}" : Property.Name);
        return stringBuilder.ToString();
    }

    public static Visitor Create(string assemblyFile, string rootType)
    {
        var moduleDefinition = ModuleDefinition.ReadModule(assemblyFile);
        if (moduleDefinition == null)
            throw new ArgumentException(string.Format("The assembly '{0}' could not be loaded.", assemblyFile), "assemblyFile");

        var typeDefintion = moduleDefinition.GetType(rootType);
        if (typeDefintion == null)
            throw new ArgumentException(string.Format("The type '{0}' could not be found.", rootType), "rootType");

        var top = new Visitor { VisitType = typeDefintion };
        GetChildren(top);
        return top;
    }

    private static void GetChildren(Visitor parent)
    {
        // get all properties
        var propertyDefinitions = parent.VisitType
            .Traverse(t => t.BaseType == null ? null : t.BaseType.Resolve())
            .SelectMany(t => t.Properties);

        foreach (PropertyDefinition property in propertyDefinitions)
        {
            var propertyType = property.PropertyType;
            if (propertyType.IsValueType || propertyType.FullName == "System.String")
                continue;

            var child = new Visitor();
            child.Property = property;

            TypeReference keyType;
            TypeReference elementType;
            if (propertyType.IsDictionary(out keyType, out elementType))
            {
                child.IsDictionary = true;
                child.VisitType = elementType.Resolve();
            }
            else if (propertyType.IsCollection(out elementType))
            {
                child.IsCollection = true;
                child.VisitType = elementType.Resolve();
            }
            else
            {
                child.VisitType = propertyType.Resolve();
            }
            parent.Children.Add(child);


            GetChildren(child);
        }
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
