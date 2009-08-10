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
			
			
			entity.UserId = "Test Test Test Test";
			entity.OrderDate = DateTime.Now;
			entity.ShipAddr1 = "Test Test Test Test Test Test Test Test Test Te";
			entity.ShipAddr2 = "Test Test Test Test Test";
			entity.ShipCity = "Test Test Test Test Test Test Test Test T";
			entity.ShipState = "Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.ShipZip = "Test";
			entity.ShipCountry = "Test Test ";
			entity.BillAddr1 = "Test Test Test Test Test Test Test Test";
			entity.BillAddr2 = "Test Test Test Test Test Test Test Test Test Test Test ";
			entity.BillCity = "Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.BillState = "Test Test Test Test Test Test Test Test Test Test Test Test Test Te";
			entity.BillZip = "Test Test T";
			entity.BillCountry = "Test Test ";
			entity.Courier = "Test Test Test Test Test Test Test Te";
			entity.TotalPrice = 62;
			entity.BillToFirstName = "Test Test Test Test Test Test Test Test Test Te";
			entity.BillToLastName = "Test Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.ShipToFirstName = "Test Test T";
			entity.ShipToLastName = "Test Test Test ";
			entity.AuthorizationNumber = 38;
			entity.Locale = "Te";
			
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
				
				entityA.UserId = "Test Test";
				
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

