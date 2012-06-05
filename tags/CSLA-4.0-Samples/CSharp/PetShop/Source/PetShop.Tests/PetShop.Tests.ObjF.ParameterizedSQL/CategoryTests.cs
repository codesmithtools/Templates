using System;
using System.Diagnostics;
using NUnit.Framework;

namespace PetShop.Tests.ObjF.ParameterizedSQL
{
    [TestFixture]
    public class CategoryTests
    {
        #region Setup

        private string TestCategoryID { get; set; }
        private string TestCategoryID2 { get; set; }
        private string TestProductID { get; set; }
        private string TestProductID2 { get; set; }

        [NUnit.Framework.TestFixtureSetUp]
        public void Init()
        {
            System.Console.WriteLine(new String('-', 75));
            System.Console.WriteLine("-- Testing the Category Entity --");
            System.Console.WriteLine(new String('-', 75));
        }

        [SetUp]
        public void Setup()
        {
            TestCategoryID = TestUtility.Instance.RandomString(10, false);
            TestCategoryID2 = TestUtility.Instance.RandomString(10, false);
            TestProductID = TestUtility.Instance.RandomString(10, false);
            TestProductID2 = TestUtility.Instance.RandomString(10, false);

            CreateCategory(TestCategoryID);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                Product.DeleteProduct(TestProductID2);
            }
            catch (Exception) { }
            try
            {
                Product.DeleteProduct(TestProductID);
            }
            catch (Exception) { }

            try
            {
                Category.DeleteCategory(TestCategoryID);
            }
            catch (Exception) { }

            try
            {
                Category.DeleteCategory(TestCategoryID2);
            }
            catch (Exception) { }
        }

        [NUnit.Framework.TestFixtureTearDown]
        public void Complete()
        {
            System.Console.WriteLine("All Tests Completed");
            System.Console.WriteLine();
        }

        [Test]
        private void CreateCategory(string categoryID)
        {
            Category category = Category.NewCategory();
            category.CategoryId = categoryID;
            category.Name = TestUtility.Instance.RandomString(80, false);
            category.Description = TestUtility.Instance.RandomString(255, false);

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Assert.IsTrue(category.CategoryId == categoryID);
        }

        #endregion

		/// <summary>
		/// Inserts a Category entity into the database.
		/// </summary>
		[Test]
		public void Step_01_Insert_Duplicate()
		{
            Console.WriteLine("1. Testing Duplicate Records.");
            Stopwatch watch = Stopwatch.StartNew();

            //Insert should fail as there should be a duplicate key.
            Category category = Category.NewCategory();
            category.CategoryId = TestCategoryID;
            category.Name = TestUtility.Instance.RandomString(80, false);
            category.Description = TestUtility.Instance.RandomString(255, false);

		    try
		    {
                Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
                category = category.Save();
                
                // Fail as a duplicate record was entered.
                Assert.Fail("Fail as a duplicate record was entered and an exception was not thrown.");
		    }
		    catch (Exception)
		    {
                Assert.IsTrue(true);
            }
            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
		}

