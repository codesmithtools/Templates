using System;
using System.Xml.Linq;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public class NHibernateCommandProperty : PropertyBase<XElement>
    {
        public NHibernateCommandProperty(XElement element, IEntity entity)
            : base(element, entity)
        {
        }

        public override void Initialize() {
            var column = PropertySource.Attribute("column") ?? PropertySource.Attribute("name");
            var name = PropertySource.Attribute("name") ?? column;
            
            KeyName = column.Value;
            Name = name.Value;

            var type = PropertySource.Attribute("type").Value;
            var lengthAttribute = PropertySource.Attribute("length");
            int? length = null;
            if (lengthAttribute != null) {
                int lengthInt;
                if (Int32.TryParse(lengthAttribute.Value, out lengthInt))
                    length = lengthInt;
            }

            SystemType = NHibernateUtilities.FromNHibernateType(type, length);
        }
    }
}
