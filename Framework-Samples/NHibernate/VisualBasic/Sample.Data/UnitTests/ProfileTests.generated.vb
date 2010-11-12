Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports Sample.Data.Generated.ManagerObjects
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.UnitTests
	<TestFixture()> _
	Public Partial Class ProfileTests
		Inherits UNuitTestBase
		
		Protected manager As IProfileManager

		
		<SetUp()> _
		Public Sub SetUp()
            manager = managerFactory.GetProfileManager()
            manager.Session.BeginTransaction()
		End Sub
		<TearDown()> _
		Public Sub TearDown()
			manager.Session.RollbackTransaction()
            manager.Dispose()
		End Sub

		Protected Function CreateNewProfile() As Profile
			Dim entity As New Profile()

			
			entity.Username = "Test Test "
			entity.ApplicationName = "Test Test "
			entity.IsAnonymous = True
			entity.LastActivityDate = DateTime.Now
			entity.LastUpdatedDate = DateTime.Now

			Return entity
		End Function
		Protected Function GetFirstProfile() As Profile
			Dim entityList As IList(Of Profile) = manager.GetAll(1)
			If entityList.Count = 0 Then
				Assert.Fail("All tables must have at least one row for unit tests to succeed.")
			End If
			Return entityList(0)
		End Function

		<Test()> _
		Public Sub Create()
			Try
				Dim entity As Profile = CreateNewProfile()

				Dim result As Object = manager.Save(entity)

				Assert.IsNotNull(result)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Read()
			Try
				Dim entityA As Profile = CreateNewProfile()
				manager.Save(entityA)

				Dim entityB As Profile = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA, entityB)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Update()
			Try
				Dim entityA As Profile = GetFirstProfile()
				
				entityA.Username = "Test Test "
				
				manager.Update(entityA)

				Dim entityB As Profile = manager.GetById(entityA.Id)

				Assert.AreEqual(entityA.Username, entityB.Username)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
		<Test()> _
		Public Sub Delete()
			Try
				Dim entity As Profile = GetFirstProfile()

				manager.Delete(entity)

                entity = manager.GetById(entity.Id)
                Assert.IsNull(entity)
			Catch ex As Exception
				Assert.Fail(ex.ToString())
			End Try
		End Sub
	End Class
End Namespace

