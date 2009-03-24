Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	'Public Partial Interface ICartManager
	'	Inherits IManagerBase(Of Cart, System.Int32)
	'End Interface

	Partial Class CartManager
		Inherits ManagerBase(Of Cart, System.Int32)
		Implements ICartManager
	End Class
End Namespace
