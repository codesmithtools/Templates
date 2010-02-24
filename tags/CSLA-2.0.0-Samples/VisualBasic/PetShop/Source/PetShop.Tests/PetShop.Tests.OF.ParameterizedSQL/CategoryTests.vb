Imports System
Imports System.Diagnostics
Imports NUnit.Framework

<TestFixture()> _
Public Class CategoryTests

#Region "Setup"

    Private _TestCategoryID As String
    Private Property TestCategoryID() As String
        Get
            Return _TestCategoryID
        End Get
        Set(ByVal value As String)
            _TestCategoryID = value
        End Set
    End Property
    Private _TestCategoryID2 As String
    Private Property TestCategoryID2() As String
        Get
            Return _TestCategoryID2
        End Get
        Set(ByVal value As String)
            _TestCategoryID2 = value
        End Set
    End Property
    Private _TestProductID As String
    Private Property TestProductID() As String
        Get
            Return _TestProductID
        End Get
        Set(ByVal value As String)
            _TestProductID = value
        End Set
    End Property
    Private _TestProductID2 As String
    Private Property TestProductID2() As String
        Get
            Return _TestProductID2
        End Get
        Set(ByVal value As String)
            _TestProductID2 = value
        End Set
    End Property

    <NUnit.Framework.TestFixtureSetUp()> _
    Public Sub Init()
        System.Console.WriteLine(New [String]("-"c, 75))
        System.Console.WriteLine("-- Testing the Category Entity --")
        System.Console.WriteLine(New [String]("-"c, 75))
    End Sub

    <SetUp()> _
    Public Sub Setup()
        TestCategoryID = TestUtility.Instance.RandomString(10, False)
        TestCategoryID2 = TestUtility.Instance.RandomString(10, False)
        TestProductID = TestUtility.Instance.RandomString(10, False)
        TestProductID2 = TestUtility.Instance.RandomString(10, False)

        CreateCategory(TestCategoryID)
    End Sub

    <TearDown()> _
    Public Sub TearDown()
        Try
            Product.DeleteProduct(TestProductID2)
        Catch generatedExceptionName As Exception
        End Try
        Try
            Product.DeleteProduct(TestProductID)
        Catch generatedExceptionName As Exception
        End Try

        Try
            Category.DeleteCategory(TestCategoryID)
        Catch generatedExceptionName As Exception
        End Try

        Try
            Category.DeleteCategory(TestCategoryID2)
        Catch generatedExceptionName As Exception
        End Try
    End Sub

    <NUnit.Framework.TestFixtureTearDown()> _
    Public Sub Complete()
        System.Console.WriteLine("All Tests Completed")
        System.Console.WriteLine()
    End Sub

    <Test()> _
    Private Sub CreateCategory(ByVal categoryID As String)
        Dim category As Category = PetShop.Tests.OF.ParameterizedSQL.Category.NewCategory()
        category.CategoryId = categoryID
        category.Name = TestUtility.Instance.RandomString(80, False)
        category.Description = TestUtility.Instance.RandomString(255, False)

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Assert.IsTrue(category.CategoryId = categoryID)
    End Sub