        /// <summary>
        /// Inserts a Category entity into the database.
        /// </summary>
        [Test]
        public void Step_02_Insert()
        {
            Console.WriteLine("2. Inserting new category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.NewCategory();
            category.CategoryId = TestCategoryID2;
            category.Name = TestUtility.Instance.RandomString(80, false);
            category.Description = TestUtility.Instance.RandomString(255, false);

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Assert.IsTrue(true);
            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
		/// Selects a sample of Category objects of the database.
		/// </summary>
		[Test]
		public void Step_03_SelectAll()
		{
            Console.WriteLine("3. Selecting all categories by calling GetByCategoryId(\"{0}\").", TestCategoryID);
            Stopwatch watch = Stopwatch.StartNew();

            CategoryList list = CategoryList.GetByCategoryId(TestCategoryID);
            Assert.IsTrue(list.Count == 1);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
		}

        /// <summary>
        /// Updates a Category entity into the database.
        /// </summary>
        [Test]
        public void Step_04_Update()
        {
            Console.WriteLine("4. Updating the category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            var name = category.Name;
            var desc = category.Description;

            category.Name = TestUtility.Instance.RandomString(80, false);
            category.Description = TestUtility.Instance.RandomString(255, false);

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Assert.IsFalse(string.Equals(category.Name, name, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsFalse(string.Equals(category.Description, desc, StringComparison.InvariantCultureIgnoreCase));

            category.Name = name;
            category.Description = desc;

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Assert.IsTrue(string.Equals(category.Name, name, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(string.Equals(category.Description, desc, StringComparison.InvariantCultureIgnoreCase));

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Delete the  Category entity into the database.
        /// </summary>
        [Test]
        public void Step_05_Delete()
        {
            Console.WriteLine("5. Deleting the category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            category.Delete();

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Assert.IsTrue(category.IsNew);
            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Updates a non Identity PK column.
        /// </summary>
        [Test]
        public void Step_06_UpdatePrimaryKey()
        {
            Console.WriteLine("6. Updating the non Identity Primary Key.");
            Stopwatch watch = Stopwatch.StartNew();

            Console.WriteLine("\tGetting category \"{0}\"", TestCategoryID);
            Category category = Category.GetByCategoryId(TestCategoryID);
            category.CategoryId = TestCategoryID2;

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();
            Console.WriteLine("\tSet categoryID to \"{0}\"", TestCategoryID2);
            Assert.IsTrue(category.CategoryId == TestCategoryID2);

            try
            {
                Category.GetByCategoryId(TestCategoryID);
                Assert.Fail("Record exists when it should have been updated.");
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
            
            Category validCategory = Category.GetByCategoryId(TestCategoryID2);
            Assert.IsTrue(validCategory.CategoryId == TestCategoryID2);
            Console.WriteLine("\tPrimaryKey has been updated.");

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Tests the rules.
        /// </summary>
        [Test]
        public void Step_07_Rules()
        {
            Console.WriteLine("7. Testing the state of the rules for the entity.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.NewCategory();
            Assert.IsFalse(category.IsValid);

            category.CategoryId = TestCategoryID;
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());

            category.Name = TestUtility.Instance.RandomString(80, false);
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());

            category.Description = TestUtility.Instance.RandomString(255, false);
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());

            // Check Category.
            category.CategoryId = null;
            Assert.IsFalse(category.IsValid);

            category.CategoryId = TestUtility.Instance.RandomString(11, false);
            Assert.IsFalse(category.IsValid);

            category.CategoryId = TestCategoryID;
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());

            // Check Name.
            category.Name = null;
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());

            category.Name = TestUtility.Instance.RandomString(81, false);
            Assert.IsFalse(category.IsValid);

            category.Name = TestUtility.Instance.RandomString(80, false);
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());

            // Check Description.
            category.Description = null;
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());

            category.Description = TestUtility.Instance.RandomString(256, false);
            Assert.IsFalse(category.IsValid);

            category.Description = TestUtility.Instance.RandomString(80, false);
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Inserts a Category entity into the database.
        /// </summary>
        [Test]
        public void Step_08_Entity_State()
        {
            Console.WriteLine("8. Checking the state of the entity.");
            Stopwatch watch = Stopwatch.StartNew();

            //Insert should fail as there should be a duplicate key.
            Category category = Category.NewCategory();
            Assert.IsTrue(category.IsDirty);

            category.CategoryId = TestCategoryID2;
            category.Name = TestUtility.Instance.RandomString(80, false);
            category.Description = TestUtility.Instance.RandomString(255, false);
            
            Assert.IsTrue(category.IsNew);
            Assert.IsTrue(category.IsDirty);
            Assert.IsFalse(category.IsDeleted);

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Assert.IsFalse(category.IsNew);
            Assert.IsFalse(category.IsDirty);
            Assert.IsFalse(category.IsDeleted);

            category.Name = TestUtility.Instance.RandomString(80, false);
            Assert.IsTrue(category.IsDirty);
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Assert.IsFalse(category.IsNew);
            Assert.IsFalse(category.IsDirty);
            Assert.IsFalse(category.IsDeleted);

            category.Delete();
            Assert.IsTrue(category.IsDeleted);
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();
            Assert.IsFalse(category.IsDeleted);
            Assert.IsTrue(category.IsNew);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Testing Child collections on a new entity.
        /// </summary>
        [Test]
        public void Step_09_New_Entity_Collection()
        {
            Console.WriteLine("9. Testing child collections on a new category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.NewCategory();
            var count = category.Products.Count;

            Assert.IsTrue(count == 0);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Testing Child collections on a new entity.
        /// </summary>
        [Test]
        public void Step_10_Existing_Entity_With_No_Collection()
        {
            Console.WriteLine("10. Testing child collections on a category with no child collection items.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            var count = category.Products.Count;

            Assert.IsTrue(count == 0);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Selects a sample of Category objects of the database using an invalid category ID.
        /// </summary>
        [Test]
        public void Step_12_SelectAll_Invalid_Value()
        {
            Console.WriteLine("12. Selecting all categories by calling GetByCategoryId with an invalid value");
            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                CategoryList.GetByCategoryId(TestUtility.Instance.RandomString(10, false));

                Assert.Fail("This call to GetByCategoryID should throw an exception");
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Selects a sample of Category objects of the database using an invalid category ID.
        /// </summary>
        [Test]
        public void Step_13_SelectAll_With_Oversized_CategoryID()
        {
            Console.WriteLine("13. Selecting all categories by calling GetByCategoryId with an invalid value");
            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                CategoryList.GetByCategoryId(TestCategoryID + "a");

                Assert.Fail("This call to GetByCategoryID should throw an exception");
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Testing Insert Child collections on a new entity.
        /// </summary>
        [Test]
        public void Step_14_Insert_Child_Collection_On_New_Category()
        {
            Console.WriteLine("14. Testing insert on child collections in a new category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.NewCategory();
            category.CategoryId = TestCategoryID2;
            category.Name = TestUtility.Instance.RandomString(80, false);
            category.Description = TestUtility.Instance.RandomString(255, false);

            Assert.IsTrue(category.Products.Count == 0);
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());

            Product product = category.Products.AddNew();

            Assert.IsTrue(category.Products.Count == 1);

            product.ProductId = TestProductID;
            product.Name = TestUtility.Instance.RandomString(80, false);
            product.Description = TestUtility.Instance.RandomString(255, false);
            product.Image = TestUtility.Instance.RandomString(80, false);
            Assert.IsTrue(product.IsValid, product.BrokenRulesCollection.ToString());

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Category category2 = Category.GetByCategoryId(TestCategoryID2);
            
            Assert.IsTrue(category.Products.Count == category2.Products.Count);
            Assert.IsTrue(category.CategoryId == category2.Products[0].CategoryId);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Testing Insert Child collections on an existing entity.
        /// </summary>
        [Test]
        public void Step_15_Insert_Child_Collection()
        {
            Console.WriteLine("15. Testing insert on child collections in a category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            Assert.IsTrue(category.Products.Count == 0);

            Product product = category.Products.AddNew();

            Assert.IsTrue(category.Products.Count == 1);

            product.ProductId = TestProductID;
            product.Name = TestUtility.Instance.RandomString(80, false);
            product.Description = TestUtility.Instance.RandomString(255, false);
            product.Image = TestUtility.Instance.RandomString(80, false);

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Category category2 = Category.GetByCategoryId(TestCategoryID);

            Assert.IsTrue(category.Products.Count == category2.Products.Count);
            Assert.IsTrue(category.CategoryId == category2.Products[0].CategoryId);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Testing update on a new child collections on a new entity.
        /// </summary>
        [Test]
        public void Step_16_Update_Child_Collection_On_New_Category()
        {
            Console.WriteLine("16. Testing update on new child collections in a new category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.NewCategory();
            category.CategoryId = TestCategoryID2;
            category.Name = TestUtility.Instance.RandomString(80, false);
            category.Description = TestUtility.Instance.RandomString(255, false);

            Assert.IsTrue(category.Products.Count == 0);
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());

            Product product = category.Products.AddNew();

            Assert.IsTrue(category.Products.Count == 1);

            product.ProductId = TestProductID;
            product.Name = TestUtility.Instance.RandomString(80, false);
            product.Description = TestUtility.Instance.RandomString(255, false);
            product.Image = TestUtility.Instance.RandomString(80, false);

            var newName = TestUtility.Instance.RandomString(80, false);
            foreach (Product item in category.Products)
            {
                item.Name = newName;
            }

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Category category2 = Category.GetByCategoryId(TestCategoryID2);
            Assert.IsTrue(string.Equals(category2.Products[0].Name, newName, StringComparison.InvariantCultureIgnoreCase));

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Testing update Child collections on an existing entity.
        /// </summary>
        [Test]
        public void Step_17_Update_Child_Collection()
        {
            Console.WriteLine("17. Testing update on child collections in a category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            Assert.IsTrue(category.Products.Count == 0);

            Product product = category.Products.AddNew();

            Assert.IsTrue(category.Products.Count == 1);

            product.ProductId = TestProductID;
            product.Name = TestUtility.Instance.RandomString(80, false);
            product.Description = TestUtility.Instance.RandomString(255, false);
            product.Image = TestUtility.Instance.RandomString(80, false);

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            var newName = TestUtility.Instance.RandomString(80, false);
            foreach (Product item in category.Products)
            {
                item.Name = newName;
            }

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Category category2 = Category.GetByCategoryId(TestCategoryID);
            ProductList list = category2.Products;

            Assert.IsTrue(string.Equals(category2.Products[0].Name, newName, StringComparison.InvariantCultureIgnoreCase));

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Testing update Child collections on an existing entity.
        /// </summary>
        [Test]
        public void Step_18_Update_PK_Of_Child_Collection()
        {
            Console.WriteLine("18. Testing update of PK on child collections in a category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            Assert.IsTrue(category.Products.Count == 0);

            Product product = category.Products.AddNew();

            Assert.IsTrue(category.Products.Count == 1);

            product.ProductId = TestProductID;
            product.Name = TestUtility.Instance.RandomString(80, false);
            product.Description = TestUtility.Instance.RandomString(255, false);
            product.Image = TestUtility.Instance.RandomString(80, false);

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            category.CategoryId = TestCategoryID2;

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Category category2 = Category.GetByCategoryId(TestCategoryID2);
            Assert.IsTrue(category2.Products.Count == 1); 
            Assert.IsTrue(string.Equals(category2.Products[0].CategoryId, category.CategoryId, StringComparison.InvariantCultureIgnoreCase));

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Testing update of Every PK in a Child collections on an existing entity.
        /// </summary>
        [Test]
        public void Step_19_Update_Every_PK_In_Child_Collection()
        {
            Console.WriteLine("19. Testing update of every PK in a child collections in a category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            Assert.IsTrue(category.Products.Count == 0);

            Product product = category.Products.AddNew();

            Assert.IsTrue(category.Products.Count == 1);

            product.ProductId = TestProductID;
            product.Name = TestUtility.Instance.RandomString(80, false);
            product.Description = TestUtility.Instance.RandomString(255, false);
            product.Image = TestUtility.Instance.RandomString(80, false);

            Product product2 = category.Products.AddNew();

            Assert.IsTrue(category.Products.Count == 2);

            product2.ProductId = TestProductID2;
            product2.Name = TestUtility.Instance.RandomString(80, false);
            product2.Description = TestUtility.Instance.RandomString(255, false);
            product2.Image = TestUtility.Instance.RandomString(80, false);

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            var newName = TestUtility.Instance.RandomString(80, false);
            foreach (Product item in category.Products)
            {
                item.Name = newName;
            }

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Category category2 = Category.GetByCategoryId(TestCategoryID);
            Assert.IsTrue(category2.Products.Count == 2);
            Assert.IsTrue(string.Equals(category2.Products[0].Name, newName, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(string.Equals(category2.Products[1].Name, newName, StringComparison.InvariantCultureIgnoreCase));

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }


        /// <summary>
        /// Testing insert of a duplicate Product into a child collections.
        /// </summary>
        [Test]
        public void Step_20_Insert_Duplicate_Item_In_Child_Collection()
        {
            Console.WriteLine("20. Testing insert of a duplicate Product into a child collections.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            Assert.IsTrue(category.Products.Count == 0);

            Product product = category.Products.AddNew();

            Assert.IsTrue(category.Products.Count == 1);

            product.ProductId = TestProductID;
            product.Name = TestUtility.Instance.RandomString(80, false);
            product.Description = TestUtility.Instance.RandomString(255, false);
            product.Image = TestUtility.Instance.RandomString(80, false);

            Product product2 = category.Products.AddNew();

            Assert.IsTrue(category.Products.Count == 2);

            product2.ProductId = TestProductID;
            product2.Name = TestUtility.Instance.RandomString(80, false);
            product2.Description = TestUtility.Instance.RandomString(255, false);
            product2.Image = TestUtility.Instance.RandomString(80, false);

            try
            {
                Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
                category = category.Save();
                Assert.Fail("Should throw a duplicate entry exception.");
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Delete the Category entity into the database twice.
        /// </summary>
        [Test]
        public void Step_21_Double_Delete()
        {
            Console.WriteLine("20. Deleting the category twice.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            Assert.IsTrue(string.Equals(category.CategoryId, TestCategoryID, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsFalse(category.IsDeleted);
            category.Delete();

            Assert.IsTrue(category.IsDeleted);

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Assert.IsTrue(string.Equals(category.CategoryId, TestCategoryID, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsFalse(category.IsDeleted);

            category.Delete();

            // Shouldn't be able to call delete twice. I'd think.
            Assert.IsTrue(category.IsDeleted);
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Assert.IsTrue(string.Equals(category.CategoryId, TestCategoryID, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(category.IsNew);
            Assert.IsFalse(category.IsDeleted);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Delete the Category entity into the database twice.
        /// </summary>
        [Test]
        public void Step_22_Double_Delete()
        {
            Console.WriteLine("20. Deleting the category twice.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            Assert.IsTrue(string.Equals(category.CategoryId, TestCategoryID, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsFalse(category.IsDeleted);
            category.Delete();

            //Delete the category the other way before calling save which would call a delete. This should simulate a concurency issue.
            Category.DeleteCategory(TestCategoryID);
            try
            {
                Category.GetByCategoryId(TestCategoryID);
                Assert.Fail("Record exists when it should have been deleted.");
            }
            catch (Exception)
            {
                // Exception was thrown if record doesn't exist.
                Assert.IsTrue(true);
            }

            // Call delete on an already deleted item.
            Assert.IsTrue(category.IsDeleted);

            try
            {
                Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
                category = category.Save();
                Assert.Fail("Record exists when it should have been deleted.");
            }
            catch (Exception)
            {
                // Exception was thrown if record doesn't exist.
                Assert.IsTrue(true);
            }

            Assert.IsTrue(string.Equals(category.CategoryId, TestCategoryID, StringComparison.InvariantCultureIgnoreCase));
            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Updates a Category entity into the database.
        /// </summary>
        [Test]
        public void Step_23_Concurrency_Duplicate_Update()
        {
            Console.WriteLine("23. Updating the category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            var name = category.Name;
            var desc = category.Description;

            category.Name = TestUtility.Instance.RandomString(80, false);
            category.Description = TestUtility.Instance.RandomString(255, false);

            Category category2 = Category.GetByCategoryId(TestCategoryID);

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Assert.IsFalse(string.Equals(category.Name, name, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsFalse(string.Equals(category.Description, desc, StringComparison.InvariantCultureIgnoreCase));

            category2.Name = TestUtility.Instance.RandomString(80, false);
            category2.Description = TestUtility.Instance.RandomString(255, false);

            try
            {
                category2 = category2.Save();
                Assert.Fail("Concurrency exception should have been thrown.");
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }


        /// <summary>
        /// Testing update Child collections on an existing entity.
        /// </summary>
        [Test]
        public void Step_24_Concurrency_Update_Children()
        {
            Console.WriteLine("24. Testing update on child collections in a category.");
            Stopwatch watch = Stopwatch.StartNew();

            Category category = Category.GetByCategoryId(TestCategoryID);
            Assert.IsTrue(category.Products.Count == 0);

            Product product = category.Products.AddNew();

            Assert.IsTrue(category.Products.Count == 1);

            product.ProductId = TestProductID;
            product.Name = TestUtility.Instance.RandomString(80, false);
            product.Description = TestUtility.Instance.RandomString(255, false);
            product.Image = TestUtility.Instance.RandomString(80, false);

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            Category category2 = Category.GetByCategoryId(TestCategoryID);

            var newName = TestUtility.Instance.RandomString(80, false);
            foreach (Product item in category.Products)
            {
                item.Name = newName;
            }

            foreach (Product item in category2.Products)
            {
                item.Name = newName;
            }

            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            category = category.Save();

            try
            {
                category2 = category2.Save();
                Assert.Fail("Concurrency exception should have been thrown.");
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }

            Assert.IsTrue(string.Equals(category2.Products[0].Name, newName, StringComparison.InvariantCultureIgnoreCase));

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }
    }
}
