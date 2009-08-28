Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports Sample.Data.Generated.ManagerObjects
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.UnitTests
	<TestFixture()> _
	Public Partial Class OrderTests
		Inherits UNuitTestBase
		
		Protected manager As IOrderManager

		Public Sub New()
			manager = managerFactory.GetOrderManager()
		End Sub

		Protected Function CreateNewOrder() As Order
			Dim entity As New Order()

			
			entity.UserId = "Te"
			entity.OrderDate = DateTime.Now
			entity.ShipAddr1 = "Test Test Test Test Test Test Test Test Test"
			entity.ShipAddr2 = "Test Test Test Te"
			entity.ShipCity = "Test T"
			entity.ShipState = "Test Test Test Test Test Test Test Test Test Test Test Test Test Tes"
			entity.ShipZip = "Test T"
			entity.ShipCountry = "Test Test Test "
			entity.BillAddr1 = "Test Te"
			entity.BillAddr2 = "Test Test Test Test Test Test Te"
			entity.BillCity = "Test Test Test Test Test Test Test Test Test Test Test "
			entity.BillState = "Test Test Test Test Test Test Test Test Test Test Test Test"
			entity.BillZip = "Test Test Test Te"
			entity.BillCountry = "Test Tes"
			entity.Courier = "Test Test Test Test Test Test"
			entity.TotalPrice = 87
			entity.BillToFirstName = "Test Test Test Test Test Test Test Test Test Test Test Test T"
			entity.BillToLastName = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test"
			entity.ShipToFirstName = "Test Test Test Test Test Test "
			entity.ShipToLastName = "Test Test Test Test Test Test Test Test Test Test Test Tes"
			entity.AuthorizationNumber = 64
			entity.Locale = "Test Test"

			Return entity
		End Function
		Protected Function GetFirstOrder() As Order
			Dim entityList As IList(Of Order) = manager.GetAll(1)
			If entityList.Count = 0 Then
				Assert.Fail("All tables must have at least one row for unit tests to succeed.")
			End If
			Return entityList(0)
		End Function

		<Test()> _
		Public Sub Create()
			Try
				Dim entity As Order = CreateNewOrder()

				Dim result As Object = manager.Save(entity)

				Assert.IsNotNull(result)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Read()
			Try
				Dim entityA As Order = CreateNewOrder()
				manager.Save(entityA)

				Dim entityB As Order = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA, entityB)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Update()
			Try
				Dim entityA As Order = GetFirstOrder()
				
				entityA.UserId = "Test"
				
				manager.Update(entityA)

				Dim entityB As Order = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA.UserId, entityB.UserId)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Delete()
			Try
				Dim entity As Order = GetFirstOrder()

				manager.Delete(entity)

                entity = manager.GetById(entity.Id)
                Assert.IsNull(entity)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
	End Class
End Namespace