#End Region

    ''' <summary>
    ''' Inserts a Category entity into the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_01_Insert_Duplicate()
        Console.WriteLine("1. Testing Duplicate Records.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        'Insert should fail as there should be a duplicate key.
        Dim category As Category = PetShop.Tests.OF.ParameterizedSQL.Category.NewCategory()
        category.CategoryId = TestCategoryID
        category.Name = TestUtility.Instance.RandomString(80, False)
        category.Description = TestUtility.Instance.RandomString(255, False)

        Try
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
            category = category.Save()

            ' Fail as a duplicate record was entered.
            Assert.Fail("Fail as a duplicate record was entered and an exception was not thrown.")
        Catch generatedExceptionName As Exception
            Assert.IsTrue(True)
        End Try
        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Inserts a Category entity into the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_02_Insert()
        Console.WriteLine("2. Inserting new category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = PetShop.Tests.OF.ParameterizedSQL.Category.NewCategory()
        category.CategoryId = TestCategoryID2
        category.Name = TestUtility.Instance.RandomString(80, False)
        category.Description = TestUtility.Instance.RandomString(255, False)

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Assert.IsTrue(True)
        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Selects a sample of Category objects of the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_03_SelectAll()
        Console.WriteLine("3. Selecting all categories by calling GetByCategoryId(""{0}"").", TestCategoryID)
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim list As CategoryList = CategoryList.GetByCategoryId(TestCategoryID)
        Assert.IsTrue(list.Count = 1)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Updates a Category entity into the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_04_Update()
        Console.WriteLine("4. Updating the category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetByCategoryId(TestCategoryID)
        Dim name = category.Name
        Dim desc = category.Description

        category.Name = TestUtility.Instance.RandomString(80, False)
        category.Description = TestUtility.Instance.RandomString(255, False)

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Assert.IsFalse(String.Equals(category.Name, name, StringComparison.InvariantCultureIgnoreCase))
        Assert.IsFalse(String.Equals(category.Description, desc, StringComparison.InvariantCultureIgnoreCase))

        category.Name = name
        category.Description = desc

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Assert.IsTrue(String.Equals(category.Name, name, StringComparison.InvariantCultureIgnoreCase))
        Assert.IsTrue(String.Equals(category.Description, desc, StringComparison.InvariantCultureIgnoreCase))

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Delete the  Category entity into the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_05_Delete()
        Console.WriteLine("5. Deleting the category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetByCategoryId(TestCategoryID)
        category.Delete()

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Assert.IsTrue(category.IsNew)
        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Updates a non Identity PK column.
    ''' </summary>
    <Test()> _
    Public Sub Step_06_UpdatePrimaryKey()
        Console.WriteLine("6. Updating the non Identity Primary Key.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Console.WriteLine(vbTab & "Getting category ""{0}""", TestCategoryID)
        Dim category As Category = category.GetByCategoryId(TestCategoryID)
        category.CategoryId = TestCategoryID2

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()
        Console.WriteLine(vbTab & "Set categoryID to ""{0}""", TestCategoryID2)
        Assert.IsTrue(category.CategoryId = TestCategoryID2)

        Try
            category.GetByCategoryId(TestCategoryID)
            Assert.Fail("Record exists when it should have been updated.")
        Catch generatedExceptionName As Exception
            Assert.IsTrue(True)
        End Try

        Dim validCategory As Category = category.GetByCategoryId(TestCategoryID2)
        Assert.IsTrue(validCategory.CategoryId = TestCategoryID2)
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

        Dim category As Category = PetShop.Tests.OF.ParameterizedSQL.Category.NewCategory()
        Assert.IsFalse(category.IsValid)

        category.CategoryId = TestCategoryID
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())

        category.Name = TestUtility.Instance.RandomString(80, False)
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())

        category.Description = TestUtility.Instance.RandomString(255, False)
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())

        ' Check Category.
        category.CategoryId = Nothing
        Assert.IsFalse(category.IsValid)

        category.CategoryId = TestUtility.Instance.RandomString(11, False)
        Assert.IsFalse(category.IsValid)

        category.CategoryId = TestCategoryID
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())

        ' Check Name.
        category.Name = Nothing
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())

        category.Name = TestUtility.Instance.RandomString(81, False)
        Assert.IsFalse(category.IsValid)

        category.Name = TestUtility.Instance.RandomString(80, False)
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())

        ' Check Descn.
        category.Description = Nothing
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())

        category.Description = TestUtility.Instance.RandomString(256, False)
        Assert.IsFalse(category.IsValid)

        category.Description = TestUtility.Instance.RandomString(80, False)
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Inserts a Category entity into the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_08_Entity_State()
        Console.WriteLine("8. Checking the state of the entity.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        'Insert should fail as there should be a duplicate key.
        Dim category As Category = PetShop.Tests.OF.ParameterizedSQL.Category.NewCategory()
        Assert.IsTrue(category.IsDirty)

        category.CategoryId = TestCategoryID2
        category.Name = TestUtility.Instance.RandomString(80, False)
        category.Description = TestUtility.Instance.RandomString(255, False)

        Assert.IsTrue(category.IsNew)
        Assert.IsTrue(category.IsDirty)
        Assert.IsFalse(category.IsDeleted)

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Assert.IsFalse(category.IsNew)
        Assert.IsFalse(category.IsDirty)
        Assert.IsFalse(category.IsDeleted)

        category.Name = TestUtility.Instance.RandomString(80, False)
        Assert.IsTrue(category.IsDirty)
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Assert.IsFalse(category.IsNew)
        Assert.IsFalse(category.IsDirty)
        Assert.IsFalse(category.IsDeleted)

        category.Delete()
        Assert.IsTrue(category.IsDeleted)
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()
        Assert.IsFalse(category.IsDeleted)
        Assert.IsTrue(category.IsNew)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Testing Child collections on a new entity.
    ''' </summary>
    <Test()> _
    Public Sub Step_09_New_Entity_Collection()
        Console.WriteLine("9. Testing child collections on a new category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = PetShop.Tests.OF.ParameterizedSQL.Category.NewCategory()
        Dim count = category.Products.Count

        Assert.IsTrue(count = 0)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Testing Child collections on a new entity.
    ''' </summary>
    <Test()> _
    Public Sub Step_10_Existing_Entity_With_No_Collection()
        Console.WriteLine("10. Testing child collections on a category with no child collection items.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetByCategoryId(TestCategoryID)
        Dim count = category.Products.Count

        Assert.IsTrue(count = 0)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Selects a sample of Category objects of the database using an invalid category ID.
    ''' </summary>
    <Test()> _
    Public Sub Step_12_SelectAll_Invalid_Value()
        Console.WriteLine("12. Selecting all categories by calling GetByCategoryId with an invalid value")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            CategoryList.GetByCategoryId(TestUtility.Instance.RandomString(10, False))

            Assert.Fail("This call to GetByCategoryID should throw an exception")
        Catch generatedExceptionName As Exception
            Assert.IsTrue(True)
        End Try
        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Selects a sample of Category objects of the database using an invalid category ID.
    ''' </summary>
    <Test()> _
    Public Sub Step_13_SelectAll_With_Oversized_CategoryID()
        Console.WriteLine("13. Selecting all categories by calling GetByCategoryId with an invalid value")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            CategoryList.GetByCategoryId(TestCategoryID & "a")

            Assert.Fail("This call to GetByCategoryID should throw an exception")
        Catch generatedExceptionName As Exception
            Assert.IsTrue(True)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Testing Insert Child collections on a new entity.
    ''' </summary>
    <Test()> _
    Public Sub Step_14_Insert_Child_Collection_On_New_Category()
        Console.WriteLine("14. Testing insert on child collections in a new category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = PetShop.Tests.OF.ParameterizedSQL.Category.NewCategory()
        category.CategoryId = TestCategoryID2
        category.Name = TestUtility.Instance.RandomString(80, False)
        category.Description = TestUtility.Instance.RandomString(255, False)

        Assert.IsTrue(category.Products.Count = 0)
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())

        Dim product As Product = category.Products.AddNew()

        Assert.IsTrue(category.Products.Count = 1)

        product.ProductId = TestProductID
        product.Name = TestUtility.Instance.RandomString(80, False)
        product.Description = TestUtility.Instance.RandomString(255, False)
        product.Image = TestUtility.Instance.RandomString(80, False)
        Assert.IsTrue(product.IsValid, product.BrokenRulesCollection.ToString())

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Dim category2 As Category = category.GetByCategoryId(TestCategoryID2)

        Assert.IsTrue(category.Products.Count = category2.Products.Count)
        Assert.IsTrue(category.CategoryId = category2.Products(0).CategoryId)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Testing Insert Child collections on an existing entity.
    ''' </summary>
    <Test()> _
    Public Sub Step_15_Insert_Child_Collection()
        Console.WriteLine("15. Testing insert on child collections in a category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetByCategoryId(TestCategoryID)
        Assert.IsTrue(category.Products.Count = 0)

        Dim product As Product = category.Products.AddNew()

        Assert.IsTrue(category.Products.Count = 1)

        product.ProductId = TestProductID
        product.Name = TestUtility.Instance.RandomString(80, False)
        product.Description = TestUtility.Instance.RandomString(255, False)
        product.Image = TestUtility.Instance.RandomString(80, False)

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Dim category2 As Category = category.GetByCategoryId(TestCategoryID)

        Assert.IsTrue(category.Products.Count = category2.Products.Count)
        Assert.IsTrue(category.CategoryId = category2.Products(0).CategoryId)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Testing update on a new child collections on a new entity.
    ''' </summary>
    <Test()> _
    Public Sub Step_16_Update_Child_Collection_On_New_Category()
        Console.WriteLine("16. Testing update on new child collections in a new category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = PetShop.Tests.OF.ParameterizedSQL.Category.NewCategory()
        category.CategoryId = TestCategoryID2
        category.Name = TestUtility.Instance.RandomString(80, False)
        category.Description = TestUtility.Instance.RandomString(255, False)

        Assert.IsTrue(category.Products.Count = 0)
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())

        Dim product As Product = category.Products.AddNew()

        Assert.IsTrue(category.Products.Count = 1)

        product.ProductId = TestProductID
        product.Name = TestUtility.Instance.RandomString(80, False)
        product.Description = TestUtility.Instance.RandomString(255, False)
        product.Image = TestUtility.Instance.RandomString(80, False)

        Dim newName = TestUtility.Instance.RandomString(80, False)
        For Each item As Product In category.Products
            item.Name = newName
        Next

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Dim category2 As Category = category.GetByCategoryId(TestCategoryID2)
        Assert.IsTrue(String.Equals(category2.Products(0).Name, newName, StringComparison.InvariantCultureIgnoreCase))

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Testing update Child collections on an existing entity.
    ''' </summary>
    <Test()> _
    Public Sub Step_17_Update_Child_Collection()
        Console.WriteLine("17. Testing update on child collections in a category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetByCategoryId(TestCategoryID)
        Assert.IsTrue(category.Products.Count = 0)

        Dim product As Product = category.Products.AddNew()

        Assert.IsTrue(category.Products.Count = 1)

        product.ProductId = TestProductID
        product.Name = TestUtility.Instance.RandomString(80, False)
        product.Description = TestUtility.Instance.RandomString(255, False)
        product.Image = TestUtility.Instance.RandomString(80, False)

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Dim newName = TestUtility.Instance.RandomString(80, False)
        For Each item As Product In category.Products
            item.Name = newName
        Next

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Dim category2 As Category = category.GetByCategoryId(TestCategoryID)
        Dim list As ProductList = category2.Products

        Assert.IsTrue(String.Equals(category2.Products(0).Name, newName, StringComparison.InvariantCultureIgnoreCase))

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Testing update Child collections on an existing entity.
    ''' </summary>
    <Test()> _
    Public Sub Step_18_Update_PK_Of_Child_Collection()
        Console.WriteLine("18. Testing update of PK on child collections in a category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetByCategoryId(TestCategoryID)
        Assert.IsTrue(category.Products.Count = 0)

        Dim product As Product = category.Products.AddNew()

        Assert.IsTrue(category.Products.Count = 1)

        product.ProductId = TestProductID
        product.Name = TestUtility.Instance.RandomString(80, False)
        product.Description = TestUtility.Instance.RandomString(255, False)
        product.Image = TestUtility.Instance.RandomString(80, False)

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        category.CategoryId = TestCategoryID2

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Dim category2 As Category = category.GetByCategoryId(TestCategoryID2)
        Assert.IsTrue(category2.Products.Count = 1)
        Assert.IsTrue(String.Equals(category2.Products(0).CategoryId, category.CategoryId, StringComparison.InvariantCultureIgnoreCase))

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Testing update of Every PK in a Child collections on an existing entity.
    ''' </summary>
    <Test()> _
    Public Sub Step_19_Update_Every_PK_In_Child_Collection()
        Console.WriteLine("19. Testing update of every PK in a child collections in a category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetByCategoryId(TestCategoryID)
        Assert.IsTrue(category.Products.Count = 0)

        Dim product As Product = category.Products.AddNew()

        Assert.IsTrue(category.Products.Count = 1)

        product.ProductId = TestProductID
        product.Name = TestUtility.Instance.RandomString(80, False)
        product.Description = TestUtility.Instance.RandomString(255, False)
        product.Image = TestUtility.Instance.RandomString(80, False)

        Dim product2 As Product = category.Products.AddNew()

        Assert.IsTrue(category.Products.Count = 2)

        product2.ProductId = TestProductID2
        product2.Name = TestUtility.Instance.RandomString(80, False)
        product2.Description = TestUtility.Instance.RandomString(255, False)
        product2.Image = TestUtility.Instance.RandomString(80, False)

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Dim newName = TestUtility.Instance.RandomString(80, False)
        For Each item As Product In category.Products
            item.Name = newName
        Next

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Dim category2 As Category = category.GetByCategoryId(TestCategoryID)
        Assert.IsTrue(category2.Products.Count = 2)
        Assert.IsTrue(String.Equals(category2.Products(0).Name, newName, StringComparison.InvariantCultureIgnoreCase))
        Assert.IsTrue(String.Equals(category2.Products(1).Name, newName, StringComparison.InvariantCultureIgnoreCase))

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub


    ''' <summary>
    ''' Testing insert of a duplicate Product into a child collections.
    ''' </summary>
    <Test()> _
    Public Sub Step_20_Insert_Duplicate_Item_In_Child_Collection()
        Console.WriteLine("20. Testing insert of a duplicate Product into a child collections.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetByCategoryId(TestCategoryID)
        Assert.IsTrue(category.Products.Count = 0)

        Dim product As Product = category.Products.AddNew()

        Assert.IsTrue(category.Products.Count = 1)

        product.ProductId = TestProductID
        product.Name = TestUtility.Instance.RandomString(80, False)
        product.Description = TestUtility.Instance.RandomString(255, False)
        product.Image = TestUtility.Instance.RandomString(80, False)

        Dim product2 As Product = category.Products.AddNew()

        Assert.IsTrue(category.Products.Count = 2)

        product2.ProductId = TestProductID
        product2.Name = TestUtility.Instance.RandomString(80, False)
        product2.Description = TestUtility.Instance.RandomString(255, False)
        product2.Image = TestUtility.Instance.RandomString(80, False)

        Try
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
            category = category.Save()
            Assert.Fail("Should throw a duplicate entry exception.")
        Catch generatedExceptionName As Exception
            Assert.IsTrue(True)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Delete the Category entity into the database twice.
    ''' </summary>
    <Test()> _
    Public Sub Step_21_Double_Delete()
        Console.WriteLine("20. Deleting the category twice.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetByCategoryId(TestCategoryID)
        Assert.IsTrue(String.Equals(category.CategoryId, TestCategoryID, StringComparison.InvariantCultureIgnoreCase))
        Assert.IsFalse(category.IsDeleted)
        category.Delete()

        Assert.IsTrue(category.IsDeleted)

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Assert.IsTrue(String.Equals(category.CategoryId, TestCategoryID, StringComparison.InvariantCultureIgnoreCase))
        Assert.IsFalse(category.IsDeleted)

        category.Delete()

        ' Shouldn't be able to call delete twice. I'd think.
        Assert.IsTrue(category.IsDeleted)
        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Assert.IsTrue(String.Equals(category.CategoryId, TestCategoryID, StringComparison.InvariantCultureIgnoreCase))
        Assert.IsTrue(category.IsNew)
        Assert.IsFalse(category.IsDeleted)

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Delete the Category entity into the database twice.
    ''' </summary>
    <Test()> _
    Public Sub Step_22_Double_Delete()
        Console.WriteLine("20. Deleting the category twice.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = category.GetByCategoryId(TestCategoryID)
        Assert.IsTrue(String.Equals(category.CategoryId, TestCategoryID, StringComparison.InvariantCultureIgnoreCase))
        Assert.IsFalse(category.IsDeleted)
        category.Delete()

        'Delete the category the other way before calling save which would call a delete. This should simulate a concurency issue.
        category.DeleteCategory(TestCategoryID)
        Try
            category.GetByCategoryId(TestCategoryID)
            Assert.Fail("Record exists when it should have been deleted.")
        Catch generatedExceptionName As Exception
            ' Exception was thrown if record doesn't exist.
            Assert.IsTrue(True)
        End Try

        ' Call delete on an already deleted item.
        Assert.IsTrue(category.IsDeleted)

        Try
            Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
            category = category.Save()
            Assert.Fail("Record exists when it should have been deleted.")
        Catch generatedExceptionName As Exception
            ' Exception was thrown if record doesn't exist.
            Assert.IsTrue(True)
        End Try

        Assert.IsTrue(String.Equals(category.CategoryId, TestCategoryID, StringComparison.InvariantCultureIgnoreCase))
        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub

    ''' <summary>
    ''' Updates a Category entity into the database.
    ''' </summary>
    <Test()> _
    Public Sub Step_23_Concurrency_Duplicate_Update()
        Console.WriteLine("23. Updating the category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = Category.GetByCategoryId(TestCategoryID)
        Dim name As String = category.Name
        Dim desc As String = category.Description

        category.Name = TestUtility.Instance.RandomString(80, False)
        category.Description = TestUtility.Instance.RandomString(255, False)

        Dim category2 As Category = Category.GetByCategoryId(TestCategoryID)

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Assert.IsFalse(String.Equals(category.Name, name, StringComparison.InvariantCultureIgnoreCase))
        Assert.IsFalse(String.Equals(category.Description, desc, StringComparison.InvariantCultureIgnoreCase))

        category2.Name = TestUtility.Instance.RandomString(80, False)
        category2.Description = TestUtility.Instance.RandomString(255, False)

        Try
            category2 = category2.Save()
            Assert.Fail("Concurrency exception should have been thrown.")
        Catch generatedExceptionName As Exception
            Assert.IsTrue(True)
        End Try

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub


    ''' <summary>
    ''' Testing update Child collections on an existing entity.
    ''' </summary>
    <Test()> _
    Public Sub Step_24_Concurrency_Update_Children()
        Console.WriteLine("24. Testing update on child collections in a category.")
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim category As Category = Category.GetByCategoryId(TestCategoryID)
        Assert.IsTrue(category.Products.Count = 0)

        Dim product As Product = category.Products.AddNew()

        Assert.IsTrue(category.Products.Count = 1)

        product.ProductId = TestProductID
        product.Name = TestUtility.Instance.RandomString(80, False)
        product.Description = TestUtility.Instance.RandomString(255, False)
        product.Image = TestUtility.Instance.RandomString(80, False)

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Dim category2 As Category = Category.GetByCategoryId(TestCategoryID)

        Dim newName As String = TestUtility.Instance.RandomString(80, False)
        For Each item As Product In category.Products
            item.Name = newName
        Next

        For Each item As Product In category2.Products
            item.Name = newName
        Next

        Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString())
        category = category.Save()

        Try
            category2 = category2.Save()
            Assert.Fail("Concurrency exception should have been thrown.")
        Catch generatedExceptionName As Exception
            Assert.IsTrue(True)
        End Try

        Assert.IsTrue(String.Equals(category2.Products(0).Name, newName, StringComparison.InvariantCultureIgnoreCase))

        Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
    End Sub
End Class
