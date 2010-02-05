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
    public partial class CategoryTests : UNuitTestBase
    {
        [SetUp]
        public void SetUp()
        {
            manager = managerFactory.GetCategoryManager();
            manager.Session.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            manager.Session.RollbackTransaction();
            manager.Dispose();
        }
        
        protected Sample.Data.Generated.ManagerObjects.ICategoryManager manager;
		
		protected Sample.Data.Generated.BusinessObjects.Category CreateNewCategory()
		{
			Sample.Data.Generated.BusinessObjects.Category entity = new Sample.Data.Generated.BusinessObjects.Category();
			
			// You may need to maually enter this key if there is a constraint violation.
			entity.Id = "Te";
			
			entity.Name = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Te";
			entity.Descn = "Test Test ";
			
			return entity;
		}
		protected Sample.Data.Generated.BusinessObjects.Category GetFirstCategory()
        {
            IList<Sample.Data.Generated.BusinessObjects.Category> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Sample.Data.Generated.BusinessObjects.Category entity = CreateNewCategory();
				
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
                Sample.Data.Generated.BusinessObjects.Category entityA = CreateNewCategory();
				manager.Save(entityA);

                Sample.Data.Generated.BusinessObjects.Category entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Category entityA = GetFirstCategory();
				
				entityA.Name = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Tes";
				
				manager.Update(entityA);

                Sample.Data.Generated.BusinessObjects.Category entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Category entity = GetFirstCategory();
				
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

