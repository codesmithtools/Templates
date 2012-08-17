using System.Web.Script.Serialization;

namespace CodeSmith.Data.Serialization
{
    /// <summary>
    /// JavaScriptSerializer extension methods.
    /// </summary>
    public static class JavaScriptSerializerExtensions
    {
        /// <summary>
        /// Registers the byte array converter.
        /// </summary>
        /// <param name="serializer">The <see cref="JavaScriptSerializer"/> instance.</param>
        public static void RegisterByteArrayConverter(this JavaScriptSerializer serializer)
        {
            serializer.RegisterConverters(new[] { new ByteArrayConverter() });
        }
    }
}