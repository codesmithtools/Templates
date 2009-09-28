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
        protected ISupplierManager manager;
		
		public SupplierTests()
        {
            manager = managerFactory.GetSupplierManager();
        }
		
		protected Supplier CreateNewSupplier()
		{
			Supplier entity = new Supplier();
			
			// You may need to maually enter this key if there is a constraint violation.
			entity.Id = 73;
			
			entity.Name = "Test Tes";
			entity.Status = "T";
			entity.Addr1 = "Test Test Test Test Test Test Test";
			entity.Addr2 = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Tes";
			entity.City = "Test";
			entity.State = "Tes";
			entity.Zip = "Tes";
			entity.Phone = "Test Te";
			
			return entity;
		}
		protected Supplier GetFirstSupplier()
        {
            IList<Supplier> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Supplier entity = CreateNewSupplier();
				
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
                Supplier entityA = CreateNewSupplier();
				manager.Save(entityA);

                Supplier entityB = manager.GetById(entityA.Id);

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
                Supplier entityA = GetFirstSupplier();
				
				entityA.Name = "Test Test Test Test Test T";
				
				manager.Update(entityA);

                Supplier entityB = manager.GetById(entityA.Id);

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
                Supplier entity = GetFirstSupplier();
				
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

