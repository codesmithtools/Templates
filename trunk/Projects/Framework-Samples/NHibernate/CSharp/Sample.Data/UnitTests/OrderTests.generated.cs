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
        protected IOrderManager manager;
		
		public OrderTests()
        {
            manager = managerFactory.GetOrderManager();
        }
		
		protected Order CreateNewOrder()
		{
			Order entity = new Order();
			
			
			entity.UserId = "Test Test Tes";
			entity.OrderDate = DateTime.Now;
			entity.ShipAddr1 = "Test Test Test Test Test T";
			entity.ShipAddr2 = "Test Test Test Test Test Test ";
			entity.ShipCity = "Test Test Test Test Test Test Test Test Test Tes";
			entity.ShipState = "Test Test Test Test Test Test Test Test Test Test";
			entity.ShipZip = "Test Test Test Te";
			entity.ShipCountry = "T";
			entity.BillAddr1 = "Test Test Test Test Test Test Test Tes";
			entity.BillAddr2 = "Test Test Test Test Test Test Test Test Test Test Test T";
			entity.BillCity = "Test Test Test Test Test Test Test Test Test T";
			entity.BillState = "Test Test Test Test Test Test Test Test Test Test Test Test Test T";
			entity.BillZip = "Test Test T";
			entity.BillCountry = "Te";
			entity.Courier = "Test Test Test Test Test";
			entity.TotalPrice = 95;
			entity.BillToFirstName = "Test Test Test T";
			entity.BillToLastName = "Test Test Test Test Test T";
			entity.ShipToFirstName = "Test Test Test Test Test Test Test Test Test Test";
			entity.ShipToLastName = "Test Test Test Test Test Test";
			entity.AuthorizationNumber = 57;
			entity.Locale = "Test T";
			
			return entity;
		}
		protected Order GetFirstOrder()
        {
            IList<Order> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Order entity = CreateNewOrder();
				
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
                Order entityA = CreateNewOrder();
				manager.Save(entityA);

                Order entityB = manager.GetById(entityA.Id);

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
                Order entityA = GetFirstOrder();
				
				entityA.UserId = "Tes";
				
				manager.Update(entityA);

                Order entityB = manager.GetById(entityA.Id);

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
                Order entity = GetFirstOrder();
				
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

