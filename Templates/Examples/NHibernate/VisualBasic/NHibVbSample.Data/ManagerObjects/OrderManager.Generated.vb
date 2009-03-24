Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	Public Interface IOrderManager
		Inherits IManagerBase(Of Order, System.Int32)
		
		' Get Methods
		
	End Interface

	Partial Class OrderManager
		Inherits ManagerBase(Of Order, System.Int32)
		Implements IOrderManager
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
