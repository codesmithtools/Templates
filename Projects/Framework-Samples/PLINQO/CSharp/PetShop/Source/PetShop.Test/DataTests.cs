using System;
using System.Diagnostics;
using NUnit.Framework;
using PetShop.Core.Data;
using System.Linq;
using System.Data.Linq;
using PetShop.Core.Utility;

namespace PetShop.Test
{
    [TestFixture]
    public class BusinessTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        private const string NAME = "UnitTests";
        private const string ID = "Unit-Test";
        private static int _supplierId = 1;

        #region Profile

        [Test]
        public void CreateProfile()
        {
            Stopwatch watch = Stopwatch.StartNew();

            var profile = new Profile();
            profile.Username = NAME;
            profile.ApplicationName = "PetShop..Businesss";
            profile.IsAnonymous = false;
            profile.LastActivityDate = DateTime.Now;
            profile.LastUpdatedDate = DateTime.Now;

            try
            {
                using (var context = new PetShopDataContext())
                {
                    context.Profile.InsertOnSubmit(profile);
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            
            Assert.IsTrue(true);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void FetchProfile()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Profile profile = null;
            using (var context = new PetShopDataContext())
            {
                profile = context.Profile.GetProfile(NAME);
                profile.Detach();
            }

            Assert.IsTrue(profile.Username == NAME);
            
            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void UpdateProfile()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Profile profile = null;
            using (var context = new PetShopDataContext())
            {
                profile = context.Profile.GetProfile(NAME);
                profile.Detach();
            }

            using (var context = new PetShopDataContext())
            {
                context.Profile.Attach(profile);
                profile.IsAnonymous = true;
                context.SubmitChanges();
            }

            using (var context = new PetShopDataContext())
            {
                Assert.IsTrue(context.Profile.GetProfile(NAME).IsAnonymous.Value);
            }
            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region Category

        [Test]
        public void CreateCategory()
        {
            Stopwatch watch = Stopwatch.StartNew();

            var category = new Category();
            category.CategoryId = ID;
            category.Name = "";
            category.Descn = "";

            try
            {
                using (var context = new PetShopDataContext())
                {
                    context.Category.InsertOnSubmit(category);
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(true);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void FetchCategory()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Category category = null;
            using (var context = new PetShopDataContext())
            {
                category = context.Category.ByKey(ID);
                category.Detach();
            }
            Assert.IsTrue(category.CategoryId == ID);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void UpdateCategory()
        {
            Stopwatch watch = Stopwatch.StartNew();
            
            Category category = null;
            using (var context = new PetShopDataContext())
            {
                category = context.Category.ByKey(ID);
                category.Detach();
            }

            using (var context = new PetShopDataContext())
            {
                context.Category.Attach(category);
                category.Descn = "This is a .";
                context.SubmitChanges();
            }

            using (var context = new PetShopDataContext())
            {
                Assert.IsTrue(context.Category.ByKey(ID).Descn == "This is a .");
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region Inventory

        [Test]
        public void CreateInventory()
        {
            Stopwatch watch = Stopwatch.StartNew();

            var inventory = new Inventory();
            inventory.ItemId = ID;
            inventory.Qty = 10;

            try
            {
                using (var context = new PetShopDataContext())
                {
                    context.Inventory.InsertOnSubmit(inventory);
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(true);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void FetchInventory()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Inventory inventory = null;

            using (var context = new PetShopDataContext())
            {
                inventory = context.Inventory.ByKey(ID);
                inventory.Detach();
            }

            Assert.IsTrue(inventory.ItemId == ID);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void UpdateInventory()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Inventory inventory = null;
            using (var context = new PetShopDataContext())
            {
                inventory = context.Inventory.ByKey(ID);
                inventory.Detach();
            }

            using (var context = new PetShopDataContext())
            {
                context.Inventory.Attach(inventory);
                inventory.Qty = 100;
                context.SubmitChanges();
            }

            using (var context = new PetShopDataContext())
            {
                Assert.IsTrue(context.Inventory.ByKey(ID).Qty == 100);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region Product

        [Test]
        public void CreateProduct()
        {
            Stopwatch watch = Stopwatch.StartNew();

            var product = new Product();
            product.ProductId = ID;
            product.CategoryId = ID;
            product.Image = "/.png";
            product.Descn = "";
            product.Name = "";

            try
            {
                using (var context = new PetShopDataContext())
                {
                    context.Product.InsertOnSubmit(product);
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(true);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void FetchProduct()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Product product = null;
            using (var context = new PetShopDataContext())
            {
                product = context.Product.ByKey(ID);
                product.Detach();
            }

            Assert.IsTrue(product.ProductId == ID);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void UpdateProduct()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Product product = null;
            using (var context = new PetShopDataContext())
            {
                product = context.Product.ByKey(ID);
                product.Detach();
            }

            using (var context = new PetShopDataContext())
            {
                context.Product.Attach(product);
                product.Descn = "This is a ";
                context.SubmitChanges();
            }

            using (var context = new PetShopDataContext())
            {
                Assert.IsTrue(context.Product.ByKey(ID).Descn == "This is a ");
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region Supplier

        [Test]
        public void CreateSupplier()
        {
            Stopwatch watch = Stopwatch.StartNew();

            var supplier = new Supplier();
            supplier.Name = NAME;
            supplier.Status = "AB";
            supplier.Addr1 = "One  Way";
            supplier.Addr2 = "Two  Way";
            supplier.City = "Dallas";
            supplier.State = "TX";
            supplier.Zip = "90210";
            supplier.Phone = "555-555-5555";

            try
            {
                using (var context = new PetShopDataContext())
                {
                    context.Supplier.InsertOnSubmit(supplier);
                    context.SubmitChanges();
                    supplier.Detach();
                    _supplierId = supplier.SuppId;
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(true);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void FetchSupplier()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Supplier supplier = null;
            using (var context = new PetShopDataContext())
            {
                supplier = context.Supplier.ByKey(_supplierId);
                supplier.Detach();
            }

            Assert.IsTrue(supplier.SuppId == _supplierId);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void UpdateSupplier()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Supplier supplier = null;
            using (var context = new PetShopDataContext())
            {
                supplier = context.Supplier.ByKey(_supplierId);
                supplier.Detach();
            }

            using (var context = new PetShopDataContext())
            {
                context.Supplier.Attach(supplier);
                supplier.Phone = "111-111-1111";
                context.SubmitChanges();
            }

            using (var context = new PetShopDataContext())
            {
                Assert.IsTrue(context.Supplier.ByKey(_supplierId).Phone == "111-111-1111");
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region Item

        [Test]
        public void CreateItem()
        {
            Stopwatch watch = Stopwatch.StartNew();

            var item = new Item();
            item.ItemId = ID;
            item.Image = "/.png";
            item.ListPrice = 0;
            item.Name = "";
            item.ProductId = ID;
            item.Status = "";
            item.Supplier1.SuppId = _supplierId;
            item.UnitCost = 0;

            try
            {
                using (var context = new PetShopDataContext())
                {
                    context.Item.InsertOnSubmit(item);
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(true);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void FetchItem()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Item item = null;
            using (var context = new PetShopDataContext())
            {
                item = context.Item.ByKey(ID);
                item.Detach();
            }

            Assert.IsTrue(item.ItemId == ID);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void UpdateItem()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Item item = null;
            using (var context = new PetShopDataContext())
            {
                item = context.Item.ByKey(ID);
                item.Detach();
            }

            using (var context = new PetShopDataContext())
            {
                context.Item.Attach(item);
                item.ListPrice = 111;
                context.SubmitChanges();
            }

            using (var context = new PetShopDataContext())
            {
                Assert.IsTrue(context.Item.ByKey(ID).ListPrice == 111);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region ShoppingCart

        [Test]
        public void CreateAndFetchShoppingCart()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Profile profile = null;
            using (var context = new PetShopDataContext())
            {
                profile = context.Profile.GetProfile(NAME);
                profile.Detach();
            }

            Assert.IsTrue(profile.ShoppingCart.Count == 0);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void AddItemToShoppingCart()
        {
            Stopwatch watch = Stopwatch.StartNew();
            
            Profile profile = null;
            using (var context = new PetShopDataContext())
            {
                profile = context.Profile.GetProfile(NAME);
                profile.Detach();
            }

            using (var context = new PetShopDataContext())
            {
                CartHelper.Add(profile.ShoppingCart, ID, profile.UniqueID, true);
            }

            using (var context = new PetShopDataContext())
            {
                Assert.IsTrue(context.Profile.GetProfile(NAME).ShoppingCart.Count == 1);
            }
            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void RemoveItemFromShoppingCart()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Profile profile = null;
            using (var context = new PetShopDataContext())
            {
                profile = context.Profile.GetProfile(NAME);
                profile.Detach();
            }

            using (var context = new PetShopDataContext())
            {
                CartHelper.Remove(profile.ShoppingCart, ID);
            }

            using (var context = new PetShopDataContext())
            {
                Assert.IsTrue(context.Profile.GetProfile(NAME).ShoppingCart.Count == 0);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void UpdateItemQuantityShoppingCart()
        {
            Stopwatch watch = Stopwatch.StartNew();

            //Add new Item to the cart.
            Profile profile = null;
            using (var context = new PetShopDataContext())
            {
                profile = context.Profile.GetProfile(NAME);
                profile.Detach();
            }

            CartHelper.Add(profile.ShoppingCart, ID, profile.UniqueID, true);
            CartHelper.Add(profile.ShoppingCart, ID, profile.UniqueID, true);


            using (var context = new PetShopDataContext())
            {
                profile = context.Profile.GetProfile(NAME);
                profile.Detach();
            }

            Assert.IsTrue(profile.ShoppingCart.Count == 1 && profile.ShoppingCart[0].Quantity == 2);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        [Test]
        public void ClearShoppingCart()
        {
            Stopwatch watch = Stopwatch.StartNew();

            //Add new Item to the cart.
            Profile profile = null;
            using (var context = new PetShopDataContext())
            {
                profile = context.Profile.GetProfile(NAME);
                profile.Detach();
            }

            CartHelper.Add(profile.ShoppingCart, ID, profile.UniqueID, true);
            

            //Clear the cart.
            using (var context = new PetShopDataContext())
            {
                profile = context.Profile.GetProfile(NAME);
                profile.Detach();
            }
            
            CartHelper.ClearCart(profile.ShoppingCart);

            using (var context = new PetShopDataContext())
            {
                Assert.IsTrue(context.Profile.GetProfile(NAME).ShoppingCart.Count == 0);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region Deletes

        #region Delete Item

        [Test]
        public void DeleteItem()
        {
            Stopwatch watch = Stopwatch.StartNew();

            using (var context = new PetShopDataContext())
            {
                context.Item.Delete(ID);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region Delete Supplier

        [Test]
        public void DeleteSupplier()
        {
            Stopwatch watch = Stopwatch.StartNew();

            using (var context = new PetShopDataContext())
            {
                context.Supplier.Delete(_supplierId);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region Delete Product

        [Test]
        public void DeleteProduct()
        {
            Stopwatch watch = Stopwatch.StartNew();

            using (var context = new PetShopDataContext())
            {
                context.Product.Delete(ID);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region Delete Inventory

        [Test]
        public void DeleteInventory()
        {
            Stopwatch watch = Stopwatch.StartNew();

            using (var context = new PetShopDataContext())
            {
                context.Inventory.Delete(ID);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region Delete Category

        [Test]
        public void DeleteCategory()
        {
            Stopwatch watch = Stopwatch.StartNew();

            using (var context = new PetShopDataContext())
            {
                context.Category.Delete(ID);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #region Delete Profile

        [Test]
        public void DeleteProfile()
        {
            Stopwatch watch = Stopwatch.StartNew();

            using (var context = new PetShopDataContext())
            {
                var profile = context.Profile.GetProfile(NAME);
                context.Profile.Delete(profile.UniqueID);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        #endregion

        #endregion
    }
}
