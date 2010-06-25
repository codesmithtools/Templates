Imports System
Imports System.Diagnostics
Imports NUnit.Framework
Imports PetShop.Tests.OF.StoredProcedures.PetShop.Tests.OF.StoredProcedures
Imports PetShop.Tests.OF.StoredProcedures

<TestFixture()> _
Public Class LineItemTests
#Region "Setup"

    Private _LineItemOrderID As Integer
    Private Property LineItemOrderID() As Integer
        Get
            Return _LineItemOrderID
        End Get
        Set(ByVal value As Integer)
            _LineItemOrderID = value
        End Set
    End Property
    Private _LineItemOrderID2 As Integer
    Private Property LineItemOrderID2() As Integer
        Get
            Return _LineItemOrderID2
        End Get
        Set(ByVal value As Integer)
            _LineItemOrderID2 = value
        End Set
    End Property

    <NUnit.Framework.TestFixtureSetUp()> _
    Public Sub Init()
        System.Console.WriteLine(New [String]("-"c, 75))
        System.Console.WriteLine("-- Testing the LineItem Entity --")
        System.Console.WriteLine(New [String]("-"c, 75))
    End Sub

    <SetUp()> _
    Public Sub Setup()
        LineItemOrderID = CreateOrder()
        LineItemOrderID2 = CreateOrder()
        CreateLineItem(LineItemOrderID, TestUtility.Instance.RandomNumber())
    End Sub

    <TearDown()> _
    Public Sub TearDown()
        Try
            Dim items As LineItemList = LineItemList.GetByOrderId(LineItemOrderID)
            items.Clear()
            items = items.Save()
        Catch generatedExceptionName As Exception
        End Try

        Try
            Order.DeleteOrder(LineItemOrderID)
        Catch generatedExceptionName As Exception
        End Try

        Try
            Dim items As LineItemList = LineItemList.GetByOrderId(LineItemOrderID2)
            items.Clear()
            items = items.Save()
        Catch generatedExceptionName As Exception
        End Try

        Try
            Order.DeleteOrder(LineItemOrderID2)
        Catch generatedExceptionName As Exception
        End Try
    End Sub

    <NUnit.Framework.TestFixtureTearDown()> _
    Public Sub Complete()
        System.Console.WriteLine("All Tests Completed")
        System.Console.WriteLine()
    End Sub

    <Test()> _
    Private Function CreateOrder() As Integer
        Dim order As Order = order.NewOrder()
        order.UserId = TestUtility.Instance.RandomString(20, False)
        order.OrderDate = TestUtility.Instance.RandomDateTime()
        order.Courier = TestUtility.Instance.RandomString(80, False)
        order.TotalPrice = TestUtility.Instance.RandomNumber(0, 500)
        order.AuthorizationNumber = TestUtility.Instance.RandomNumber(Integer.MinValue, Integer.MaxValue)
        order.Locale = TestUtility.Instance.RandomString(20, False)

        order.ShipAddr1 = TestUtility.Instance.RandomString(80, False)
        order.ShipAddr2 = TestUtility.Instance.RandomString(80, False)
        order.ShipCity = TestUtility.Instance.RandomString(80, False)
        order.ShipState = TestUtility.Instance.RandomString(80, False)
        order.ShipZip = TestUtility.Instance.RandomString(20, False)
        order.ShipCountry = TestUtility.Instance.RandomString(20, False)
        order.ShipToFirstName = TestUtility.Instance.RandomString(80, False)
        order.ShipToLastName = TestUtility.Instance.RandomString(80, False)

        order.BillAddr1 = TestUtility.Instance.RandomString(80, False)
        order.BillAddr2 = TestUtility.Instance.RandomString(80, False)
        order.BillCity = TestUtility.Instance.RandomString(80, False)
        order.BillState = TestUtility.Instance.RandomString(80, False)
        order.BillZip = TestUtility.Instance.RandomString(20, False)
        order.BillCountry = TestUtility.Instance.RandomString(20, False)
        order.BillToFirstName = TestUtility.Instance.RandomString(80, False)
        order.BillToLastName = TestUtility.Instance.RandomString(80, False)

        Assert.IsTrue(order.IsValid, order.BrokenRulesCollection.ToString())
        order = order.Save()

        Return order.OrderId
    End Function

    <Test()> _
    Private Sub CreateLineItem(ByVal orderID As Integer, ByVal lineNum As Integer)
        Dim lineItem As LineItem = lineItem.NewLineItem()
        lineItem.OrderId = orderID
        lineItem.LineNum = lineNum
        lineItem.ItemId = TestUtility.Instance.RandomString(10, False)
        lineItem.Quantity = TestUtility.Instance.RandomNumber(Integer.MinValue, Integer.MaxValue)
        lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500)

        Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString())
        lineItem = lineItem.Save()

        Assert.IsTrue(lineItem.OrderId = orderID)
    End Sub

