using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace CodeSmith.Data.Linq
{
    public static class SerializationExtensions
    {
        /// <summary>
        /// Converts the object to a binary array by serializing.
        /// </summary>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <param name="item">The object to convert.</param>
        /// <returns>A serialized binary array of the object.</returns>
        public static byte[] ToBinary<T>(this T item)
        {
            if (item == null)
                return null;

            var serializer = new DataContractSerializer(typeof(T));

            byte[] buffer;
            using (var ms = new MemoryStream())
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ms))
            {
                serializer.WriteObject(writer, item);
                writer.Flush();
                buffer = ms.ToArray();
            }

            return buffer;
        }

        /// <summary>
        /// Deserializes an instance of T from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array representing a T instance.</param>
        /// <returns>An instance of T that is deserialized from the byte array.</returns>
        public static T ToObject<T>(this byte[] buffer)
        {
            var deserializer = new DataContractSerializer(typeof(T));

            using (var ms = new MemoryStream(buffer))
            using (var reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max))
            {
                return (T)deserializer.ReadObject(reader);
            }
        }

        /// <summary>
        /// Returns an XML <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>. 
        /// </summary>
        /// <returns>An XML <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.</returns>
        public static string ToXml<T>(this T item)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            var sb = new System.Text.StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(writer, item);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Deserializes an instance of T from XML.
        /// </summary>
        /// <param name="xml">The XML string representing a T instance.</param>
        /// <returns>An instance of T that is deserialized from the XML string.</returns>
        public static T ToObject<T>(this string xml)
        {
            var deserializer = new DataContractSerializer(typeof(T));

            using (var sr = new StringReader(xml))
            using (var reader = XmlReader.Create(sr))
            {
                return (T)deserializer.ReadObject(reader);
            }
        }

        /// <summary>
        /// Converts the collection to a binary array by serializing.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to convert.</param>
        /// <returns>A serialized binary array of the collection.</returns>
        public static byte[] ToBinary<T>(this ICollection<T> collection)
        {
            if (collection == null)
                return null;

            var serializer = new DataContractSerializer(typeof(ICollection<T>));

            byte[] buffer;
            using (var ms = new MemoryStream())
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ms))
            {
                serializer.WriteObject(writer, collection);
                writer.Flush();
                buffer = ms.ToArray();
            }

            return buffer;
        }

        /// <summary>
        /// Converts the byte array to a collection by deserializing.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="buffer">The byte array to convert.</param>
        /// <returns>An instance of <see cref="T:System.Collections.Generic.ICollection`1"/> deserialized from the byte array.</returns>
        public static ICollection<T> ToCollection<T>(this byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return null;

            var serializer = new DataContractSerializer(typeof(ICollection<T>));
            object value;

            using (var ms = new MemoryStream(buffer))
            using (var reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max))
            {
                ms.Position = 0;
                value = serializer.ReadObject(reader);
            }

            return value as ICollection<T>;
        }
    }
}
