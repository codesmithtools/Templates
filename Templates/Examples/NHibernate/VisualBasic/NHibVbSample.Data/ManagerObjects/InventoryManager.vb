Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	'Public Partial Interface IInventoryManager
	'	Inherits IManagerBase(Of Inventory, System.String)
	'End Interface

	Partial Class InventoryManager
		Inherits ManagerBase(Of Inventory, System.String)
		Implements IInventoryManager
	End Class
End Namespace
