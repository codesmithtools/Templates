Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	'Public Partial Interface IAccountManager
	'	Inherits IManagerBase(Of Account, System.Int32)
	'End Interface

	Partial Class AccountManager
		Inherits ManagerBase(Of Account, System.Int32)
		Implements IAccountManager
	End Class
End Namespace
