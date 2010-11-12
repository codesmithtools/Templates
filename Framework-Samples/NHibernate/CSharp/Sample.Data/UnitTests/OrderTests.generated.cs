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
    public partial class OrderTests : UNuitTestBase
    {
        [SetUp]
        public void SetUp()
        {
            manager = managerFactory.GetOrderManager();
            manager.Session.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            manager.Session.RollbackTransaction();
            manager.Dispose();
        }
        
        protected Sample.Data.Generated.ManagerObjects.IOrderManager manager;
		
		protected Sample.Data.Generated.BusinessObjects.Order CreateNewOrder()
		{
			Sample.Data.Generated.BusinessObjects.Order entity = new Sample.Data.Generated.BusinessObjects.Order();
			
			
			entity.UserId = "Test Tes";
			entity.OrderDate = System.DateTime.Now;
			entity.ShipAddr1 = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.ShipAddr2 = "Test Test Test Test Test Test Test Test";
			entity.ShipCity = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.ShipState = "Test Test Test Test Te";
			entity.ShipZip = "Test T";
			entity.ShipCountry = "Tes";
			entity.BillAddr1 = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Te";
			entity.BillAddr2 = "Test Test Test Test Test Tes";
			entity.BillCity = "Test Test Test Test Test Test Test Test Test Test Test Test Test T";
			entity.BillState = "Test Test Test Test Test Te";
			entity.BillZip = "Test Te";
			entity.BillCountry = "Test Test Test";
			entity.Courier = "Test Test Test Tes";
			entity.TotalPrice = 91;
			entity.BillToFirstName = "Test Test Test Test Test Test Test Test ";
			entity.BillToLastName = "Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.ShipToFirstName = "Test Test Test Test Test Test Test Test Test Test Test Test ";
			entity.ShipToLastName = "Test Test Te";
			entity.AuthorizationNumber = 99;
			entity.Locale = "Test Test Test";
			
			return entity;
		}
		protected Sample.Data.Generated.BusinessObjects.Order GetFirstOrder()
        {
            IList<Sample.Data.Generated.BusinessObjects.Order> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Sample.Data.Generated.BusinessObjects.Order entity = CreateNewOrder();
				
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
                Sample.Data.Generated.BusinessObjects.Order entityA = CreateNewOrder();
				manager.Save(entityA);

                Sample.Data.Generated.BusinessObjects.Order entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Order entityA = GetFirstOrder();
				
				entityA.UserId = "T";
				
				manager.Update(entityA);

                Sample.Data.Generated.BusinessObjects.Order entityB = manager.GetById(entityA.Id);

                Assert.AreEqual(entityA.UserId, entityB.UserId);
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
                Sample.Data.Generated.BusinessObjects.Order entity = GetFirstOrder();
				
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

