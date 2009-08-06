Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	'Public Partial Interface IItemManager
	'	Inherits IManagerBase(Of Item, System.String)
	'End Interface

	Partial Class ItemManager
		Inherits ManagerBase(Of Item, System.String)
		Implements IItemManager
	End Class
End Namespace
