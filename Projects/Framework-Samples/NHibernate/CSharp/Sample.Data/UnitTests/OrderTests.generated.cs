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
			
			
			entity.UserId = "Test Test Te";
			entity.OrderDate = DateTime.Now;
			entity.ShipAddr1 = "Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.ShipAddr2 = "Test Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.ShipCity = "Test Test Te";
			entity.ShipState = "Test Test Tes";
			entity.ShipZip = "Tes";
			entity.ShipCountry = "Te";
			entity.BillAddr1 = "T";
			entity.BillAddr2 = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Te";
			entity.BillCity = "Test Test Test Test Test Test Test Test ";
			entity.BillState = "Test Test Test Test Test Test Test Test Test Test Test";
			entity.BillZip = "Test Test Test Te";
			entity.BillCountry = "Test Test Test Test";
			entity.Courier = "Test Test Test Test Test Test Test Test Test Test Test Test Test T";
			entity.TotalPrice = 72;
			entity.BillToFirstName = "Test Test Test Test Test Test Test Test Test Test Test";
			entity.BillToLastName = "Test Test Test Test Test Test Test Test Test Test Test Te";
			entity.ShipToFirstName = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test T";
			entity.ShipToLastName = "Test Test ";
			entity.AuthorizationNumber = 48;
			entity.Locale = "Test Test Test";
			
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
				
				entityA.UserId = "Test Test Test";
				
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

