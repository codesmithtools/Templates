using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public class NHibernateCommandProperty : PropertyBase<XElement>
    {
        public NHibernateCommandProperty(XElement element, IEntity entity)
            : base(element, entity)
        {
        }

        public override void Initialize()
        {
            var nameAttribute = PropertySource.Attribute("name");
            KeyName = nameAttribute != null
                ? nameAttribute.Value
                : PropertySource.Attribute("column").Value;

            var type = PropertySource.Attribute("type").Value;
            var lengthAttribute = PropertySource.Attribute("length");
            int? length = null;
            if (lengthAttribute != null)
            {
                int lengthInt;
                if (Int32.TryParse(lengthAttribute.Value, out lengthInt))
                    length = lengthInt;
            }
            SystemType = NHibernateUtilities.FromNHibernateType(type, length);
        }
    }
}
