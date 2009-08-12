Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	Public Interface IInventoryManager
		Inherits IManagerBase(Of Inventory, System.String)
		
		' Get Methods
		
	End Interface

	Partial Class InventoryManager
		Inherits ManagerBase(Of Inventory, System.String)
		Implements IInventoryManager
#region "Constructors"

		Public Sub New()
			MyBase.New()
		End Sub
		Public Sub New(ByVal session As INHibernateSession)
			MyBase.New(session)
		End Sub
#End Region

#region "Get Methods"

		
#End Region
	End Class
End Namespace
