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
        protected IInventoryManager manager;
		
		public InventoryTests()
        {
            manager = managerFactory.GetInventoryManager();
        }
		
		protected Inventory CreateNewInventory()
		{
			Inventory entity = new Inventory();
			
			// You may need to maually enter this key if there is a constraint violation.
			entity.Id = "Tes";
			
			entity.Qty = 67;
			
			return entity;
		}
		protected Inventory GetFirstInventory()
        {
            IList<Inventory> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Inventory entity = CreateNewInventory();
				
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
                Inventory entityA = CreateNewInventory();
				manager.Save(entityA);

                Inventory entityB = manager.GetById(entityA.Id);

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
                Inventory entityA = GetFirstInventory();
				
				entityA.Qty = 42;
				
				manager.Update(entityA);

                Inventory entityB = manager.GetById(entityA.Id);

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
                Inventory entity = GetFirstInventory();
				
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

