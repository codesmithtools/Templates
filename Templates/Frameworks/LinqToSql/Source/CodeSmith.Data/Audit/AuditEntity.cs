using System.Collections.Generic;
using System.Xml.Serialization;

namespace CodeSmith.Data.Audit
{
    /// <summary>
    /// A class for logging the changes to an entity.
    /// </summary>
    [XmlRoot(Namespace = AuditLog.AuditNamespace, ElementName = "entity")]
    public class AuditEntity
    {
        private readonly List<AuditProperty> _properties = new List<AuditProperty>();

        /// <summary>
        /// Gets or sets the action that was taken on the entity.
        /// </summary>
        /// <value>The action that was taken on the entity.</value>
        [XmlAttribute("action")]
        public AuditAction Action { get; set; }

        /// <summary>
        /// Gets or sets the data type of the entity.
        /// </summary>
        /// <value>The data type of the entity.</value>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets the list of properties that action was taken on.
        /// </summary>
        /// <value>The list of properties that action was taken on.</value>
        [XmlElement("property", typeof(AuditProperty))]
        public List<AuditProperty> Properties
        {
            get { return _properties; }
        }
    }
}