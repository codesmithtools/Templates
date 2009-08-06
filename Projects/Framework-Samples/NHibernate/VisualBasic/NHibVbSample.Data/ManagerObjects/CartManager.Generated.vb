Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	Public Interface ICartManager
		Inherits IManagerBase(Of Cart, System.Int32)
		
		' Get Methods
		Function GetByUniqueID(ByVal profile As System.Int32) As IList(Of Cart)
		Function GetByIsShoppingCart(ByVal isShoppingCart As System.Boolean) As IList(Of Cart)
		
	End Interface

	Partial Class CartManager
		Inherits ManagerBase(Of Cart, System.Int32)
		Implements ICartManager
#region "Constructors"

		Public Sub New()
			MyBase.New()
		End Sub
		Public Sub New(ByVal session As INHibernateSession)
			MyBase.New(session)
		End Sub
#End Region

#region "Get Methods"

		Public Function GetByUniqueID(ByVal profile As System.Int32) As IList(Of Cart) Implements ICartManager.GetByUniqueID
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(Cart))
			
			
			Dim profileCriteria As ICriteria = criteria.CreateCriteria("Profile")
            profileCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", profile))
						
            return criteria.List(Of Cart)()
        End Function
		Public Function GetByIsShoppingCart(ByVal isShoppingCart As System.Boolean) As IList(Of Cart) Implements ICartManager.GetByIsShoppingCart
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(Cart))
			
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("IsShoppingCart", isShoppingCart))
						
            return criteria.List(Of Cart)()
        End Function
		
#End Region
	End Class
End Namespace
