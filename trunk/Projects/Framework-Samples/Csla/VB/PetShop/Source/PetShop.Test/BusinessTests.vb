Imports System
Imports System.Diagnostics

Imports NUnit.Framework

Imports PetShop.Business

<TestFixture()> _
Public Class BusinessTests
    <SetUp()> _
    Public Sub Setup()
    End Sub

    <TearDown()> _
    Public Sub TearDown()
    End Sub

    Private Const NAME As String = "UnitTests"
    Private Const ID As String = "Unit-Test"
    Private Shared _supplierId As Integer = 1

#Region "Profile"

    <Test()> _
    Public Sub CreateProfile()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim profile As Profile = profile.NewProfile()
        profile.Username = NAME
        profile.ApplicationName = "PetShop..Businesss"
        profile.IsAnonymous = False
        profile.LastActivityDate = DateTime.Now
        profile.LastUpdatedDate = DateTime.Now

        Try
            profile = profile.Save()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Assert.IsTrue(True)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub FetchProfile()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim profile As Profile = profile.GetProfile(NAME)

        Assert.IsTrue(profile.Username = NAME)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub UpdateProfile()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim profile As Profile = profile.GetProfile(NAME)
        profile.IsAnonymous = True

        profile = profile.Save()

        Assert.IsTrue(profile.GetProfile(NAME).IsAnonymous.Value)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Category"

    <Test()> _
    Public Sub CreateCategory()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.NewCategory()
        category.CategoryId = ID
        category.Name = ""
        category.Descn = ""

        Try
            category = category.Save()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Assert.IsTrue(True)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub FetchCategory()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetCategory(ID)

        Assert.IsTrue(category.CategoryId = ID)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub UpdateCategory()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetCategory(ID)
        category.Descn = "This is a ."

        category = category.Save()

        Assert.IsTrue(category.GetCategory(ID).Descn = "This is a .")

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Inventory"

    <Test()> _
    Public Sub CreateInventory()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim inventory As Inventory = inventory.NewInventory()
        inventory.ItemId = ID
        inventory.Qty = 10

        Try
            inventory = inventory.Save()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Assert.IsTrue(True)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub FetchInventory()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim inventory As Inventory = inventory.GetInventory(ID)

        Assert.IsTrue(inventory.ItemId = ID)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub UpdateInventory()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim inventory As Inventory = inventory.GetInventory(ID)
        inventory.Qty = 100

        inventory = inventory.Save()

        Assert.IsTrue(inventory.GetInventory(ID).Qty = 100)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Product"

    <Test()> _
    Public Sub CreateProduct()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim product As Product = product.NewProduct()
        product.ProductId = ID
        product.CategoryId = ID
        product.Image = "/.png"
        product.Descn = ""
        product.Name = ""

        Try
            product = product.Save()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Assert.IsTrue(True)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub FetchProduct()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim product As Product = product.GetProduct(ID)

        Assert.IsTrue(product.ProductId = ID)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub UpdateProduct()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim product As Product = product.GetProduct(ID)
        product.Descn = "This is a "

        product = product.Save()

        Assert.IsTrue(product.GetProduct(ID).Descn = "This is a ")

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Supplier"

    <Test()> _
    Public Sub CreateSupplier()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim supplier As Supplier = supplier.NewSupplier()
        supplier.Name = NAME
        supplier.Status = "AB"
        supplier.Addr1 = "One  Way"
        supplier.Addr2 = "Two  Way"
        supplier.City = "Dallas"
        supplier.State = "TX"
        supplier.Zip = "90210"
        supplier.Phone = "555-555-5555"

        Try
            supplier = supplier.Save()
            _supplierId = supplier.SuppId
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Assert.IsTrue(True)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub FetchSupplier()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim supplier As Supplier = supplier.GetSupplier(_supplierId)

        Assert.IsTrue(supplier.SuppId = _supplierId)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub UpdateSupplier()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim supplier As Supplier = supplier.GetSupplier(_supplierId)
        supplier.Phone = "111-111-1111"

        supplier = supplier.Save()

        Assert.IsTrue(supplier.GetSupplier(_supplierId).Phone = "111-111-1111")

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Item"

    <Test()> _
    Public Sub CreateItem()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim item As Item = item.NewItem()
        item.ItemId = ID
        item.Image = "/.png"
        item.ListPrice = 0
        item.Name = ""
        item.ProductId = ID
        item.Status = ""
        item.SuppId = _supplierId
        item.UnitCost = 0

        Try
            item = item.Save()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Assert.IsTrue(True)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub FetchItem()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim item As Item = item.GetItem(ID)

        Assert.IsTrue(item.ItemId = ID)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub UpdateItem()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim item As Item = item.GetItem(ID)
        item.ListPrice = 111

        item = item.Save()

        Assert.IsTrue(item.GetItem(ID).ListPrice = 111)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region