#End Region

    ''' <summary>
    ''' Inserts a LineItem entity into the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_01_Insert_Duplicate()
        Console.WriteLine("1. Testing Duplicate Records.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        'Insert should fail as there should be a duplicate key.
        Dim lineItem As LineItem = lineItem.NewLineItem()
        lineItem.OrderId = LineItemOrderID
        lineItem.LineNum = TestUtility.Instance.RandomNumber()
        lineItem.ItemId = TestUtility.Instance.RandomString(10, False)
        lineItem.Quantity = TestUtility.Instance.RandomNumber(Integer.MinValue, Integer.MaxValue)
        lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500)

        Try
            Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString())
            lineItem = lineItem.Save()

            ' Fail as a duplicate record was entered.
            Assert.Fail("Fail as a duplicate record was entered and an exception was not thrown.")
        Catch generatedExceptionName As Exception
            Assert.IsTrue(True)
        End Try
        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Inserts a LineItem entity into the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_02_Insert()
        Console.WriteLine("2. Inserting new lineItem.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim lineItem As LineItem = lineItem.NewLineItem()
        lineItem.OrderId = LineItemOrderID
        lineItem.LineNum = TestUtility.Instance.RandomNumber()
        lineItem.ItemId = TestUtility.Instance.RandomString(10, False)
        lineItem.Quantity = TestUtility.Instance.RandomNumber(Integer.MinValue, Integer.MaxValue)
        lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500)

        Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString())
        lineItem = lineItem.Save()

        Assert.IsTrue(True)
        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Selects a sample of LineItem objects of the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_03_SelectAll()
        Console.WriteLine("3. Selecting all LineItems by calling GetByOrderId(""{0}"").", LineItemOrderID)
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim list As LineItemList = LineItemList.GetByOrderId(LineItemOrderID)
        Assert.IsTrue(list.Count = 1)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Updates a LineItem entity into the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_04_Update()
        Console.WriteLine("4. Updating the lineItem.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim lineItem As LineItem = lineItem.GetByOrderId(LineItemOrderID)
        Dim quantity = lineItem.Quantity
        Dim unitPrice = lineItem.UnitPrice

        lineItem.Quantity = TestUtility.Instance.RandomNumber(Integer.MinValue, Integer.MaxValue)
        lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500)

        Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString())
        lineItem = lineItem.Save()

        Assert.IsFalse(lineItem.Quantity = quantity)
        Assert.IsFalse(lineItem.UnitPrice = unitPrice)

        lineItem.Quantity = quantity
        lineItem.UnitPrice = unitPrice

        Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString())
        lineItem = lineItem.Save()

        Assert.IsTrue(lineItem.Quantity = quantity)
        Assert.IsTrue(lineItem.UnitPrice = unitPrice)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Delete the  LineItem entity into the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_05_Delete()
        Console.WriteLine("5. Deleting the lineItem.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim lineItem As LineItem = lineItem.GetByOrderId(LineItemOrderID)
        lineItem.Delete()

        Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString())
        lineItem = lineItem.Save()

        Assert.IsTrue(lineItem.IsNew)
        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Updates a non Identity PK column.
    ''' </summary>
    <Test()> _
    Public Sub Step_06_UpdatePrimaryKey()
        Console.WriteLine("6. Updating the non Identity Primary Key.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Console.WriteLine(vbTab & "Getting lineItem ""{0}""", LineItemOrderID)
        Dim lineItem As LineItem = lineItem.GetByOrderId(LineItemOrderID)
        lineItem.OrderId = LineItemOrderID2
        lineItem = lineItem.Save()
        Console.WriteLine(vbTab & "Set lineItemID to ""{0}""", LineItemOrderID2)
        Assert.IsTrue(lineItem.OrderId = LineItemOrderID2)

        Try
            lineItem.GetByOrderId(LineItemOrderID)
            Assert.Fail("Record exists when it should have been updated.")
        Catch generatedExceptionName As Exception
            Assert.IsTrue(True)
        End Try

        Dim validLineItem As LineItem = lineItem.GetByOrderId(LineItemOrderID2)
        Assert.IsTrue(validLineItem.OrderId = LineItemOrderID2)
        Console.WriteLine(vbTab & "PrimaryKey has been updated.")

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Tests the rules.
    ''' </summary>
    <Test()> _
    Public Sub Step_07_Rules()
        Console.WriteLine("7. Testing the state of the rules for the entity.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim lineItem As LineItem = lineItem.NewLineItem()
        Assert.IsFalse(lineItem.IsValid)

        lineItem.OrderId = LineItemOrderID
        Assert.IsFalse(lineItem.IsValid)

        lineItem.LineNum = TestUtility.Instance.RandomNumber()
        Assert.IsFalse(lineItem.IsValid)

        lineItem.ItemId = TestUtility.Instance.RandomString(10, False)
        Assert.IsTrue(lineItem.IsValid)

        lineItem.Quantity = TestUtility.Instance.RandomNumber(Integer.MinValue, Integer.MaxValue)
        Assert.IsTrue(lineItem.IsValid)

        lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500)
        Assert.IsTrue(lineItem.IsValid)

        ' Check LineItem.
        lineItem.OrderId = -10
        Assert.IsFalse(lineItem.IsValid, "This test will fail until we implement exists rule to check fk..")

        lineItem.OrderId = LineItemOrderID
        Assert.IsFalse(lineItem.IsValid)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Updates a LineItem entity into the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_08_Concurrency_Duplicate_Update()
        Console.WriteLine("8. Updating the lineItem.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim lineItem As LineItem = lineItem.GetByOrderId(LineItemOrderID)
        Dim lineItem2 As LineItem = lineItem.GetByOrderId(LineItemOrderID)
        Dim quantity As Integer = lineItem.Quantity
        Dim unitPrice As Decimal = lineItem.UnitPrice

        lineItem.Quantity = TestUtility.Instance.RandomNumber(Integer.MinValue, Integer.MaxValue)
        lineItem.UnitPrice = TestUtility.Instance.RandomNumber(0, 500)

        Assert.IsTrue(lineItem.IsValid, lineItem.BrokenRulesCollection.ToString())
        lineItem = lineItem.Save()

        lineItem2.Quantity = TestUtility.Instance.RandomNumber(Integer.MinValue, Integer.MaxValue)
        lineItem2.UnitPrice = TestUtility.Instance.RandomNumber(0, 500)

        Try
            lineItem2 = lineItem2.Save()
            Assert.Fail("Concurrency exception should have been thrown.")
        Catch generatedExceptionName As Exception
            Assert.IsTrue(True)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
End Class
