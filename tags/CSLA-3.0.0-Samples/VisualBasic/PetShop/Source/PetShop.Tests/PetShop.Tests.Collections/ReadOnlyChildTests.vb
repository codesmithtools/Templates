Imports System.Diagnostics
Imports NUnit.Framework
Imports PetShop.Tests.Collections.ReadOnlyChild

Namespace PetShop.Tests.Collections
    <TestFixture()> _
    Public Class ReadOnlyChildTests
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

        '[Test]
        'private void CreateCategory(string categoryID)
        '{
        '    Category category = Category.NewCategory();
        '    category.CategoryId = categoryID;
        '    category.Name = TestUtility.Instance.RandomString(80, false);
        '    category.Description = TestUtility.Instance.RandomString(255, false);

        '    Assert.IsTrue(category.IsValid, category.BrokenRulesCollection.ToString());
        '    category = category.Save();

        '    Assert.IsTrue(category.CategoryId == categoryID);
        '}

#End Region

        '''// <summary>
        '''// Inserts a Category entity into the database.
        '''// </summary>
        '[Test]
        'public void Step_01_Create_List()
        '{
        '    Console.WriteLine("1. Testing creating new list.");
        '    Stopwatch watch = Stopwatch.StartNew();

        '    CategoryList list = CategoryList.NewList();
        '    Assert.IsTrue(list.Count == 0);

        '    Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
        '}

        ''' <summary>
        ''' Selects all Category entitys.
        ''' </summary>
        <Test()> _
        Public Sub Step_02_GetByCategoryID()
            Console.WriteLine("2. Selects all Category entitys.")
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim list As CategoryInfoList = CategoryInfoList.GetByCategoryId(TestCategoryID)
            Assert.IsTrue(list.Count = 1)

            Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds)
        End Sub
    End Class
End Namespace
