using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CodeSmith.Data.Audit
{
    /// <summary>
    /// A class representing a log of the changes to a <see cref="DataContext"/>.
    /// </summary>
    [XmlRoot(Namespace = AuditNamespace, ElementName = "audit")]
    public class AuditLog
    {
        /// <summary>
        /// The schema namespace for the audit log.
        /// </summary>
        public const string AuditNamespace = "http://schemas.codesmithtools.com/datacontext/audit/1.0";

        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof (AuditLog), AuditNamespace);
        private readonly List<AuditEntity> _entities = new List<AuditEntity>();

        /// <summary>
        /// Gets the list entities that have changes in the <see cref="DataContext"/>.
        /// </summary>
        /// <value>The list entities that have changes.</value>
        [XmlElement("entity", typeof (AuditEntity))]
        public List<AuditEntity> Entities
        {
            get { return _entities; }
        }

        /// <summary>
        /// Returns an xml string of the <see cref="AuditLog"/>.
        /// </summary>
        /// <returns>An xml string of the <see cref="AuditLog"/>.</returns>
        public string ToXml()
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            var builder = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                if (writer != null)
                {
                    _serializer.Serialize(writer, this);
                    writer.Flush();
                }
            }

            return builder.ToString();
        }
    }
}