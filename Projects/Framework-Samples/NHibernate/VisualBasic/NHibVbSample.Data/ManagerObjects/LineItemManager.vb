Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	'Public Partial Interface ILineItemManager
	'	Inherits IManagerBase(Of LineItem, System.String)
	'End Interface

	Partial Class LineItemManager
		Inherits ManagerBase(Of LineItem, System.String)
		Implements ILineItemManager
	End Class
End Namespace
