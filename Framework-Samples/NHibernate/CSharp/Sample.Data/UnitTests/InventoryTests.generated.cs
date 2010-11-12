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
    public partial class InventoryTests : UNuitTestBase
    {
        [SetUp]
        public void SetUp()
        {
            manager = managerFactory.GetInventoryManager();
            manager.Session.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            manager.Session.RollbackTransaction();
            manager.Dispose();
        }
        
        protected Sample.Data.Generated.ManagerObjects.IInventoryManager manager;
		
		protected Sample.Data.Generated.BusinessObjects.Inventory CreateNewInventory()
		{
			Sample.Data.Generated.BusinessObjects.Inventory entity = new Sample.Data.Generated.BusinessObjects.Inventory();
			
			// You may need to maually enter this key if there is a constraint violation.
			entity.Id = "Te";
			
			entity.Qty = 59;
			
			return entity;
		}
		protected Sample.Data.Generated.BusinessObjects.Inventory GetFirstInventory()
        {
            IList<Sample.Data.Generated.BusinessObjects.Inventory> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Sample.Data.Generated.BusinessObjects.Inventory entity = CreateNewInventory();
				
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
                Sample.Data.Generated.BusinessObjects.Inventory entityA = CreateNewInventory();
				manager.Save(entityA);

                Sample.Data.Generated.BusinessObjects.Inventory entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Inventory entityA = GetFirstInventory();
				
				entityA.Qty = 32;
				
				manager.Update(entityA);

                Sample.Data.Generated.BusinessObjects.Inventory entityB = manager.GetById(entityA.Id);

                Assert.AreEqual(entityA.Qty, entityB.Qty);
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
                Sample.Data.Generated.BusinessObjects.Inventory entity = GetFirstInventory();
				
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

