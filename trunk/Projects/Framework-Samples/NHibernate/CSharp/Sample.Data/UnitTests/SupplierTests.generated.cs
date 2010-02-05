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
    public partial class SupplierTests : UNuitTestBase
    {
        [SetUp]
        public void SetUp()
        {
            manager = managerFactory.GetSupplierManager();
            manager.Session.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            manager.Session.RollbackTransaction();
            manager.Dispose();
        }
        
        protected Sample.Data.Generated.ManagerObjects.ISupplierManager manager;
		
		protected Sample.Data.Generated.BusinessObjects.Supplier CreateNewSupplier()
		{
			Sample.Data.Generated.BusinessObjects.Supplier entity = new Sample.Data.Generated.BusinessObjects.Supplier();
			
			// You may need to maually enter this key if there is a constraint violation.
			entity.Id = 43;
			
			entity.Name = "Test Test Test Test Test Test Test Test Test";
			entity.Status = "T";
			entity.Addr1 = "Test Test Test Te";
			entity.Addr2 = "Test Test Test Test Test Test Test Test Test Test Test Test";
			entity.City = "Test Test Test ";
			entity.State = "Test Test Test Test Test Test Test Test Test Test Test Test T";
			entity.Zip = "Tes";
			entity.Phone = "Test Test Test Test Test Te";
			
			return entity;
		}
		protected Sample.Data.Generated.BusinessObjects.Supplier GetFirstSupplier()
        {
            IList<Sample.Data.Generated.BusinessObjects.Supplier> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Sample.Data.Generated.BusinessObjects.Supplier entity = CreateNewSupplier();
				
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
                Sample.Data.Generated.BusinessObjects.Supplier entityA = CreateNewSupplier();
				manager.Save(entityA);

                Sample.Data.Generated.BusinessObjects.Supplier entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Supplier entityA = GetFirstSupplier();
				
				entityA.Name = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test ";
				
				manager.Update(entityA);

                Sample.Data.Generated.BusinessObjects.Supplier entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Supplier entity = GetFirstSupplier();
				
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

