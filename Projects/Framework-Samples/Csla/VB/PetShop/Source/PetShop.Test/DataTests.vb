Imports System
Imports System.Diagnostics
Imports Csla.Data
Imports PetShop.Data

Imports NUnit.Framework

Imports PetShop.Business

<TestFixture()> _
Public Class DataTests
    <SetUp()> _
    Public Sub Setup()
    End Sub

    <TearDown()> _
    Public Sub TearDown()
    End Sub

#Region "Item"

    <Test()> _
    Public Sub ItemInsert()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            DataAccessLayer.Instance.ItemInsert("EST-86", 1, 5, "P", "New Item Test", "", "BG-03", 1).Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Assert.IsTrue(True)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub ItemFetch()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            Dim item As SafeDataReader = DataAccessLayer.Instance.ItemFetch(New ItemCriteria("EST-86").StateBag)
            Assert.IsTrue(item.FieldCount > 0)

            item.Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub ItemUpdate()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            DataAccessLayer.Instance.ItemUpdate("EST-86", 5, 1, "P", "New Item Test Updated", "", _
                                                "BG-03", 1).Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub ItemDelete()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            DataAccessLayer.Instance.ItemDelete(New ItemCriteria("EST-86").StateBag).Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Product"

    <Test()> _
    Public Sub ProductInsert()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            DataAccessLayer.Instance.ProductInsert("BLAKE-0", "Blake", "Blake Niemyjski", "", "EDANGER").Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Assert.IsTrue(True)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub ProductFetch()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            Dim product As SafeDataReader = DataAccessLayer.Instance.ProductFetch(New ProductCriteria("BLAKE-0").StateBag)
            Assert.IsTrue(product.FieldCount > 0)

            product.Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub ProductUpdate()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            DataAccessLayer.Instance.ProductUpdate("BLAKE-0", "Blake", "Blake A. Niemyjski", "", "EDANGER").Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub ProductDelete()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            DataAccessLayer.Instance.ProductDelete(New ProductCriteria("BLAKE-0").StateBag).Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region

#Region "Profile"

    Private _profileUniqueID As Integer = 0

    <Test()> _
    Public Sub ProfileInsert()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            Dim data As SafeDataReader = DataAccessLayer.Instance.ProfileInsert("BLAKE-0", "Blake", False, DateTime.Now, DateTime.Now)

            data.Read()

            _profileUniqueID = data.GetInt32("UniqueID")

            data.Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Assert.IsTrue(True)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub ProfileFetch()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            Dim profile As SafeDataReader = DataAccessLayer.Instance.ProfileFetch(New ProfileCriteria().StateBag)
            Assert.IsTrue(profile.FieldCount > 0)

            profile.Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub ProfileUpdate()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            DataAccessLayer.Instance.ProfileUpdate(_profileUniqueID, "BLAKE-0", "Blake", True, DateTime.Now, DateTime.Now).Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    <Test()> _
    Public Sub ProfileDelete()
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            DataAccessLayer.Instance.ProfileDelete(New ProfileCriteria().StateBag).Close()
        Catch ex As Exception
            Assert.Fail(ex.Message)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
#End Region
End Class