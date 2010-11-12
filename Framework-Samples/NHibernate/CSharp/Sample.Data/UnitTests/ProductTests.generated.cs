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
        [SetUp]
        public void SetUp()
        {
            manager = managerFactory.GetProductManager();
            manager.Session.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            manager.Session.RollbackTransaction();
            manager.Dispose();
        }
        
        protected Sample.Data.Generated.ManagerObjects.IProductManager manager;
		
		protected Sample.Data.Generated.BusinessObjects.Product CreateNewProduct()
		{
			Sample.Data.Generated.BusinessObjects.Product entity = new Sample.Data.Generated.BusinessObjects.Product();
			
			// You may need to maually enter this key if there is a constraint violation.
			entity.Id = "T";
			
			entity.Name = "Test Test Test Test Test Test T";
			entity.Descn = "Test Test ";
			entity.Image = "Test Test Test Test Test Test";
			
			using(Sample.Data.Generated.ManagerObjects.ICategoryManager categoryManager = managerFactory.GetCategoryManager())
			    entity.Category = categoryManager.GetAll(1)[0];
			
			return entity;
		}
		protected Sample.Data.Generated.BusinessObjects.Product GetFirstProduct()
        {
            IList<Sample.Data.Generated.BusinessObjects.Product> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Sample.Data.Generated.BusinessObjects.Product entity = CreateNewProduct();
				
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
                Sample.Data.Generated.BusinessObjects.Product entityA = CreateNewProduct();
				manager.Save(entityA);

                Sample.Data.Generated.BusinessObjects.Product entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Product entityA = GetFirstProduct();
				
				entityA.Name = "Test Test Test Test Test Test Test Test Te";
				
				manager.Update(entityA);

                Sample.Data.Generated.BusinessObjects.Product entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Product entity = GetFirstProduct();
				
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

