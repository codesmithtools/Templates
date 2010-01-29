using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Sample.Data.Generated.ManagerObjects;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.UnitTests
{
	[TestFixture]
    public partial class ProfileTests : UNuitTestBase
    {
        [SetUp]
        public void SetUp()
        {
            manager = managerFactory.GetProfileManager();
            manager.Session.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            manager.Session.RollbackTransaction();
            manager.Dispose();
        }
        
        protected Sample.Data.Generated.ManagerObjects.IProfileManager manager;
		
		protected Sample.Data.Generated.BusinessObjects.Profile CreateNewProfile()
		{
			Sample.Data.Generated.BusinessObjects.Profile entity = new Sample.Data.Generated.BusinessObjects.Profile();
			
			
			entity.Username = "Test Test ";
			entity.ApplicationName = "Test Test ";
			entity.IsAnonymous = true;
			entity.LastActivityDate = System.DateTime.Now;
			entity.LastUpdatedDate = System.DateTime.Now;
			
			return entity;
		}
		protected Sample.Data.Generated.BusinessObjects.Profile GetFirstProfile()
        {
            IList<Sample.Data.Generated.BusinessObjects.Profile> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Sample.Data.Generated.BusinessObjects.Profile entity = CreateNewProfile();
				
                object result = manager.Save(entity);

                Assert.IsNotNull(result);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
        [Test]
        public void Read()
        {
            try
            {
                Sample.Data.Generated.BusinessObjects.Profile entityA = CreateNewProfile();
				manager.Save(entityA);

                Sample.Data.Generated.BusinessObjects.Profile entityB = manager.GetById(entityA.Id);

                Assert.AreEqual(entityA, entityB);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
		[Test]
		public void Update()
        {
            try
            {
                Sample.Data.Generated.BusinessObjects.Profile entityA = GetFirstProfile();
				
				entityA.Username = "Test Test ";
				
				manager.Update(entityA);

                Sample.Data.Generated.BusinessObjects.Profile entityB = manager.GetById(entityA.Id);

                Assert.AreEqual(entityA.Username, entityB.Username);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
        [Test]
        public void Delete()
        {
            try
            {
                Sample.Data.Generated.BusinessObjects.Profile entity = GetFirstProfile();
				
                manager.Delete(entity);

                entity = manager.GetById(entity.Id);
                Assert.IsNull(entity);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
	}
}