#Region "ShoppingCart"

    <Test()> _
    Public Sub CreateAndFetchShoppingCart()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim profile As Profile = profile.GetProfile(NAME)
        Assert.IsTrue(profile.ShoppingCart.Count = 0)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub AddItemToShoppingCart()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim profile As Profile = profile.GetProfile(NAME)
        profile.ShoppingCart.Add(ID, profile.UniqueID, True)
        profile = profile.Save()

        Assert.IsTrue(profile.GetProfile(NAME).ShoppingCart.Count = 1)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub RemoveItemFromShoppingCart()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim profile As Profile = profile.GetProfile(NAME)
        profile.ShoppingCart.Remove(ID)
        profile = profile.Save()

        Assert.IsTrue(profile.GetProfile(NAME).ShoppingCart.Count = 0)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub UpdateItemQuantityShoppingCart()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        'Add new Item to the cart.
        Dim profile As Profile = profile.GetProfile(NAME)
        profile.ShoppingCart.Add(ID, profile.UniqueID, True)
        profile = profile.Save()

        profile = profile.GetProfile(NAME)
        profile.ShoppingCart.Add(ID, profile.UniqueID, True)
        profile = profile.Save()

        profile = profile.GetProfile(NAME)

        Assert.IsTrue(profile.ShoppingCart.Count = 1 AndAlso profile.ShoppingCart(0).Quantity = 2)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub ClearShoppingCart()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        'Add new Item to the cart.
        Dim profile As Profile = profile.GetProfile(NAME)
        profile.ShoppingCart.Add(ID, profile.UniqueID, True)
        profile = profile.Save()

        'Clear the cart.
        profile = profile.GetProfile(NAME)
        profile.ShoppingCart.Clear()
        profile = profile.Save()

        Assert.IsTrue(profile.GetProfile(NAME).ShoppingCart.Count = 0)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Deletes"

#Region "Delete Item"

    <Test()> _
    Public Sub DeleteItem()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim item As Item = item.GetItem(ID)
        item.Delete()

        item = item.Save()

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Delete Supplier"

    <Test()> _
    Public Sub DeleteSupplier()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim supplier As Supplier = supplier.GetSupplier(_supplierId)
        supplier.Delete()

        supplier = supplier.Save()

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Delete Product"

    <Test()> _
    Public Sub DeleteProduct()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim product As Product = product.GetProduct(ID)
        product.Delete()

        product = product.Save()

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Delete Inventory"

    <Test()> _
    Public Sub DeleteInventory()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim inventory As Inventory = inventory.GetInventory(ID)
        inventory.Delete()

        inventory = inventory.Save()

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Delete Category"

    <Test()> _
    Public Sub DeleteCategory()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetCategory(ID)
        category.Delete()

        category = category.Save()

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Delete Profile"

    <Test()> _
    Public Sub DeleteProfile()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim profile As Profile = profile.GetProfile(NAME)
        profile.Delete()

        profile = profile.Save()

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region
#End Region
End Class