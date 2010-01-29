Imports System
Imports System.Collections.Generic
Imports System.Text
Imports NUnit.Framework
Imports Sample.Data.Generated.ManagerObjects
Imports Sample.Data.Generated.BusinessObjects

Namespace Sample.Data.Generated.Base
	Public Class UNuitTestBase
		Protected managerFactory As IManagerFactory = New ManagerFactory()
	End Class
End Namespace
