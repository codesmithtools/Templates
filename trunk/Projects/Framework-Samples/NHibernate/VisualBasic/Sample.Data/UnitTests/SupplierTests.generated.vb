Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports Sample.Data.Generated.ManagerObjects
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.UnitTests
	<TestFixture()> _
	Public Partial Class SupplierTests
		Inherits UNuitTestBase
		
		Protected manager As ISupplierManager

		
		<SetUp()> _
		Public Sub SetUp()
            manager = managerFactory.GetSupplierManager()
            manager.Session.BeginTransaction()
		End Sub
		<TearDown()> _
		Public Sub TearDown()
			manager.Session.RollbackTransaction()
            manager.Dispose()
		End Sub

		Protected Function CreateNewSupplier() As Supplier
			Dim entity As New Supplier()

			' You may need to maually enter this key if there is a constraint violation.
			entity.Id = 75
			
			entity.Name = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Te"
			entity.Status = "T"
			entity.Addr1 = "Test Test Test Test Test Test Te"
			entity.Addr2 = "Test Test Test Test Test Test Test Test Test Test T"
			entity.City = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test"
			entity.State = "Test Test Test Test Test Test Test Test Test Test Te"
			entity.Zip = "Test"
			entity.Phone = "Test Test "

			Return entity
		End Function
		Protected Function GetFirstSupplier() As Supplier
			Dim entityList As IList(Of Supplier) = manager.GetAll(1)
			If entityList.Count = 0 Then
				Assert.Fail("All tables must have at least one row for unit tests to succeed.")
			End If
			Return entityList(0)
		End Function

		<Test()> _
		Public Sub Create()
			Try
				Dim entity As Supplier = CreateNewSupplier()

				Dim result As Object = manager.Save(entity)

				Assert.IsNotNull(result)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Read()
			Try
				Dim entityA As Supplier = CreateNewSupplier()
				manager.Save(entityA)

				Dim entityB As Supplier = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA, entityB)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Update()
			Try
				Dim entityA As Supplier = GetFirstSupplier()
				
				entityA.Name = "Test Test Test Test Test Test "
				
				manager.Update(entityA)

				Dim entityB As Supplier = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA.Name, entityB.Name)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Delete()
			Try
				Dim entity As Supplier = GetFirstSupplier()

				manager.Delete(entity)

                entity = manager.GetById(entity.Id)
                Assert.IsNull(entity)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
	End Class
End Namespace

