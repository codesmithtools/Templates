using System;
using System.Diagnostics;
using NUnit.Framework;

namespace PetShop.Tests.ObjF.StoredProcedures
{
    [TestFixture]
    public class LineItemTests
    {
        #region Setup

        private int LineItemOrderID { get; set; }
        private int LineItemOrderID2 { get; set; }

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
            LineItemOrderID = CreateOrder();
            LineItemOrderID2 = CreateOrder();
            CreateLineItem(LineItemOrderID, TestUtility.Instance.RandomNumber());
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

            try
            {
                LineItemList items = LineItemList.GetByOrderId(LineItemOrderID2);
                items.Clear();
                items = items.Save();
            }
            catch (Exception) { }

            try
            {
                Order.DeleteOrder(LineItemOrderID2);
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
        private int CreateOrder()
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

            Assert.IsTrue(order.IsValid, order.BrokenRulesCollection.ToString());
            order = order.Save();

            return order.OrderId;
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

            Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString());
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
            lineItem.LineNum = TestUtility.Instance.RandomNumber();
            lineItem.ItemId = TestUtility.Instance.RandomString(10, false);
            lineItem.Quantity = TestUtility.Instance.RandomNumber(int.MinValue, int.MaxValue);
            lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500);

            try
            {
                Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString());
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
            lineItem.LineNum = TestUtility.Instance.RandomNumber();
            lineItem.ItemId = TestUtility.Instance.RandomString(10, false);
            lineItem.Quantity = TestUtility.Instance.RandomNumber(int.MinValue, int.MaxValue);
            lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500);

            Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString());
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

            Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString());
            lineItem = lineItem.Save();

            Assert.IsFalse(lineItem.Quantity == quantity);
            Assert.IsFalse(lineItem.UnitPrice == unitPrice);

            lineItem.Quantity = quantity;
            lineItem.UnitPrice = unitPrice;

            Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString());
            lineItem = lineItem.Save();

            Assert.IsTrue(lineItem.Quantity == quantity);
            Assert.IsTrue(lineItem.UnitPrice == unitPrice);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Delete the  LineItem entity into the database.
        /// </summary>
        [Test]
        public void Step_05_Delete()
        {
            Console.WriteLine("5. Deleting the lineItem.");
            Stopwatch watch = Stopwatch.StartNew();

            LineItem lineItem = LineItem.GetByOrderId(LineItemOrderID);
            lineItem.Delete();

            Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString());
            lineItem = lineItem.Save();

            Assert.IsTrue(lineItem.IsNew);
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

            Console.WriteLine("\tGetting lineItem \"{0}\"", LineItemOrderID);
            LineItem lineItem = LineItem.GetByOrderId(LineItemOrderID);
            lineItem.OrderId = LineItemOrderID2;
            lineItem = lineItem.Save();
            Console.WriteLine("\tSet lineItemID to \"{0}\"", LineItemOrderID2);
            Assert.IsTrue(lineItem.OrderId == LineItemOrderID2);

            try
            {
                LineItem.GetByOrderId(LineItemOrderID);
                Assert.Fail("Record exists when it should have been updated.");
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }

            LineItem validLineItem = LineItem.GetByOrderId(LineItemOrderID2);
            Assert.IsTrue(validLineItem.OrderId == LineItemOrderID2);
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

            LineItem lineItem = LineItem.NewLineItem();
            Assert.IsFalse(lineItem.IsValid);

            lineItem.OrderId = LineItemOrderID;
            Assert.IsFalse(lineItem.IsValid);

            lineItem.LineNum = TestUtility.Instance.RandomNumber();
            Assert.IsFalse(lineItem.IsValid);

            lineItem.ItemId = TestUtility.Instance.RandomString(10, false);
            Assert.IsTrue(lineItem.IsValid);
            
            lineItem.Quantity = TestUtility.Instance.RandomNumber(int.MinValue, int.MaxValue);
            Assert.IsTrue(lineItem.IsValid);

            lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500);
            Assert.IsTrue(lineItem.IsValid);

            // Check LineItem.
            lineItem.OrderId = -10;
            Assert.IsFalse(lineItem.IsValid, "This test will fail until we implement exists rule to check fk..");

            lineItem.OrderId = LineItemOrderID;
            Assert.IsFalse(lineItem.IsValid);

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Updates a LineItem entity into the database.
        /// </summary>
        [Test]
        public void Step_08_Concurrency_Duplicate_Update()
        {
            Console.WriteLine("8. Updating the lineItem.");
            Stopwatch watch = Stopwatch.StartNew();

            LineItem lineItem = LineItem.GetByOrderId(LineItemOrderID);
            LineItem lineItem2 = LineItem.GetByOrderId(LineItemOrderID);
            var quantity = lineItem.Quantity;
            var unitPrice = lineItem.UnitPrice;

            lineItem.Quantity = TestUtility.Instance.RandomNumber(int.MinValue, int.MaxValue);
            lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500);

            Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString());
            lineItem = lineItem.Save();

            lineItem2.Quantity = TestUtility.Instance.RandomNumber(int.MinValue, int.MaxValue);
            lineItem2.UnitPrice = TestUtility.Instance.RandomNumber(0, 500);

            try
            {
                lineItem2 = lineItem2.Save();
                Assert.Fail("Concurrency exception should have been thrown.");
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        }
    }
}
