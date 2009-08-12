Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports Sample.Data.Generated.ManagerObjects
Imports Sample.Data.Generated.BusinessObjects

Namespace Sample.Data.Generated.Base
	Public Class UNuitTestBase
		Protected managerFactory As IManagerFactory = New ManagerFactory()

		<SetUp()> _
		Public Sub SetUp()
			NHibernateSessionManager.Instance.Session.BeginTransaction()
		End Sub
		<TearDown()> _
		Public Sub TearDown()
			NHibernateSessionManager.Instance.Session.RollbackTransaction()
		End Sub
	End Class
End Namespace
