Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports Sample.Data.Generated.ManagerObjects
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.UnitTests
	<TestFixture()> _
	Public Partial Class LineItemTests
		Inherits UNuitTestBase
		
		Protected manager As ILineItemManager

		
		<SetUp()> _
		Public Sub SetUp()
            manager = managerFactory.GetLineItemManager()
            manager.Session.BeginTransaction()
		End Sub
		<TearDown()> _
		Public Sub TearDown()
			manager.Session.RollbackTransaction()
            manager.Dispose()
		End Sub

		Protected Function CreateNewLineItem() As LineItem
			Dim entity As New LineItem()

			
			entity.OrderId = 65
			entity.LineNum = 77
			entity.ItemId = "Test Test"
			entity.Quantity = 74
			entity.UnitPrice = 33

			Return entity
		End Function
		Protected Function GetFirstLineItem() As LineItem
			Dim entityList As IList(Of LineItem) = manager.GetAll(1)
			If entityList.Count = 0 Then
				Assert.Fail("All tables must have at least one row for unit tests to succeed.")
			End If
			Return entityList(0)
		End Function

		<Test()> _
		Public Sub Create()
			Try
				Dim entity As LineItem = CreateNewLineItem()

				Dim result As Object = manager.Save(entity)

				Assert.IsNotNull(result)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Read()
			Try
				Dim entityA As LineItem = CreateNewLineItem()
				manager.Save(entityA)

				Dim entityB As LineItem = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA, entityB)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Update()
			Try
				Dim entityA As LineItem = GetFirstLineItem()
				
				entityA.ItemId = "Te"
				
				manager.Update(entityA)

				Dim entityB As LineItem = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA.OrderId, entityB.OrderId)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Delete()
			Try
				Dim entity As LineItem = GetFirstLineItem()

				manager.Delete(entity)

                entity = manager.GetById(entity.Id)
                Assert.IsNull(entity)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
	End Class
End Namespace

