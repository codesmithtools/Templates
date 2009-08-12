Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	'Public Partial Interface ICartManager
	'	Inherits IManagerBase(Of Cart, System.Int32)
	'End Interface

	Partial Class CartManager
		Inherits ManagerBase(Of Cart, System.Int32)
		Implements ICartManager
	End Class
End Namespace
