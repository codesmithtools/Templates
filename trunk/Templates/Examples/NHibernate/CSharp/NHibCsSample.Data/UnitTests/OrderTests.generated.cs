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
			
			
			entity.UserId = "Test Test T";
			entity.OrderDate = DateTime.Now;
			entity.ShipAddr1 = "Test Test Test Test Test Test Test Test Test Test Test Test Te";
			entity.ShipAddr2 = "Test Test Test Test Test Test Test Test";
			entity.ShipCity = "Test Test Test Test Test Test Test Test Test Te";
			entity.ShipState = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Te";
			entity.ShipZip = "Test Test";
			entity.ShipCountry = "Te";
			entity.BillAddr1 = "Test Test Test Test Test Test Test Test";
			entity.BillAddr2 = "Test Test Test Test Test Test Tes";
			entity.BillCity = "T";
			entity.BillState = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.BillZip = "Test Test Tes";
			entity.BillCountry = "Te";
			entity.Courier = "Test Test Test Test Test Test Test Test Test Test ";
			entity.TotalPrice = 24;
			entity.BillToFirstName = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test ";
			entity.BillToLastName = "Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.ShipToFirstName = "Test Test Te";
			entity.ShipToLastName = "Test Test Test Test Te";
			entity.AuthorizationNumber = 34;
			entity.Locale = "T";
			
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

