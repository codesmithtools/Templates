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
    public partial class ProductTests : UNuitTestBase
    {
        protected IProductManager manager;
		
		public ProductTests()
        {
            manager = managerFactory.GetProductManager();
        }
		
		protected Product CreateNewProduct()
		{
			Product entity = new Product();
			
			// You may need to maually enter this key if there is a constraint violation.
			entity.Id = "Te";
			
			entity.Name = "Test Test Test T";
			entity.Descn = "Test Test ";
			entity.Image = "Test Test Test Test Test Test Test Test Test T";
			
			ICategoryManager categoryManager = managerFactory.GetCategoryManager();
			entity.Category = categoryManager.GetAll(1)[0];
			
			return entity;
		}
		protected Product GetFirstProduct()
        {
            IList<Product> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Product entity = CreateNewProduct();
				
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
                Product entityA = CreateNewProduct();
				manager.Save(entityA);

                Product entityB = manager.GetById(entityA.Id);

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
                Product entityA = GetFirstProduct();
				
				entityA.Name = "Test Test Test Test Test";
				
				manager.Update(entityA);

                Product entityB = manager.GetById(entityA.Id);

                Assert.AreEqual(entityA.Name, entityB.Name);
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
                Product entity = GetFirstProduct();
				
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

