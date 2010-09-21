Imports System.Diagnostics
Imports NUnit.Framework
Imports PetShop.Tests.Collections.EditableRoot

Namespace PetShop.Tests.Collections
    <TestFixture()> _
    Public Class EditableRootTests
#Region "Setup"

        Private Shared ReadOnly Property TestCategoryID() As String
            Get
                Return "BIRDS"
            End Get
        End Property

        <NUnit.Framework.TestFixtureSetUp()> _
        Public Sub Init()
            System.Console.WriteLine(New [String]("-"c, 75))
            System.Console.WriteLine("-- Testing the Category Entity --")
            System.Console.WriteLine(New [String]("-"c, 75))
        End Sub

        <SetUp()> _
        Public Sub Setup()
            'TestCategoryID = TestUtility.Instance.RandomString(10, false);

            'CreateCategory(TestCategoryID);
        End Sub

        <TearDown()> _
        Public Sub TearDown()
            'try
            '{
            '    CategoryList list = CategoryList.GetByCategoryId(TestCategoryID);
            '    list.Clear(); 

            '    // Save the list..
            '}
            'catch (Exception) { }
        End Sub

        <NUnit.Framework.TestFixtureTearDown()> _
        Public Sub Complete()
            System.Console.WriteLine("All Tests Completed")
            System.Console.WriteLine()
        End Sub

        <Test()> _
        Private Sub CreateCategory(ByVal categoryID As String)
            Dim category__1 As Category = Category.NewCategory()
            category__1.CategoryId = categoryID
            category__1.Name = TestUtility.Instance.RandomString(80, False)
            category__1.Description = TestUtility.Instance.RandomString(255, False)

            Assert.IsTrue(category__1.IsValid, category__1.BrokenRulesCollection.ToString())
            category__1 = category__1.Save()

            Assert.IsTrue(category__1.CategoryId = categoryID)
        End Sub

#End Region

        ''' <summary>
        ''' Inserts a Category entity into the database.
        ''' </summary>
        <Test()> _
        Public Sub Step_01_Create_List()
            Console.WriteLine("1. Testing creating new list.")
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim list As CategoryList = CategoryList.NewList()
            Assert.IsTrue(list.Count = 0)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub

        ''' <summary>
        ''' Selects all Category entitys.
        ''' </summary>
        <Test()> _
        Public Sub Step_02_GetByCategoryID()
            Console.WriteLine("2. Selects all Category entitys.")
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim list As CategoryList = CategoryList.GetByCategoryId(TestCategoryID)
            Assert.IsTrue(list.Count = 1)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub
    End Class
End Namespace