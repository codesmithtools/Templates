Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	Public Interface ICategoryManager
		Inherits IManagerBase(Of Category, System.String)
		
		' Get Methods
		
	End Interface

	Partial Class CategoryManager
		Inherits ManagerBase(Of Category, System.String)
		Implements ICategoryManager
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
