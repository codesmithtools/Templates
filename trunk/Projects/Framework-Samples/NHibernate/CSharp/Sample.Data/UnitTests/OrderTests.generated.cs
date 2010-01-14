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
			
			
			entity.UserId = "Test T";
			entity.OrderDate = DateTime.Now;
			entity.ShipAddr1 = "Test Te";
			entity.ShipAddr2 = "Test Test Test Test Test Test Te";
			entity.ShipCity = "Test Test Test ";
			entity.ShipState = "Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.ShipZip = "Test Test Te";
			entity.ShipCountry = "Test Test Tes";
			entity.BillAddr1 = "Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.BillAddr2 = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.BillCity = "Test Test Test";
			entity.BillState = "Test Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.BillZip = "T";
			entity.BillCountry = "Test Te";
			entity.Courier = "Test Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.TotalPrice = 67;
			entity.BillToFirstName = "Test Test Test T";
			entity.BillToLastName = "Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.ShipToFirstName = "Test Test Test Test Test Test T";
			entity.ShipToLastName = "Test Test Test Test Test Test Test Test Test Test Test Te";
			entity.AuthorizationNumber = 29;
			entity.Locale = "Test Test Test Te";
			
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
				
				entityA.UserId = "Test Tes";
				
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

