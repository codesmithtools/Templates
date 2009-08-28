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
    public partial class LineItemTests : UNuitTestBase
    {
        protected ILineItemManager manager;
		
		public LineItemTests()
        {
            manager = managerFactory.GetLineItemManager();
        }
		
		protected LineItem CreateNewLineItem()
		{
			LineItem entity = new LineItem();
			
			
			entity.OrderId = 19;
			entity.LineNum = 47;
			entity.ItemId = "Te";
			entity.Quantity = 94;
			entity.UnitPrice = 6;
			
			return entity;
		}
		protected LineItem GetFirstLineItem()
        {
            IList<LineItem> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				LineItem entity = CreateNewLineItem();
				
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
                LineItem entityA = CreateNewLineItem();
				manager.Save(entityA);

                LineItem entityB = manager.GetById(entityA.Id);

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
                LineItem entityA = GetFirstLineItem();
				
				entityA.ItemId = "Test";
				
				manager.Update(entityA);

                LineItem entityB = manager.GetById(entityA.Id);

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
                LineItem entity = GetFirstLineItem();
				
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

