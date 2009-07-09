using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web.Script.Serialization;

namespace CodeSmith.Data.Serialization
{
    /// <summary>
    /// Implements a byte array <see cref="JavaScriptConverter"/> for the <see cref="JavaScriptSerializer"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="ByteArrayConverter"/> converts the byte array to a base64 string. The converter supports 
    /// an array of <see cref="T:System.Byte"/> and <see cref="T:System.Data.Linq.Binary"/>.
    /// </remarks>
    public class ByteArrayConverter : JavaScriptConverter
    {
        private const string TypeKey = "base64";
        private static readonly Type[] ByteArrayTypes = new[] { typeof(byte[]), typeof(Binary) };

        /// <summary>
        /// When overridden in a derived class, converts the provided dictionary into an object of the specified type.
        /// </summary>
        /// <param name="dictionary">An <see cref="T:System.Collections.Generic.IDictionary`2"/> instance of property data stored as name/value pairs.</param>
        /// <param name="type">The type of the resulting object.</param>
        /// <param name="serializer">The <see cref="T:System.Web.Script.Serialization.JavaScriptSerializer"/> instance.</param>
        /// <returns>The deserialized object.</returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            byte[] bytes = null;

            if (dictionary.ContainsKey(TypeKey))
                bytes = Convert.FromBase64String(dictionary[TypeKey].ToString());

            if (bytes == null)
                return null;

            return type == typeof(Binary) ? new Binary(bytes) : bytes;
        }

        /// <summary>
        /// When overridden in a derived class, builds a dictionary of name/value pairs.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The object that is responsible for the serialization.</param>
        /// <returns>
        /// An object that contains key/value pairs that represent the object’s data.
        /// </returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var values = new Dictionary<string, object>();
            byte[] bytes = null;

            if (obj is Binary)
                bytes = ((Binary) obj).ToArray();
            else if (obj is byte[])
                bytes = (byte[]) obj;

            if (bytes != null)
                values.Add(TypeKey, Convert.ToBase64String(bytes));

            return values;
        }

        /// <summary>
        /// When overridden in a derived class, gets a collection of the supported types.
        /// </summary>
        /// <value></value>
        /// <returns>An object that implements <see cref="T:System.Collections.Generic.IEnumerable`1"/> that represents the types supported by the converter.</returns>
        public override IEnumerable<Type> SupportedTypes
        {
            get { return ByteArrayTypes; }
        }
    }
}