using System;
using System.Diagnostics;
using NUnit.Framework;

namespace PetShop.Tests.Collections.ReadOnlyRoot
{
    [TestFixture]
    public class ReadOnlyRootTests
    {
        #region Setup

        private static string TestCategoryID
        {
            get
            {
                return "BIRDS";
            }
        }

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
            //TestCategoryID = TestUtility.Instance.RandomString(10, false);

            //CreateCategory(TestCategoryID);
        }

        [TearDown]
        public void TearDown()
        {
            //try
            //{
            //    CategoryList list = CategoryList.GetByCategoryId(TestCategoryID);
            //    list.Clear(); 

            //    // Save the list..
            //}
            //catch (Exception) { }
        }

        [NUnit.Framework.TestFixtureTearDown]
        public void Complete()
        {
            System.Console.WriteLine("All Tests Completed");
            System.Console.WriteLine();
        }

        //[Test]
        //private void CreateCategory(string categoryID)
        //{
            //Category category = Category.NewCategory();
            //category.CategoryId = categoryID;
            //category.Name = TestUtility.Instance.RandomString(80, false);
            //category.Description = TestUtility.Instance.RandomString(255, false);

            //Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
            //category = category.Save();

            //Assert.IsTrue(category.CategoryId == categoryID);
        //}

        #endregion

        ///// <summary>
        ///// Inserts a Category entity into the database.
        ///// </summary>
        //[Test]
        //public void Step_01_Create_List()
        //{
        //    Console.WriteLine("1. Testing creating new list.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    CategoryList list = CategoryList.NewList();
        //    Assert.IsTrue(list.Count == 0);

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        /// <summary>
        /// Selects all Category entitys.
        /// </summary>
        [Test]
        public void Step_02_GetByCategoryID()
        {
            Console.WriteLine("2. Selects all Category entitys.");
            Stopwatch watch = Stopwatch.StartNew();

            CategoryInfoList list = CategoryInfoList.GetByCategoryId(TestCategoryID);
            Assert.IsTrue(list.Count == 1);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }
    }
}
