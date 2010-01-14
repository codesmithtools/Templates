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
    public partial class ItemTests : UNuitTestBase
    {
        protected IItemManager manager;
		
		public ItemTests()
        {
            manager = managerFactory.GetItemManager();
        }
		
		protected Item CreateNewItem()
		{
			Item entity = new Item();
			
			// You may need to maually enter this key if there is a constraint violation.
			entity.Id = "Te";
			
			entity.ListPrice = 25;
			entity.UnitCost = 34;
			entity.Status = "T";
			entity.Name = "Test Test Test Test Test Test Test Test Test Test Test ";
			entity.Image = "Test Test Test Test Test Tes";
			
			IProductManager productManager = managerFactory.GetProductManager();
			entity.Product = productManager.GetAll(1)[0];
			
			ISupplierManager supplierManager = managerFactory.GetSupplierManager();
			entity.Supplier = supplierManager.GetAll(1)[0];
			
			return entity;
		}
		protected Item GetFirstItem()
        {
            IList<Item> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Item entity = CreateNewItem();
				
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
                Item entityA = CreateNewItem();
				manager.Save(entityA);

                Item entityB = manager.GetById(entityA.Id);

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
                Item entityA = GetFirstItem();
				
				entityA.ListPrice = 77;
				
				manager.Update(entityA);

                Item entityB = manager.GetById(entityA.Id);

                Assert.AreEqual(entityA.ListPrice, entityB.ListPrice);
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
                Item entity = GetFirstItem();
				
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

