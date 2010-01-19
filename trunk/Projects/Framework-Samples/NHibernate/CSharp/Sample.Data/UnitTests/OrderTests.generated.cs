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
			
			
			entity.UserId = "Tes";
			entity.OrderDate = DateTime.Now;
			entity.ShipAddr1 = "Test Test Test Test ";
			entity.ShipAddr2 = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Te";
			entity.ShipCity = "Test Test ";
			entity.ShipState = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.ShipZip = "Test";
			entity.ShipCountry = "Test Test Te";
			entity.BillAddr1 = "Test Test Test Test Test";
			entity.BillAddr2 = "Test Test Te";
			entity.BillCity = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.BillState = "Tes";
			entity.BillZip = "Test Test Test T";
			entity.BillCountry = "Test Test Test Test";
			entity.Courier = "Test Test Test Test T";
			entity.TotalPrice = 34;
			entity.BillToFirstName = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Te";
			entity.BillToLastName = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.ShipToFirstName = "Te";
			entity.ShipToLastName = "Test Test Test Test Test Test Test Test Te";
			entity.AuthorizationNumber = 76;
			entity.Locale = "Test Test Te";
			
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
				
				entityA.UserId = "Test Test Test Te";
				
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

