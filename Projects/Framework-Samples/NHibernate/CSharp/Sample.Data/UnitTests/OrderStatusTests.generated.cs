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
    public partial class OrderStatusTests : UNuitTestBase
    {
        protected Sample.Data.Generated.ManagerObjects.IOrderStatusManager manager;
		
		public OrderStatusTests()
        {
            manager = managerFactory.GetOrderStatusManager();
        }
		
		protected Sample.Data.Generated.BusinessObjects.OrderStatus CreateNewOrderStatus()
		{
			Sample.Data.Generated.BusinessObjects.OrderStatus entity = new Sample.Data.Generated.BusinessObjects.OrderStatus();
			
			
			entity.OrderId = 22;
			entity.LineNum = 81;
			entity.Timestamp = System.DateTime.Now;
			entity.Status = "T";
			
			return entity;
		}
		protected Sample.Data.Generated.BusinessObjects.OrderStatus GetFirstOrderStatus()
        {
            IList<Sample.Data.Generated.BusinessObjects.OrderStatus> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Sample.Data.Generated.BusinessObjects.OrderStatus entity = CreateNewOrderStatus();
				
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
                Sample.Data.Generated.BusinessObjects.OrderStatus entityA = CreateNewOrderStatus();
				manager.Save(entityA);

                Sample.Data.Generated.BusinessObjects.OrderStatus entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.OrderStatus entityA = GetFirstOrderStatus();
				
				entityA.Timestamp = System.DateTime.Now;
				
				manager.Update(entityA);

                Sample.Data.Generated.BusinessObjects.OrderStatus entityB = manager.GetById(entityA.Id);

                Assert.AreEqual(entityA.OrderId, entityB.OrderId);
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
                Sample.Data.Generated.BusinessObjects.OrderStatus entity = GetFirstOrderStatus();
				
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

