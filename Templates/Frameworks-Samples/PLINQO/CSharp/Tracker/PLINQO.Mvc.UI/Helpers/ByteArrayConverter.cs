using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PLINQO.Mvc.UI.Helpers
{
    /// <summary>
    /// Converts a <see cref="byte"/> Array to and from a Base64 string.
    /// </summary>
    public class ByteArrayConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        public override void WriteJson(JsonWriter writer, object value)
        {
            byte[] bytes = null;

            if (value is Binary)
                bytes = ((Binary)value).ToArray();
            else if (value is byte[])
                bytes = (byte[])value;
            else
                return;

            string text = Convert.ToBase64String(bytes);
            writer.WriteValue(text);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.String)
                throw new Exception(string.Format("Unexpected token parsing byte array. Expected String, got {0}.", reader.TokenType));

            string value = reader.Value.ToString();
            byte[] bytes = Convert.FromBase64String(value);
            return objectType == typeof(Binary) ? new Binary(bytes) : bytes;
            ;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return (objectType.IsArray && objectType.Equals(typeof(byte[]))) || objectType.Equals(typeof(Binary));
        }
    }
}
