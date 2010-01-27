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
    public partial class AccountTests : UNuitTestBase
    {
        protected Sample.Data.Generated.ManagerObjects.IAccountManager manager;
		
		public AccountTests()
        {
            manager = managerFactory.GetAccountManager();
        }
		
		protected Sample.Data.Generated.BusinessObjects.Account CreateNewAccount()
		{
			Sample.Data.Generated.BusinessObjects.Account entity = new Sample.Data.Generated.BusinessObjects.Account();
			
			
			entity.Email = "Test T";
			entity.FirstName = "Test Test Test Test Test Test";
			entity.LastName = "Test Test Test Test T";
			entity.Address1 = "Te";
			entity.Address2 = "Test Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.City = "Test Test Test Test Test Test Test Test Test Test Test Test Test ";
			entity.State = "Test Test Test Test Test Test Test Test Test ";
			entity.Zip = "Test Test Test T";
			entity.Country = "Test Test Test T";
			entity.Phone = "Test Test";
			entity.Number = 30;
			
			Sample.Data.Generated.ManagerObjects.IProfileManager profileManager = managerFactory.GetProfileManager();
			entity.Profile = profileManager.GetAll(1)[0];
			
			return entity;
		}
		protected Sample.Data.Generated.BusinessObjects.Account GetFirstAccount()
        {
            IList<Sample.Data.Generated.BusinessObjects.Account> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Sample.Data.Generated.BusinessObjects.Account entity = CreateNewAccount();
				
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
                Sample.Data.Generated.BusinessObjects.Account entityA = CreateNewAccount();
				manager.Save(entityA);

                Sample.Data.Generated.BusinessObjects.Account entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Account entityA = GetFirstAccount();
				
				entityA.Email = "Test T";
				
				manager.Update(entityA);

                Sample.Data.Generated.BusinessObjects.Account entityB = manager.GetById(entityA.Id);

                Assert.AreEqual(entityA.Email, entityB.Email);
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
                Sample.Data.Generated.BusinessObjects.Account entity = GetFirstAccount();
				
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

