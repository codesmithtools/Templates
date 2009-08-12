Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	'Public Partial Interface IOrderManager
	'	Inherits IManagerBase(Of Order, System.Int32)
	'End Interface

	Partial Class OrderManager
		Inherits ManagerBase(Of Order, System.Int32)
		Implements IOrderManager
	End Class
End Namespace
