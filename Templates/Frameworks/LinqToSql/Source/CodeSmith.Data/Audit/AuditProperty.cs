using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace CodeSmith.Data.Audit
{
    /// <summary>
    /// A class for logging the changes to a property on an entity.
    /// </summary>
    [XmlRoot(Namespace = AuditLog.AuditNamespace, ElementName = "property")]
    public class AuditProperty
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the current/changed value of the property.
        /// </summary>
        /// <value>The current value of the property.</value>
        [XmlElement("current")]
        public object Current { get; set; }

        /// <summary>
        /// Gets or sets the original value of the property.
        /// </summary>
        /// <value>The original value of the property.</value>
        [XmlElement("original")]
        public object Original { get; set; }
    }
}