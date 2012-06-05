using System.Xml.Linq;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public class NHibernateAssociationProperty : PropertyBase<XElement>
    {
        private string _safeName;

        public NHibernateAssociationProperty(XElement property, IEntity entity)
            : base(property, entity) {}

        public override void Initialize() {
            KeyName = Name = _safeName = PropertySource.Attribute("name").Value;
        }

        public override string GetSafeName()
        {
            return _safeName;
        }
    }
}