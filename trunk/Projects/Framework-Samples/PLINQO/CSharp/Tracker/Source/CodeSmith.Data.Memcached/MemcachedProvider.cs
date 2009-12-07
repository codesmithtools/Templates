using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using CodeSmith.Data.Caching;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace CodeSmith.Data.Memcached
{
    public class MemcachedProvider : CacheProvider
    {
        public override void Save<T>(string key, T data, CacheSettings settings)
        {
            if (data == null)
                return;

            var serializer = new DataContractSerializer(typeof(T));

            byte[] buffer;
            using (var ms = new MemoryStream())
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ms))
            {
                serializer.WriteObject(writer, data);
                writer.Flush();
                buffer = ms.ToArray();
            }

            switch (settings.Mode)
            {
                case CacheExpirationMode.Duration:
                    Client.Store(StoreMode.Set, key, buffer, 0, buffer.Length, DateTime.UtcNow.Add(settings.Duration));
                    break;
                case CacheExpirationMode.Sliding:
                    Client.Store(StoreMode.Set, key, buffer, 0, buffer.Length, settings.Duration);
                    break;
                case CacheExpirationMode.Absolute:
                    Client.Store(StoreMode.Set, key, buffer, 0, buffer.Length, settings.AbsoluteExpiration);
                    break;
                default:
                    Client.Store(StoreMode.Set, key, buffer, 0, buffer.Length);
                    break;
            }
        }

        public override void Remove(string key)
        {
            Client.Remove(key);
        }

        public override T Get<T>(string key)
        {
            var serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(T));

            byte[] buffer = Client.Get(key) as byte[];

            if (buffer == null || buffer.Length == 0)
                return default(T);

            object value;
            using (var ms = new MemoryStream(buffer))
            using (var reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max))
            {
                ms.Position = 0;
                value = serializer.ReadObject(reader);
            }

            return value == null ? default(T) : (T)value;
        }


        private static MemcachedClient Client
        {
            get { return Nested.Client; }
        }

        /// <summary>
        /// Nested class to lazy-load singleton.
        /// </summary>
        private class Nested
        {
            internal static readonly MemcachedClient Client = new MemcachedClient();
        }

    }
}
