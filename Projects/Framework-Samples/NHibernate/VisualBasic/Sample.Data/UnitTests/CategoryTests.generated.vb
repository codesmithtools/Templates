Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports Sample.Data.Generated.ManagerObjects
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.UnitTests
	<TestFixture()> _
	Public Partial Class CategoryTests
		Inherits UNuitTestBase
		
		Protected manager As ICategoryManager

		Public Sub New()
			manager = managerFactory.GetCategoryManager()
		End Sub

		Protected Function CreateNewCategory() As Category
			Dim entity As New Category()

			' You may need to maually enter this key if there is a constraint violation.
			entity.Id = "Test Test"
			
			entity.Name = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test"
			entity.Descn = "Test Test "

			Return entity
		End Function
		Protected Function GetFirstCategory() As Category
			Dim entityList As IList(Of Category) = manager.GetAll(1)
			If entityList.Count = 0 Then
				Assert.Fail("All tables must have at least one row for unit tests to succeed.")
			End If
			Return entityList(0)
		End Function

		<Test()> _
		Public Sub Create()
			Try
				Dim entity As Category = CreateNewCategory()

				Dim result As Object = manager.Save(entity)

				Assert.IsNotNull(result)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Read()
			Try
				Dim entityA As Category = CreateNewCategory()
				manager.Save(entityA)

				Dim entityB As Category = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA, entityB)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Update()
			Try
				Dim entityA As Category = GetFirstCategory()
				
				entityA.Name = "Test Test Test Test Test Test Test Test Test Test"
				
				manager.Update(entityA)

				Dim entityB As Category = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA.Name, entityB.Name)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Delete()
			Try
				Dim entity As Category = GetFirstCategory()

				manager.Delete(entity)

                entity = manager.GetById(entity.Id)
                Assert.IsNull(entity)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
	End Class
End Namespace

