using System;
using System.Diagnostics;
using NUnit.Framework;

namespace PetShop.Tests.StoredProcedures
{
    [TestFixture]
    public class LineItemTests
    {
        #region Setup

        private int LineItemOrderID { get; set; }
        private int LineItemLineNum { get; set; }

        [NUnit.Framework.TestFixtureSetUp]
        public void Init()
        {
            System.Console.WriteLine(new String('-', 75));
            System.Console.WriteLine("-- Testing the LineItem Entity --");
            System.Console.WriteLine(new String('-', 75));
        }

        [SetUp]
        public void Setup()
        {
            LineItemOrderID = TestUtility.Instance.RandomNumber();
            LineItemLineNum = TestUtility.Instance.RandomNumber();

            TearDown();
            CreateOrder();
            CreateLineItem(LineItemOrderID, LineItemLineNum);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                LineItemList items = LineItemList.GetByOrderId(LineItemOrderID);
                items.Clear();
                items = items.Save();
            }
            catch (Exception) { }

            try
            {
                Order.DeleteOrder(LineItemOrderID);
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
        private void CreateOrder()
        {
            Order order = Order.NewOrder();
            order.UserId = TestUtility.Instance.RandomString(20, false);
            order.OrderDate = TestUtility.Instance.RandomDateTime();
            order.Courier = TestUtility.Instance.RandomString(80, false);
            order.TotalPrice = TestUtility.Instance.RandomNumber(0, 500);
            order.AuthorizationNumber = TestUtility.Instance.RandomNumber(int.MinValue, int.MaxValue);
            order.Locale = TestUtility.Instance.RandomString(20, false);

            order.ShipAddr1 = TestUtility.Instance.RandomString(80, false);
            order.ShipAddr2 = TestUtility.Instance.RandomString(80, false);
            order.ShipCity = TestUtility.Instance.RandomString(80, false);
            order.ShipState = TestUtility.Instance.RandomString(80, false);
            order.ShipZip = TestUtility.Instance.RandomString(20, false);
            order.ShipCountry = TestUtility.Instance.RandomString(20, false);
            order.ShipToFirstName = TestUtility.Instance.RandomString(80, false);
            order.ShipToLastName = TestUtility.Instance.RandomString(80, false);

            order.BillAddr1 = TestUtility.Instance.RandomString(80, false);
            order.BillAddr2 = TestUtility.Instance.RandomString(80, false);
            order.BillCity = TestUtility.Instance.RandomString(80, false);
            order.BillState = TestUtility.Instance.RandomString(80, false);
            order.BillZip = TestUtility.Instance.RandomString(20, false);
            order.BillCountry = TestUtility.Instance.RandomString(20, false);
            order.BillToFirstName = TestUtility.Instance.RandomString(80, false);
            order.BillToLastName = TestUtility.Instance.RandomString(80, false);

            order = order.Save();

            LineItemOrderID = order.OrderId;
        }

        [Test]
        private void CreateLineItem(int orderID, int lineNum)
        {
            LineItem lineItem = LineItem.NewLineItem();
            lineItem.OrderId = orderID;
            lineItem.LineNum = lineNum;
            lineItem.ItemId = TestUtility.Instance.RandomString(10, false);
            lineItem.Quantity = TestUtility.Instance.RandomNumber(int.MinValue, int.MaxValue);
            lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500);

            lineItem = lineItem.Save();

            Assert.IsTrue(lineItem.OrderId == orderID);
        }

        #endregion

        /// <summary>
        /// Inserts a LineItem entity into the database.
        /// </summary>
        [Test]
        public void Step_01_Insert_Duplicate()
        {
            Console.WriteLine("1. Testing Duplicate Records.");
            Stopwatch watch = Stopwatch.StartNew();

            //Insert should fail as there should be a duplicate key.
            LineItem lineItem = LineItem.NewLineItem();
            lineItem.OrderId = LineItemOrderID;
            lineItem.LineNum = LineItemLineNum;
            lineItem.ItemId = TestUtility.Instance.RandomString(10, false);
            lineItem.Quantity = TestUtility.Instance.RandomNumber(int.MinValue, int.MaxValue);
            lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500);

