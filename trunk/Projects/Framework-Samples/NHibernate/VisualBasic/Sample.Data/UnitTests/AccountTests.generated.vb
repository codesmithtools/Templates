Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports Sample.Data.Generated.ManagerObjects
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.UnitTests
	<TestFixture()> _
	Public Partial Class AccountTests
		Inherits UNuitTestBase
		
		Protected manager As IAccountManager

		Public Sub New()
			manager = managerFactory.GetAccountManager()
		End Sub

		Protected Function CreateNewAccount() As Account
			Dim entity As New Account()

			
			entity.Email = "Test Test Test Test Test Test Test Test Test "
			entity.FirstName = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test"
			entity.LastName = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test"
			entity.Address1 = "Test Test T"
			entity.Address2 = "Test Test Test Test Test Test Test Test Test Test Tes"
			entity.City = "Test Test Test Test Test Test Test Test Test Test Test "
			entity.State = "Test Test Test Test Test Test Test Test Test Test Test Test Test Test Te"
			entity.Zip = "Test Test "
			entity.Country = "Test Test Test"
			entity.Phone = "Test Test T"
			entity.Number = 55
			
			Dim profileManager As IProfileManager = managerFactory.GetProfileManager()
			entity.Profile = profileManager.GetAll(1)(0)

			Return entity
		End Function
		Protected Function GetFirstAccount() As Account
			Dim entityList As IList(Of Account) = manager.GetAll(1)
			If entityList.Count = 0 Then
				Assert.Fail("All tables must have at least one row for unit tests to succeed.")
			End If
			Return entityList(0)
		End Function

		<Test()> _
		Public Sub Create()
			Try
				Dim entity As Account = CreateNewAccount()

				Dim result As Object = manager.Save(entity)

				Assert.IsNotNull(result)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Read()
			Try
				Dim entityA As Account = CreateNewAccount()
				manager.Save(entityA)

				Dim entityB As Account = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA, entityB)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Update()
			Try
				Dim entityA As Account = GetFirstAccount()
				
				entityA.Email = "Test Test Test Test Test Test T"
				
				manager.Update(entityA)

				Dim entityB As Account = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA.Email, entityB.Email)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Delete()
			Try
				Dim entity As Account = GetFirstAccount()

				manager.Delete(entity)

                entity = manager.GetById(entity.Id)
                Assert.IsNull(entity)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
	End Class
End Namespace

