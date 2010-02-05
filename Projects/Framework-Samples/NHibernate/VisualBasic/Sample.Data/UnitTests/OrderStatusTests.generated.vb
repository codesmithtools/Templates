Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports Sample.Data.Generated.ManagerObjects
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.UnitTests
	<TestFixture()> _
	Public Partial Class OrderStatusTests
		Inherits UNuitTestBase
		
		Protected manager As IOrderStatusManager

		
		<SetUp()> _
		Public Sub SetUp()
            manager = managerFactory.GetOrderStatusManager()
            manager.Session.BeginTransaction()
		End Sub
		<TearDown()> _
		Public Sub TearDown()
			manager.Session.RollbackTransaction()
            manager.Dispose()
		End Sub

		Protected Function CreateNewOrderStatus() As OrderStatus
			Dim entity As New OrderStatus()

			
			entity.OrderId = 59
			entity.LineNum = 85
			entity.Timestamp = DateTime.Now
			entity.Status = "T"

			Return entity
		End Function
		Protected Function GetFirstOrderStatus() As OrderStatus
			Dim entityList As IList(Of OrderStatus) = manager.GetAll(1)
			If entityList.Count = 0 Then
				Assert.Fail("All tables must have at least one row for unit tests to succeed.")
			End If
			Return entityList(0)
		End Function

		<Test()> _
		Public Sub Create()
			Try
				Dim entity As OrderStatus = CreateNewOrderStatus()

				Dim result As Object = manager.Save(entity)

				Assert.IsNotNull(result)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Read()
			Try
				Dim entityA As OrderStatus = CreateNewOrderStatus()
				manager.Save(entityA)

				Dim entityB As OrderStatus = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA, entityB)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Update()
			Try
				Dim entityA As OrderStatus = GetFirstOrderStatus()
				
				entityA.Timestamp = DateTime.Now
				
				manager.Update(entityA)

				Dim entityB As OrderStatus = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA.OrderId, entityB.OrderId)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Delete()
			Try
				Dim entity As OrderStatus = GetFirstOrderStatus()

				manager.Delete(entity)

                entity = manager.GetById(entity.Id)
                Assert.IsNull(entity)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
	End Class
End Namespace

