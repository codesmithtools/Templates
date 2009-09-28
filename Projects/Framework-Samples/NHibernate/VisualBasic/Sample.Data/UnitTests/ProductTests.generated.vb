Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports Sample.Data.Generated.ManagerObjects
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.UnitTests
	<TestFixture()> _
	Public Partial Class ProductTests
		Inherits UNuitTestBase
		
		Protected manager As IProductManager

		Public Sub New()
			manager = managerFactory.GetProductManager()
		End Sub

		Protected Function CreateNewProduct() As Product
			Dim entity As New Product()

			' You may need to maually enter this key if there is a constraint violation.
			entity.Id = "Test T"
			
			entity.Name = "Test Test Test"
			entity.Descn = "Test Test "
			entity.Image = "Test Test Test Test Test Test Te"
			
			Dim categoryManager As ICategoryManager = managerFactory.GetCategoryManager()
			entity.Category = categoryManager.GetAll(1)(0)

			Return entity
		End Function
		Protected Function GetFirstProduct() As Product
			Dim entityList As IList(Of Product) = manager.GetAll(1)
			If entityList.Count = 0 Then
				Assert.Fail("All tables must have at least one row for unit tests to succeed.")
			End If
			Return entityList(0)
		End Function

		<Test()> _
		Public Sub Create()
			Try
				Dim entity As Product = CreateNewProduct()

				Dim result As Object = manager.Save(entity)

				Assert.IsNotNull(result)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Read()
			Try
				Dim entityA As Product = CreateNewProduct()
				manager.Save(entityA)

				Dim entityB As Product = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA, entityB)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Update()
			Try
				Dim entityA As Product = GetFirstProduct()
				
				entityA.Name = "Test Test Test Test Test Test Test Test Te"
				
				manager.Update(entityA)

				Dim entityB As Product = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA.Name, entityB.Name)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Delete()
			Try
				Dim entity As Product = GetFirstProduct()

				manager.Delete(entity)

                entity = manager.GetById(entity.Id)
                Assert.IsNull(entity)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
	End Class
End Namespace

