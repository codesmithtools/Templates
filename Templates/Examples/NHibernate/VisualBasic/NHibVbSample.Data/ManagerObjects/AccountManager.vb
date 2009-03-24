Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	'Public Partial Interface IAccountManager
	'	Inherits IManagerBase(Of Account, System.Int32)
	'End Interface

	Partial Class AccountManager
		Inherits ManagerBase(Of Account, System.Int32)
		Implements IAccountManager
	End Class
End Namespace
