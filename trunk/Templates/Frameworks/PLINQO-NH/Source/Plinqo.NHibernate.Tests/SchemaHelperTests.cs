using System.Linq;
using CodeSmith.SchemaHelper;
using CodeSmith.SchemaHelper.NHibernate;
using NUnit.Framework;

namespace Plinqo.NHibernate.Tests
{
    [TestFixture]
    public class SchemaHelperTests
    {
        [Test]
        public void NHibernateProviderTest()
        {
            var provider = new NHibernateProvider(@"D:\Code\CodeSmith\Experimental\PLINQO NHib\Test\Plinqo.NHib.Data\Maps");
            
            var manager = new EntityManager(provider);

            Assert.Greater(manager.Entities.Count(), 0);
        }
    }
}
