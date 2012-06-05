using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public static class NHibernateExtensions
    {
        public static XElement Descendant(this XElement element, string name, string namespaceName)
        {
            return element
                .Descendants(name, namespaceName)
                .FirstOrDefault();
        }

        public static IEnumerable<XElement> Descendants(this XElement element, string name, string namespaceName)
        {
            return element
                .Descendants(XName.Get(name, namespaceName));
        }

        public static bool HasAttributeValue(this XElement element, string key, string value)
        {
            return element
                .Attributes(key)
                .Any(a => a.Value == value);
        }

        public static void AddOrSet(this Dictionary<string, object> dictionary, KeyValuePair<string, object> pair)
        {
            if (dictionary.ContainsKey(pair.Key))
                dictionary[pair.Key] = pair.Value;
            else
                dictionary.Add(pair.Key, pair.Value);
        }
    }
}