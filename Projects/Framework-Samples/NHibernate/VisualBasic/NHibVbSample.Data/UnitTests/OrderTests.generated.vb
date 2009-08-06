Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports NHibVbSample.Generated.ManagerObjects
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.UnitTests
	<TestFixture()> _
	Public Partial Class OrderTests
		Inherits UNuitTestBase
		
		Protected manager As IOrderManager

		Public Sub New()
			manager = managerFactory.GetOrderManager()
		End Sub

		Protected Function CreateNewOrder() As Order
			Dim entity As New Order()

			
			entity.UserId = "Test Test Test Te"
			entity.OrderDate = DateTime.Now
			entity.ShipAddr1 = "Test Test Test Test Test Test Test Test Test Test Te"
			entity.ShipAddr2 = "Test Test Test T"
			entity.ShipCity = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test T"
			entity.ShipState = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Te"
			entity.ShipZip = "Test Test Test Tes"
			entity.ShipCountry = "Test Test "
			entity.BillAddr1 = "Test Test Test Test Test Test Test Test Test Test "
			entity.BillAddr2 = "Test Test Test Test Test Test Test Te"
			entity.BillCity = "Test Test "
			entity.BillState = "Test Test Test Test Test Test Test Test Test Test Test Test Tes"
			entity.BillZip = "Test Test Test Tes"
			entity.BillCountry = "Test Tes"
			entity.Courier = "Test Test Test Test Test Test Test Test Test Test "
			entity.TotalPrice = 34
			entity.BillToFirstName = "Te"
			entity.BillToLastName = "Test Test Test Test Test Tes"
			entity.ShipToFirstName = "Test Test Test Test Test Test Test Test Test Test Test T"
			entity.ShipToLastName = "Test Test Test Test Test Test Test Test Test Test Te"
			entity.AuthorizationNumber = 9
			entity.Locale = "Test Test Tes"

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
				
				entityA.UserId = "Test Te"
				
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

