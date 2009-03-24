using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NHibCsSample.Generated.ManagerObjects;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.UnitTests
{
	[TestFixture]
    public partial class ProfileTests : UNuitTestBase
    {
        protected IProfileManager manager;
		
		public ProfileTests()
        {
            manager = managerFactory.GetProfileManager();
        }
		
		protected Profile CreateNewProfile()
		{
			Profile entity = new Profile();
			
			
			entity.Username = "Test Test ";
			entity.ApplicationName = "Test Test ";
			entity.IsAnonymous = true;
			entity.LastActivityDate = DateTime.Now;
			entity.LastUpdatedDate = DateTime.Now;
			
			return entity;
		}
		protected Profile GetFirstProfile()
        {
            IList<Profile> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Profile entity = CreateNewProfile();
				
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
                Profile entityA = CreateNewProfile();
				manager.Save(entityA);

                Profile entityB = manager.GetById(entityA.Id);

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
                Profile entityA = GetFirstProfile();
				
				entityA.Username = "Test Test ";
				
				manager.Update(entityA);

                Profile entityB = manager.GetById(entityA.Id);

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
                Profile entity = GetFirstProfile();
				
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

