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
    public partial class OrderTests : UnitTestbase
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
			
			
			entity.UserId = "Te";
			entity.OrderDate = System.DateTime.Now;
			entity.ShipAddr1 = "Test Test Test ";
			entity.ShipAddr2 = "Test Test Test Tes";
			entity.ShipCity = "Test Test Test Test Test Test Test Test Test Test Te";
			entity.ShipState = "Test Test Test Test Tes";
			entity.ShipZip = "Test Test Te";
			entity.ShipCountry = "Test Test Test Tes";
			entity.BillAddr1 = "Test Test Te";
			entity.BillAddr2 = "Test Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.BillCity = "Test Test Test Test Test Test Test Test T";
			entity.BillState = "Test Test Test Test Test Test Test Test Test Test Test T";
			entity.BillZip = "Test T";
			entity.BillCountry = "Test T";
			entity.Courier = "Test Test Test Test Test Test T";
			entity.TotalPrice = 98;
			entity.BillToFirstName = "Test Test Test Test Test Test Tes";
			entity.BillToLastName = "Test Test Test Test Test T";
			entity.ShipToFirstName = "Test Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.ShipToLastName = "Test Test Test Test Test Test Test";
			entity.AuthorizationNumber = 80;
			entity.Locale = "Test Test Test T";
			
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
				
				entityA.UserId = "Test Test Test T";
				
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

