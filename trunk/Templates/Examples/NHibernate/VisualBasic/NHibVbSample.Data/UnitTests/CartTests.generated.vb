Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports NHibVbSample.Generated.ManagerObjects
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.UnitTests
	<TestFixture()> _
	Public Partial Class CartTests
		Inherits UNuitTestBase
		
		Protected manager As ICartManager

		Public Sub New()
			manager = managerFactory.GetCartManager()
		End Sub

		Protected Function CreateNewCart() As Cart
			Dim entity As New Cart()

			
			entity.ItemId = "Test Test"
			entity.Name = "Test Test Test Test Test Test Test Test Test Test Test"
			entity.Type = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test"
			entity.Price = 4
			entity.CategoryId = "Test Te"
			entity.ProductId = "Test "
			entity.IsShoppingCart = True
			entity.Quantity = 40
			
			Dim profileManager As IProfileManager = managerFactory.GetProfileManager()
			entity.Profile = profileManager.GetAll(1)(0)

			Return entity
		End Function
		Protected Function GetFirstCart() As Cart
			Dim entityList As IList(Of Cart) = manager.GetAll(1)
			If entityList.Count = 0 Then
				Assert.Fail("All tables must have at least one row for unit tests to succeed.")
			End If
			Return entityList(0)
		End Function

		<Test()> _
		Public Sub Create()
			Try
				Dim entity As Cart = CreateNewCart()

				Dim result As Object = manager.Save(entity)

				Assert.IsNotNull(result)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Read()
			Try
				Dim entityA As Cart = CreateNewCart()
				manager.Save(entityA)

				Dim entityB As Cart = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA, entityB)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Update()
			Try
				Dim entityA As Cart = GetFirstCart()
				
				entityA.ItemId = "Tes"
				
				manager.Update(entityA)

				Dim entityB As Cart = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA.ItemId, entityB.ItemId)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Delete()
			Try
				Dim entity As Cart = GetFirstCart()

				manager.Delete(entity)

                entity = manager.GetById(entity.Id)
                Assert.IsNull(entity)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
	End Class
End Namespace

