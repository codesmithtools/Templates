Imports System
Imports System.Diagnostics
Imports NUnit.Framework
Imports PetShop.Core.Data
Imports System.Linq
Imports System.Data.Linq
'Imports PetShop.Core.Utility

Namespace PetShop.Test
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

            Dim profile = New Profile()
            profile.Username = NAME
            profile.ApplicationName = "PetShop..Businesss"
            profile.IsAnonymous = False
            profile.LastActivityDate = DateTime.Now
            profile.LastUpdatedDate = DateTime.Now

            Try
                Using context = New PetShopDataContext()
                    context.Profile.InsertOnSubmit(profile)
                    context.SubmitChanges()
                End Using
            Catch ex As Exception
                Assert.Fail(ex.Message)
            End Try

            Assert.IsTrue(True)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub FetchProfile()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim profile As Profile = Nothing
            Using context = New PetShopDataContext()
                profile = context.Profile.ByApplicationName(NAME).First()
                profile.Detach()
            End Using

            Assert.IsTrue(profile.Username = NAME)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub UpdateProfile()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim profile As Profile = Nothing
            Using context = New PetShopDataContext()
                profile = context.Profile.ByApplicationName(NAME).First()
                profile.Detach()
            End Using

            Using context = New PetShopDataContext()
                context.Profile.Attach(profile)
                profile.IsAnonymous = True
                context.SubmitChanges()
            End Using

            Using context = New PetShopDataContext()
                Assert.IsTrue(context.Profile.ByApplicationName(NAME).First().IsAnonymous.Value)
            End Using
            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#Region "Category"

        <Test()> _
        Public Sub CreateCategory()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim category = New Category()
            category.CategoryId = ID
            category.Name = ""
            category.Descn = ""

            Try
                Using context = New PetShopDataContext()
                    context.Category.InsertOnSubmit(category)
                    context.SubmitChanges()
                End Using
            Catch ex As Exception
                Assert.Fail(ex.Message)
            End Try

            Assert.IsTrue(True)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub FetchCategory()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim category As Category = Nothing
            Using context = New PetShopDataContext()
                category = context.Category.GetByKey(ID)
                category.Detach()
            End Using
            Assert.IsTrue(category.CategoryId = ID)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub UpdateCategory()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim category As Category = Nothing
            Using context = New PetShopDataContext()
                category = context.Category.GetByKey(ID)
                category.Detach()
            End Using

            Using context = New PetShopDataContext()
                context.Category.Attach(category)
                category.Descn = "This is a ."
                context.SubmitChanges()
            End Using

            Using context = New PetShopDataContext()
                Assert.IsTrue(context.Category.GetByKey(ID).Descn = "This is a .")
            End Using

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#Region "Inventory"

        <Test()> _
        Public Sub CreateInventory()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim inventory = New Inventory()
            inventory.ItemId = ID
            inventory.Qty = 10

            Try
                Using context = New PetShopDataContext()
                    context.Inventory.InsertOnSubmit(inventory)
                    context.SubmitChanges()
                End Using
            Catch ex As Exception
                Assert.Fail(ex.Message)
            End Try

            Assert.IsTrue(True)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub FetchInventory()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim inventory As Inventory = Nothing

            Using context = New PetShopDataContext()
                inventory = context.Inventory.GetByKey(ID)
                inventory.Detach()
            End Using

            Assert.IsTrue(inventory.ItemId = ID)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub UpdateInventory()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim inventory As Inventory = Nothing
            Using context = New PetShopDataContext()
                inventory = context.Inventory.GetByKey(ID)
                inventory.Detach()
            End Using

            Using context = New PetShopDataContext()
                context.Inventory.Attach(inventory)
                inventory.Qty = 100
                context.SubmitChanges()
            End Using

            Using context = New PetShopDataContext()
                Assert.IsTrue(context.Inventory.GetByKey(ID).Qty = 100)
            End Using

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#Region "Product"

        <Test()> _
        Public Sub CreateProduct()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim product = New Product()
            product.ProductId = ID
            product.CategoryId = ID
            product.Image = "/.png"
            product.Descn = ""
            product.Name = ""

            Try
                Using context = New PetShopDataContext()
                    context.Product.InsertOnSubmit(product)
                    context.SubmitChanges()
                End Using
            Catch ex As Exception
                Assert.Fail(ex.Message)
            End Try

            Assert.IsTrue(True)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub FetchProduct()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim product As Product = Nothing
            Using context = New PetShopDataContext()
                product = context.Product.GetByKey(ID)
                product.Detach()
            End Using

            Assert.IsTrue(product.ProductId = ID)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub UpdateProduct()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim product As Product = Nothing
            Using context = New PetShopDataContext()
                product = context.Product.GetByKey(ID)
                product.Detach()
            End Using

            Using context = New PetShopDataContext()
                context.Product.Attach(product)
                product.Descn = "This is a "
                context.SubmitChanges()
            End Using

            Using context = New PetShopDataContext()
                Assert.IsTrue(context.Product.GetByKey(ID).Descn = "This is a ")
            End Using

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#Region "Supplier"

        <Test()> _
        Public Sub CreateSupplier()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim supplier = New Supplier()
            supplier.Name = NAME
            supplier.Status = "AB"
            supplier.Addr1 = "One  Way"
            supplier.Addr2 = "Two  Way"
            supplier.City = "Dallas"
            supplier.State = "TX"
            supplier.Zip = "90210"
            supplier.Phone = "555-555-5555"

            Try
                Using context = New PetShopDataContext()
                    context.Supplier.InsertOnSubmit(supplier)
                    context.SubmitChanges()
                    supplier.Detach()
                    _supplierId = supplier.SuppId
                End Using
            Catch ex As Exception
                Assert.Fail(ex.Message)
            End Try

            Assert.IsTrue(True)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub FetchSupplier()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim supplier As Supplier = Nothing
            Using context = New PetShopDataContext()
                supplier = context.Supplier.GetByKey(_supplierId)
                supplier.Detach()
            End Using

            Assert.IsTrue(supplier.SuppId = _supplierId)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub UpdateSupplier()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim supplier As Supplier = Nothing
            Using context = New PetShopDataContext()
                supplier = context.Supplier.GetByKey(_supplierId)
                supplier.Detach()
            End Using

            Using context = New PetShopDataContext()
                context.Supplier.Attach(supplier)
                supplier.Phone = "111-111-1111"
                context.SubmitChanges()
            End Using

            Using context = New PetShopDataContext()
                Assert.IsTrue(context.Supplier.GetByKey(_supplierId).Phone = "111-111-1111")
            End Using

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#Region "Item"

        <Test()> _
        Public Sub CreateItem()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim item = New Item()
            item.ItemId = ID
            item.Image = "/.png"
            item.ListPrice = 0
            item.Name = ""
            item.ProductId = ID
            item.Status = ""
            item.Supplier = _supplierId
            item.UnitCost = 0

            Try
                Using context = New PetShopDataContext()
                    context.Item.InsertOnSubmit(item)
                    context.SubmitChanges()
                End Using
            Catch ex As Exception
                Assert.Fail(ex.Message)
            End Try

            Assert.IsTrue(True)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub FetchItem()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim item As Item = Nothing
            Using context = New PetShopDataContext()
                item = context.Item.GetByKey(ID)
                item.Detach()
            End Using

            Assert.IsTrue(item.ItemId = ID)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        <Test()> _
        Public Sub UpdateItem()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim item As Item = Nothing
            Using context = New PetShopDataContext()
                item = context.Item.GetByKey(ID)
                item.Detach()
            End Using

            Using context = New PetShopDataContext()
                context.Item.Attach(item)
                item.ListPrice = 111
                context.SubmitChanges()
            End Using

            Using context = New PetShopDataContext()
                Assert.IsTrue(context.Item.GetByKey(ID).ListPrice = 111)
            End Using

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#Region "Deletes"

#Region "Delete Item"

        <Test()> _
        Public Sub DeleteItem()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Using context = New PetshopDataContext()
                context.Item.Delete(ID)
            End Using

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#Region "Delete Supplier"

        <Test()> _
        Public Sub DeleteSupplier()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Using context = New PetshopDataContext()
                context.Supplier.Delete(_supplierId)
            End Using

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#Region "Delete Product"

        <Test()> _
        Public Sub DeleteProduct()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Using context = New PetshopDataContext()
                context.Product.Delete(ID)
            End Using

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#Region "Delete Inventory"

        <Test()> _
        Public Sub DeleteInventory()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Using context = New PetshopDataContext()
                context.Inventory.Delete(ID)
            End Using

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#Region "Delete Category"

        <Test()> _
        Public Sub DeleteCategory()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Using context = New PetshopDataContext()
                context.Category.Delete(ID)
            End Using

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#Region "Delete Profile"

        <Test()> _
        Public Sub DeleteProfile()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Using context = New PetshopDataContext()
                Dim profile = context.Profile.ByApplicationName(NAME).First()
                context.Profile.Delete(profile.UniqueID)
            End Using

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

#End Region

#End Region
    End Class
End Namespace