            try
            {
                lineItem = lineItem.Save();

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
        /// Inserts a LineItem entity into the database.
        /// </summary>
        [Test]
        public void Step_02_Insert()
        {
            Console.WriteLine("2. Inserting new lineItem.");
            Stopwatch watch = Stopwatch.StartNew();

            LineItem lineItem = LineItem.NewLineItem();
            lineItem.OrderId = LineItemOrderID;
            lineItem.LineNum = LineItemLineNum;
            lineItem.ItemId = TestUtility.Instance.RandomString(10, false);
            lineItem.Quantity = TestUtility.Instance.RandomNumber(int.MinValue, int.MaxValue);
            lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500);

            lineItem = lineItem.Save();

            Assert.IsTrue(true);
            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Selects a sample of LineItem objects of the database.
        /// </summary>
        [Test]
        public void Step_03_SelectAll()
        {
            Console.WriteLine("3. Selecting all LineItems by calling GetByOrderId(\"{0}\").", LineItemOrderID);
            Stopwatch watch = Stopwatch.StartNew();

            LineItemList list = LineItemList.GetByOrderId(LineItemOrderID);
            Assert.IsTrue(list.Count == 1);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Updates a LineItem entity into the database.
        /// </summary>
        [Test]
        public void Step_04_Update()
        {
            Console.WriteLine("4. Updating the lineItem.");
            Stopwatch watch = Stopwatch.StartNew();

            LineItem lineItem = LineItem.GetByOrderId(LineItemOrderID);
            var quantity = lineItem.Quantity;
            var unitPrice = lineItem.UnitPrice;

            lineItem.Quantity = TestUtility.Instance.RandomNumber(int.MinValue, int.MaxValue);
            lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500);

            lineItem = lineItem.Save();

            Assert.IsFalse(lineItem.Quantity == quantity);
            Assert.IsFalse(lineItem.UnitPrice == unitPrice);

            lineItem.Quantity = quantity;
            lineItem.UnitPrice = unitPrice;

            lineItem = lineItem.Save();

            Assert.IsTrue(lineItem.Quantity == quantity);
            Assert.IsTrue(lineItem.UnitPrice == unitPrice);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        ///// <summary>
        ///// Delete the  LineItem entity into the database.
        ///// </summary>
        //[Test]
        //public void Step_05_Delete()
        //{
        //    Console.WriteLine("5. Deleting the lineItem.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.GetByLineItemId(TestLineItemID);
        //    lineItem.Delete();

        //    lineItem = lineItem.Save();

        //    Assert.IsTrue(lineItem.IsNew);
        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Updates a non Identity PK column.
        ///// </summary>
        //[Test]
        //public void Step_06_UpdatePrimaryKey()
        //{
        //    Console.WriteLine("6. Updating the non Identity Primary Key.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    Console.WriteLine("\tGetting lineItem \"{0}\"", TestLineItemID);
        //    LineItem lineItem = LineItem.GetByLineItemId(TestLineItemID);
        //    lineItem.LineItemId = TestLineItemID2;
        //    lineItem = lineItem.Save();
        //    Console.WriteLine("\tSet lineItemID to \"{0}\"", TestLineItemID2);
        //    Assert.IsTrue(lineItem.LineItemId == TestLineItemID2);

        //    try
        //    {
        //        LineItem.GetByLineItemId(TestLineItemID);
        //        Assert.Fail("Record exists when it should have been updated.");
        //    }
        //    catch (Exception)
        //    {
        //        Assert.IsTrue(true);
        //    }

        //    LineItem validLineItem = LineItem.GetByLineItemId(TestLineItemID2);
        //    Assert.IsTrue(validLineItem.LineItemId == TestLineItemID2);
        //    Console.WriteLine("\tPrimaryKey has been updated.");

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Tests the rules.
        ///// </summary>
        //[Test]
        //public void Step_07_Rules()
        //{
        //    Console.WriteLine("7. Testing the state of the rules for the entity.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.NewLineItem();
        //    Assert.IsFalse(lineItem.IsValid);

        //    lineItem.LineItemId = TestLineItemID;
        //    Assert.IsTrue(lineItem.IsValid);

        //    lineItem.Name = TestUtility.Instance.RandomString(80, false);
        //    Assert.IsTrue(lineItem.IsValid);

        //    lineItem.Descn = TestUtility.Instance.RandomString(255, false);
        //    Assert.IsTrue(lineItem.IsValid);

        //    // Check LineItem.
        //    lineItem.LineItemId = null;
        //    Assert.IsFalse(lineItem.IsValid);

        //    lineItem.LineItemId = TestUtility.Instance.RandomString(11, false);
        //    Assert.IsFalse(lineItem.IsValid);

        //    lineItem.LineItemId = TestLineItemID;
        //    Assert.IsTrue(lineItem.IsValid);

        //    // Check Name.
        //    lineItem.Name = null;
        //    Assert.IsTrue(lineItem.IsValid);

        //    lineItem.Name = TestUtility.Instance.RandomString(81, false);
        //    Assert.IsFalse(lineItem.IsValid);

        //    lineItem.Name = TestUtility.Instance.RandomString(80, false);
        //    Assert.IsTrue(lineItem.IsValid);

        //    // Check Descn.
        //    lineItem.Descn = null;
        //    Assert.IsTrue(lineItem.IsValid);

        //    lineItem.Descn = TestUtility.Instance.RandomString(256, false);
        //    Assert.IsFalse(lineItem.IsValid);

        //    lineItem.Descn = TestUtility.Instance.RandomString(80, false);
        //    Assert.IsTrue(lineItem.IsValid);

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Inserts a LineItem entity into the database.
        ///// </summary>
        //[Test]
        //public void Step_08_Entity_State()
        //{
        //    Console.WriteLine("8. Checking the state of the entity.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    //Insert should fail as there should be a duplicate key.
        //    LineItem lineItem = LineItem.NewLineItem();
        //    Assert.IsTrue(lineItem.IsDirty);

        //    lineItem.LineItemId = TestLineItemID2;
        //    lineItem.Name = TestUtility.Instance.RandomString(80, false);
        //    lineItem.Descn = TestUtility.Instance.RandomString(255, false);

        //    Assert.IsTrue(lineItem.IsNew);
        //    Assert.IsTrue(lineItem.IsDirty);
        //    Assert.IsFalse(lineItem.IsDeleted);

        //    lineItem = lineItem.Save();

        //    Assert.IsFalse(lineItem.IsNew);
        //    Assert.IsFalse(lineItem.IsDirty);
        //    Assert.IsFalse(lineItem.IsDeleted);

        //    lineItem.Name = TestUtility.Instance.RandomString(80, false);
        //    Assert.IsTrue(lineItem.IsDirty);
        //    lineItem = lineItem.Save();

        //    Assert.IsFalse(lineItem.IsNew);
        //    Assert.IsFalse(lineItem.IsDirty);
        //    Assert.IsFalse(lineItem.IsDeleted);

        //    lineItem.Delete();
        //    Assert.IsTrue(lineItem.IsDeleted);
        //    lineItem = lineItem.Save();
        //    Assert.IsFalse(lineItem.IsDeleted);
        //    Assert.IsTrue(lineItem.IsNew);

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Testing Child collections on a new entity.
        ///// </summary>
        //[Test]
        //public void Step_09_New_Entity_Collection()
        //{
        //    Console.WriteLine("9. Testing child collections on a new lineItem.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.NewLineItem();
        //    var count = lineItem.Products.Count;

        //    Assert.IsTrue(count == 0);

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Testing Child collections on a new entity.
        ///// </summary>
        //[Test]
        //public void Step_10_Existing_Entity_With_No_Collection()
        //{
        //    Console.WriteLine("10. Testing child collections on a lineItem with no child collection items.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.GetByLineItemId(TestLineItemID);
        //    var count = lineItem.Products.Count;

        //    Assert.IsTrue(count == 0);

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Selects a sample of LineItem objects of the database using an invalid lineItem ID.
        ///// </summary>
        //[Test]
        //public void Step_12_SelectAll_Invalid_Value()
        //{
        //    Console.WriteLine("12. Selecting all categories by calling GetByLineItemId with an invalid value");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    try
        //    {
        //        LineItemList.GetByLineItemId(TestUtility.Instance.RandomString(10, false));

        //        Assert.Fail("This call to GetByLineItemID should throw an exception");
        //    }
        //    catch (Exception)
        //    {
        //        Assert.IsTrue(true);
        //    }
        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Selects a sample of LineItem objects of the database using an invalid lineItem ID.
        ///// </summary>
        //[Test]
        //public void Step_13_SelectAll_With_Oversized_LineItemID()
        //{
        //    Console.WriteLine("13. Selecting all categories by calling GetByLineItemId with an invalid value");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    try
        //    {
        //        LineItemList.GetByLineItemId(TestLineItemID + "a");

        //        Assert.Fail("This call to GetByLineItemID should throw an exception");
        //    }
        //    catch (Exception)
        //    {
        //        Assert.IsTrue(true);
        //    }

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Testing Insert Child collections on a new entity.
        ///// </summary>
        //[Test]
        //public void Step_14_Insert_Child_Collection_On_New_LineItem()
        //{
        //    Console.WriteLine("14. Testing insert on child collections in a new lineItem.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.NewLineItem();
        //    lineItem.LineItemId = TestLineItemID2;
        //    lineItem.Name = TestUtility.Instance.RandomString(80, false);
        //    lineItem.Descn = TestUtility.Instance.RandomString(255, false);

        //    Assert.IsTrue(lineItem.Products.Count == 0);

        //    Product product = lineItem.Products.AddNew();

        //    Assert.IsTrue(lineItem.Products.Count == 1);

        //    product.ProductId = TestProductID;
        //    product.Name = TestUtility.Instance.RandomString(80, false);
        //    product.Descn = TestUtility.Instance.RandomString(255, false);
        //    product.Image = TestUtility.Instance.RandomString(80, false);

        //    lineItem = lineItem.Save();

        //    LineItem lineItem2 = LineItem.GetByLineItemId(TestLineItemID2);

        //    Assert.IsTrue(lineItem.Products.Count == lineItem2.Products.Count);
        //    Assert.IsTrue(lineItem.LineItemId == lineItem2.Products[0].LineItemId);

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Testing Insert Child collections on an existing entity.
        ///// </summary>
        //[Test]
        //public void Step_15_Insert_Child_Collection()
        //{
        //    Console.WriteLine("15. Testing insert on child collections in a lineItem.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.GetByLineItemId(TestLineItemID);
        //    Assert.IsTrue(lineItem.Products.Count == 0);

        //    Product product = lineItem.Products.AddNew();

        //    Assert.IsTrue(lineItem.Products.Count == 1);

        //    product.ProductId = TestProductID;
        //    product.Name = TestUtility.Instance.RandomString(80, false);
        //    product.Descn = TestUtility.Instance.RandomString(255, false);
        //    product.Image = TestUtility.Instance.RandomString(80, false);

        //    lineItem = lineItem.Save();

        //    LineItem lineItem2 = LineItem.GetByLineItemId(TestLineItemID);

        //    Assert.IsTrue(lineItem.Products.Count == lineItem2.Products.Count);
        //    Assert.IsTrue(lineItem.LineItemId == lineItem2.Products[0].LineItemId);

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Testing update on a new child collections on a new entity.
        ///// </summary>
        //[Test]
        //public void Step_16_Update_Child_Collection_On_New_LineItem()
        //{
        //    Console.WriteLine("16. Testing update on new child collections in a new lineItem.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.NewLineItem();
        //    lineItem.LineItemId = TestLineItemID2;
        //    lineItem.Name = TestUtility.Instance.RandomString(80, false);
        //    lineItem.Descn = TestUtility.Instance.RandomString(255, false);

        //    Assert.IsTrue(lineItem.Products.Count == 0);

        //    Product product = lineItem.Products.AddNew();

        //    Assert.IsTrue(lineItem.Products.Count == 1);

        //    product.ProductId = TestProductID;
        //    product.Name = TestUtility.Instance.RandomString(80, false);
        //    product.Descn = TestUtility.Instance.RandomString(255, false);
        //    product.Image = TestUtility.Instance.RandomString(80, false);

        //    var newName = TestUtility.Instance.RandomString(80, false);
        //    foreach (Product item in lineItem.Products)
        //    {
        //        item.Name = newName;
        //    }

        //    lineItem = lineItem.Save();

        //    LineItem lineItem2 = LineItem.GetByLineItemId(TestLineItemID2);
        //    Assert.IsTrue(string.Equals(lineItem2.Products[0].Name, newName, StringComparison.InvariantCultureIgnoreCase));

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Testing update Child collections on an existing entity.
        ///// </summary>
        //[Test]
        //public void Step_17_Update_Child_Collection()
        //{
        //    Console.WriteLine("17. Testing update on child collections in a lineItem.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.GetByLineItemId(TestLineItemID);
        //    Assert.IsTrue(lineItem.Products.Count == 0);

        //    Product product = lineItem.Products.AddNew();

        //    Assert.IsTrue(lineItem.Products.Count == 1);

        //    product.ProductId = TestProductID;
        //    product.Name = TestUtility.Instance.RandomString(80, false);
        //    product.Descn = TestUtility.Instance.RandomString(255, false);
        //    product.Image = TestUtility.Instance.RandomString(80, false);

        //    lineItem = lineItem.Save();

        //    var newName = TestUtility.Instance.RandomString(80, false);
        //    foreach (Product item in lineItem.Products)
        //    {
        //        item.Name = newName;
        //    }

        //    lineItem = lineItem.Save();

        //    LineItem lineItem2 = LineItem.GetByLineItemId(TestLineItemID);
        //    Assert.IsTrue(string.Equals(lineItem2.Products[0].Name, newName, StringComparison.InvariantCultureIgnoreCase));

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Testing update Child collections on an existing entity.
        ///// </summary>
        //[Test]
        //public void Step_18_Update_PK_Of_Child_Collection()
        //{
        //    Console.WriteLine("18. Testing update of PK on child collections in a lineItem.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.GetByLineItemId(TestLineItemID);
        //    Assert.IsTrue(lineItem.Products.Count == 0);

        //    Product product = lineItem.Products.AddNew();

        //    Assert.IsTrue(lineItem.Products.Count == 1);

        //    product.ProductId = TestProductID;
        //    product.Name = TestUtility.Instance.RandomString(80, false);
        //    product.Descn = TestUtility.Instance.RandomString(255, false);
        //    product.Image = TestUtility.Instance.RandomString(80, false);

        //    lineItem = lineItem.Save();

        //    lineItem.LineItemId = TestLineItemID2;

        //    lineItem = lineItem.Save();

        //    LineItem lineItem2 = LineItem.GetByLineItemId(TestLineItemID2);
        //    Assert.IsTrue(lineItem2.Products.Count == 1);
        //    Assert.IsTrue(string.Equals(lineItem2.Products[0].LineItemId, lineItem.LineItemId, StringComparison.InvariantCultureIgnoreCase));

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Testing update of Every PK in a Child collections on an existing entity.
        ///// </summary>
        //[Test]
        //public void Step_19_Update_Every_PK_In_Child_Collection()
        //{
        //    Console.WriteLine("19. Testing update of every PK in a child collections in a lineItem.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.GetByLineItemId(TestLineItemID);
        //    Assert.IsTrue(lineItem.Products.Count == 0);

        //    Product product = lineItem.Products.AddNew();

        //    Assert.IsTrue(lineItem.Products.Count == 1);

        //    product.ProductId = TestProductID;
        //    product.Name = TestUtility.Instance.RandomString(80, false);
        //    product.Descn = TestUtility.Instance.RandomString(255, false);
        //    product.Image = TestUtility.Instance.RandomString(80, false);

        //    Product product2 = lineItem.Products.AddNew();

        //    Assert.IsTrue(lineItem.Products.Count == 2);

        //    product2.ProductId = TestProductID2;
        //    product2.Name = TestUtility.Instance.RandomString(80, false);
        //    product2.Descn = TestUtility.Instance.RandomString(255, false);
        //    product2.Image = TestUtility.Instance.RandomString(80, false);

        //    lineItem = lineItem.Save();

        //    var newName = TestUtility.Instance.RandomString(80, false);
        //    foreach (Product item in lineItem.Products)
        //    {
        //        item.Name = newName;
        //    }

        //    lineItem = lineItem.Save();

        //    LineItem lineItem2 = LineItem.GetByLineItemId(TestLineItemID);
        //    Assert.IsTrue(lineItem2.Products.Count == 2);
        //    Assert.IsTrue(string.Equals(lineItem2.Products[0].Name, newName, StringComparison.InvariantCultureIgnoreCase));
        //    Assert.IsTrue(string.Equals(lineItem2.Products[1].Name, newName, StringComparison.InvariantCultureIgnoreCase));

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}


        ///// <summary>
        ///// Testing insert of a duplicate Product into a child collections.
        ///// </summary>
        //[Test]
        //public void Step_20_Insert_Duplicate_Item_In_Child_Collection()
        //{
        //    Console.WriteLine("20. Testing insert of a duplicate Product into a child collections.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.GetByLineItemId(TestLineItemID);
        //    Assert.IsTrue(lineItem.Products.Count == 0);

        //    Product product = lineItem.Products.AddNew();

        //    Assert.IsTrue(lineItem.Products.Count == 1);

        //    product.ProductId = TestProductID;
        //    product.Name = TestUtility.Instance.RandomString(80, false);
        //    product.Descn = TestUtility.Instance.RandomString(255, false);
        //    product.Image = TestUtility.Instance.RandomString(80, false);

        //    Product product2 = lineItem.Products.AddNew();

        //    Assert.IsTrue(lineItem.Products.Count == 2);

        //    product2.ProductId = TestProductID;
        //    product2.Name = TestUtility.Instance.RandomString(80, false);
        //    product2.Descn = TestUtility.Instance.RandomString(255, false);
        //    product2.Image = TestUtility.Instance.RandomString(80, false);

        //    try
        //    {
        //        lineItem = lineItem.Save();
        //        Assert.Fail("Should throw a duplicate entry exception.");
        //    }
        //    catch (Exception)
        //    {
        //        Assert.IsTrue(true);
        //    }

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Delete the LineItem entity into the database twice.
        ///// </summary>
        //[Test]
        //public void Step_21_Double_Delete()
        //{
        //    Console.WriteLine("20. Deleting the lineItem twice.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.GetByLineItemId(TestLineItemID);
        //    Assert.IsTrue(string.Equals(lineItem.LineItemId, TestLineItemID, StringComparison.InvariantCultureIgnoreCase));
        //    Assert.IsFalse(lineItem.IsDeleted);
        //    lineItem.Delete();

        //    Assert.IsTrue(lineItem.IsDeleted);

        //    lineItem = lineItem.Save();

        //    Assert.IsTrue(string.Equals(lineItem.LineItemId, TestLineItemID, StringComparison.InvariantCultureIgnoreCase));
        //    Assert.IsFalse(lineItem.IsDeleted);

        //    lineItem.Delete();

        //    // Shouldn't be able to call delete twice. I'd think.
        //    Assert.IsTrue(lineItem.IsDeleted);
        //    lineItem = lineItem.Save();

        //    Assert.IsTrue(string.Equals(lineItem.LineItemId, TestLineItemID, StringComparison.InvariantCultureIgnoreCase));
        //    Assert.IsTrue(lineItem.IsNew);
        //    Assert.IsFalse(lineItem.IsDeleted);

        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        ///// <summary>
        ///// Delete the LineItem entity into the database twice.
        ///// </summary>
        //[Test]
        //public void Step_22_Double_Delete()
        //{
        //    Console.WriteLine("20. Deleting the lineItem twice.");
        //    Stopwatch watch = Stopwatch.StartNew();

        //    LineItem lineItem = LineItem.GetByLineItemId(TestLineItemID);
        //    Assert.IsTrue(string.Equals(lineItem.LineItemId, TestLineItemID, StringComparison.InvariantCultureIgnoreCase));
        //    Assert.IsFalse(lineItem.IsDeleted);
        //    lineItem.Delete();

        //    //Delete the lineItem the other way before calling save which would call a delete. This should simulate a concurency issue.
        //    LineItem.DeleteLineItem(TestLineItemID);
        //    try
        //    {
        //        LineItem.GetByLineItemId(TestLineItemID);
        //        Assert.Fail("Record exists when it should have been deleted.");
        //    }
        //    catch (Exception)
        //    {
        //        // Exception was thrown if record doesn't exist.
        //        Assert.IsTrue(true);
        //    }

        //    // Call delete on an already deleted item.
        //    Assert.IsTrue(lineItem.IsDeleted);

        //    try
        //    {
        //        lineItem = lineItem.Save();
        //        Assert.Fail("Record exists when it should have been deleted.");
        //    }
        //    catch (Exception)
        //    {
        //        // Exception was thrown if record doesn't exist.
        //        Assert.IsTrue(true);
        //    }

        //    Assert.IsTrue(string.Equals(lineItem.LineItemId, TestLineItemID, StringComparison.InvariantCultureIgnoreCase));
        //    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        //}

        // Add concurrency tests.
    }
}
