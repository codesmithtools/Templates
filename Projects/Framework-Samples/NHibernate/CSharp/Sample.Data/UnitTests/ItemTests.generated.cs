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
        protected Sample.Data.Generated.ManagerObjects.IItemManager manager;
		
		public ItemTests()
        {
            manager = managerFactory.GetItemManager();
        }
		
		protected Sample.Data.Generated.BusinessObjects.Item CreateNewItem()
		{
			Sample.Data.Generated.BusinessObjects.Item entity = new Sample.Data.Generated.BusinessObjects.Item();
			
			// You may need to maually enter this key if there is a constraint violation.
			entity.Id = "Test Tes";
			
			entity.ListPrice = 61;
			entity.UnitCost = 1;
			entity.Status = "T";
			entity.Name = "Test Te";
			entity.Image = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Te";
			
			Sample.Data.Generated.ManagerObjects.IProductManager productManager = managerFactory.GetProductManager();
			entity.Product = productManager.GetAll(1)[0];
			
			Sample.Data.Generated.ManagerObjects.ISupplierManager supplierManager = managerFactory.GetSupplierManager();
			entity.Supplier = supplierManager.GetAll(1)[0];
			
			return entity;
		}
		protected Sample.Data.Generated.BusinessObjects.Item GetFirstItem()
        {
            IList<Sample.Data.Generated.BusinessObjects.Item> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Sample.Data.Generated.BusinessObjects.Item entity = CreateNewItem();
				
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
                Sample.Data.Generated.BusinessObjects.Item entityA = CreateNewItem();
				manager.Save(entityA);

                Sample.Data.Generated.BusinessObjects.Item entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Item entityA = GetFirstItem();
				
				entityA.ListPrice = 6;
				
				manager.Update(entityA);

                Sample.Data.Generated.BusinessObjects.Item entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Item entity = GetFirstItem();
				
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

