Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	Public Interface ILineItemManager
		Inherits IManagerBase(Of LineItem, System.String)
		
		' Get Methods
		Overloads Function GetById(ByVal orderId As System.Int32, ByVal lineNum As System.Int32) As LineItem
		Function GetByOrderId(ByVal orderId As System.Int32) As IList(Of LineItem)
		
	End Interface

	Partial Class LineItemManager
		Inherits ManagerBase(Of LineItem, System.String)
		Implements ILineItemManager
#region "Constructors"

		Public Sub New()
			MyBase.New()
		End Sub
		Public Sub New(ByVal session As INHibernateSession)
			MyBase.New(session)
		End Sub
#End Region

#region "Get Methods"

		Public Overloads Overrides Function GetById(ByVal id As String) As LineItem
			Dim keys As String() = id.Split("^"C)
		
			If keys.Length <> 2 Then
				Throw New Exception("Invalid Id for LineItemManager.GetById")
			End If
		
			Return GetById(System.Int32.Parse(keys(0)), System.Int32.Parse(keys(1)))
		End Function
		Public Overloads Function GetById(ByVal orderId As System.Int32, ByVal lineNum As System.Int32) As LineItem  Implements ILineItemManager.GetById
			Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(LineItem))
		
			criteria.Add(NHibernate.Criterion.Expression.Eq("OrderId", orderId))
			criteria.Add(NHibernate.Criterion.Expression.Eq("LineNum", lineNum))
			
			Dim result As LineItem = DirectCast(criteria.UniqueResult(), LineItem)
		
			If result Is Nothing Then
				Throw New NHibernate.ObjectDeletedException("", Nothing, Nothing)
			End If
		
			Return result
		End Function
		Public Function GetByOrderId(ByVal orderId As System.Int32) As IList(Of LineItem) Implements ILineItemManager.GetByOrderId
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(LineItem))
			
			
			Dim orderIdCriteria As ICriteria = criteria.CreateCriteria("OrderId")
            orderIdCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", orderId))
						
            return criteria.List(Of LineItem)()
        End Function
		
#End Region
	End Class
End Namespace
