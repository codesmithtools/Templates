using System.Xml.Linq;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public class NHibernateAssociationProperty : PropertyBase<XElement>
    {
        private string _safeName;

        public NHibernateAssociationProperty(XElement property, IEntity entity) : base(property, entity)
        {
            _safeName = property.Attribute("name").Value;
        }

        public override void Initialize()
        {
        }

        public override string GetSafeName()
        {
            return _safeName;
        }
    }
}