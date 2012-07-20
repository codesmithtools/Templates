using System;
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
        /// Gets or sets the user name that made the changes.
        /// </summary>
        /// <value>The user name that made the changes.</value>
        [XmlAttribute("username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the date when the changes were made.
        /// </summary>
        /// <value>The date when the changes were made.</value>
        [XmlAttribute("date")]
        public DateTime Date { get; set; }

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
        /// Returns an XML string of the <see cref="AuditLog"/>.
        /// </summary>
        /// <returns>An XML string of the <see cref="AuditLog"/>.</returns>
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

        /// <summary>
        /// Returns an <see cref="AuditLog"/> object created from an XML string.
        /// </summary>
        /// <param name="auditLog">
        /// An XML string
        /// </param>
        /// <returns>
        /// An <see cref="AuditLog"/> object created from an XML string.
        /// </returns>
        public static AuditLog FromXml(string auditLog)
        {
            if (string.IsNullOrEmpty(auditLog))
                return new AuditLog(); 
            
            try
            {
                using (var reader = new StringReader(auditLog))
                using (XmlReader xr = XmlReader.Create(reader))
                {
                    return FromXml(xr);
                }
            }
            catch
            {
                return new AuditLog();
            }
        }

        /// <summary>
        /// Returns an <see cref="AuditLog"/> object created from an XML string.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> to create the AuditLog from.</param>
        /// <returns>
        /// An <see cref="AuditLog"/> object created from an <see cref="XmlReader"/>.
        /// </returns>
        public static AuditLog FromXml(XmlReader reader)
        {
            if (reader == null)
                return new AuditLog(); 

            try
            {
                return _serializer.Deserialize(reader) as AuditLog;
            }
            catch
            {
                return new AuditLog();
            }
        }
    }
}