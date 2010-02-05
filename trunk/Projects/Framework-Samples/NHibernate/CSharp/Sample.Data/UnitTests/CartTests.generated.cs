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
    public partial class CartTests : UNuitTestBase
    {
        [SetUp]
        public void SetUp()
        {
            manager = managerFactory.GetCartManager();
            manager.Session.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            manager.Session.RollbackTransaction();
            manager.Dispose();
        }
        
        protected Sample.Data.Generated.ManagerObjects.ICartManager manager;
		
		protected Sample.Data.Generated.BusinessObjects.Cart CreateNewCart()
		{
			Sample.Data.Generated.BusinessObjects.Cart entity = new Sample.Data.Generated.BusinessObjects.Cart();
			
			
			entity.ItemId = "Test ";
			entity.Name = "Test Test Test Test Te";
			entity.Type = "Test Test Test Tes";
			entity.Price = 57;
			entity.CategoryId = "Te";
			entity.ProductId = "Te";
			entity.IsShoppingCart = true;
			entity.Quantity = 61;
			
			using(Sample.Data.Generated.ManagerObjects.IProfileManager profileManager = managerFactory.GetProfileManager())
			    entity.Profile = profileManager.GetAll(1)[0];
			
			return entity;
		}
		protected Sample.Data.Generated.BusinessObjects.Cart GetFirstCart()
        {
            IList<Sample.Data.Generated.BusinessObjects.Cart> entityList = manager.GetAll(1);
            if (entityList.Count == 0)
                Assert.Fail("All tables must have at least one row for unit tests to succeed.");
            return entityList[0];
        }
		
		[Test]
        public void Create()
        {
            try
            {
				Sample.Data.Generated.BusinessObjects.Cart entity = CreateNewCart();
				
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
                Sample.Data.Generated.BusinessObjects.Cart entityA = CreateNewCart();
				manager.Save(entityA);

                Sample.Data.Generated.BusinessObjects.Cart entityB = manager.GetById(entityA.Id);

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
                Sample.Data.Generated.BusinessObjects.Cart entityA = GetFirstCart();
				
				entityA.ItemId = "Te";
				
				manager.Update(entityA);

                Sample.Data.Generated.BusinessObjects.Cart entityB = manager.GetById(entityA.Id);

                Assert.AreEqual(entityA.ItemId, entityB.ItemId);
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
                Sample.Data.Generated.BusinessObjects.Cart entity = GetFirstCart();
				
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

