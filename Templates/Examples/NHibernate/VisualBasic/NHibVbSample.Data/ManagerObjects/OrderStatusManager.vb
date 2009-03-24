Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	'Public Partial Interface IOrderStatusManager
	'	Inherits IManagerBase(Of OrderStatus, System.String)
	'End Interface

	Partial Class OrderStatusManager
		Inherits ManagerBase(Of OrderStatus, System.String)
		Implements IOrderStatusManager
	End Class
End Namespace
