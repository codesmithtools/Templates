Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports Sample.Data.Generated.ManagerObjects
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.UnitTests
	<TestFixture()> _
	Public Partial Class ItemTests
		Inherits UNuitTestBase
		
		Protected manager As IItemManager

		Public Sub New()
			manager = managerFactory.GetItemManager()
		End Sub

		Protected Function CreateNewItem() As Item
			Dim entity As New Item()

			' You may need to maually enter this key if there is a constraint violation.
			entity.Id = "Test "
			
			entity.ListPrice = 97
			entity.UnitCost = 92
			entity.Status = "T"
			entity.Name = "Test Test Test Test Test Test Test Test Test Test Test Test "
			entity.Image = "Test Test Te"
			
			Dim productManager As IProductManager = managerFactory.GetProductManager()
			entity.Product = productManager.GetAll(1)(0)
			
			Dim supplierManager As ISupplierManager = managerFactory.GetSupplierManager()
			entity.Supplier = supplierManager.GetAll(1)(0)

			Return entity
		End Function
		Protected Function GetFirstItem() As Item
			Dim entityList As IList(Of Item) = manager.GetAll(1)
			If entityList.Count = 0 Then
				Assert.Fail("All tables must have at least one row for unit tests to succeed.")
			End If
			Return entityList(0)
		End Function

		<Test()> _
		Public Sub Create()
			Try
				Dim entity As Item = CreateNewItem()

				Dim result As Object = manager.Save(entity)

				Assert.IsNotNull(result)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Read()
			Try
				Dim entityA As Item = CreateNewItem()
				manager.Save(entityA)

				Dim entityB As Item = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA, entityB)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Update()
			Try
				Dim entityA As Item = GetFirstItem()
				
				entityA.ListPrice = 9
				
				manager.Update(entityA)

				Dim entityB As Item = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA.ListPrice, entityB.ListPrice)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Delete()
			Try
				Dim entity As Item = GetFirstItem()

				manager.Delete(entity)

                entity = manager.GetById(entity.Id)
                Assert.IsNull(entity)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
	End Class
End Namespace

