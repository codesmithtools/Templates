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
        protected Sample.Data.Generated.ManagerObjects.ILineItemManager manager;
		
		public LineItemTests()
        {
            manager = managerFactory.GetLineItemManager();
        }
		
		protected Sample.Data.Generated.BusinessObjects.LineItem CreateNewLineItem()
		{
			Sample.Data.Generated.BusinessObjects.LineItem entity = new Sample.Data.Generated.BusinessObjects.LineItem();
			
			
			entity.OrderId = 20;
			entity.LineNum = 91;
			entity.ItemId = "Tes";
			entity.Quantity = 35;
			entity.UnitPrice = 78;
			
			return entity;
		}
		protected Sample.Data.Generated.BusinessObjects.LineItem GetFirstLineItem()
        {
            IList<Sample.Data.Generated.BusinessObjects.LineItem> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Sample.Data.Generated.BusinessObjects.LineItem entity = CreateNewLineItem();
				
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
                Sample.Data.Generated.BusinessObjects.LineItem entityA = CreateNewLineItem();
				manager.Save(entityA);

                Sample.Data.Generated.BusinessObjects.LineItem entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.LineItem entityA = GetFirstLineItem();
				
				entityA.ItemId = "T";
				
				manager.Update(entityA);

                Sample.Data.Generated.BusinessObjects.LineItem entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.LineItem entity = GetFirstLineItem();
				
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

