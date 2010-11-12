Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	Public Interface IOrderStatusManager
		Inherits IManagerBase(Of OrderStatus, System.String)
		
		' Get Methods
		Overloads Function GetById(ByVal orderId As System.Int32, ByVal lineNum As System.Int32) As OrderStatus
		Function GetByOrderId(ByVal orderId As System.Int32) As IList(Of OrderStatus)
		
	End Interface

	Partial Class OrderStatusManager
		Inherits ManagerBase(Of OrderStatus, System.String)
		Implements IOrderStatusManager
#region "Constructors"

		Public Sub New()
			MyBase.New()
		End Sub
		Public Sub New(ByVal session As INHibernateSession)
			MyBase.New(session)
		End Sub
#End Region

#region "Get Methods"

		Public Overloads Overrides Function GetById(ByVal id As String) As OrderStatus
			Dim keys As String() = id.Split("^"C)
		
			If keys.Length <> 2 Then
				Throw New Exception("Invalid Id for OrderStatusManager.GetById")
			End If
		
			Return GetById(System.Int32.Parse(keys(0)), System.Int32.Parse(keys(1)))
		End Function
		Public Overloads Function GetById(ByVal orderId As System.Int32, ByVal lineNum As System.Int32) As OrderStatus  Implements IOrderStatusManager.GetById
			Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(OrderStatus))
		
			criteria.Add(NHibernate.Criterion.Expression.Eq("OrderId", orderId))
			criteria.Add(NHibernate.Criterion.Expression.Eq("LineNum", lineNum))
			
			Dim result As OrderStatus = DirectCast(criteria.UniqueResult(), OrderStatus)
		
			If result Is Nothing Then
				Throw New NHibernate.ObjectDeletedException("", Nothing, Nothing)
			End If
		
			Return result
		End Function
		Public Function GetByOrderId(ByVal orderId As System.Int32) As IList(Of OrderStatus) Implements IOrderStatusManager.GetByOrderId
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(OrderStatus))
			
			
			Dim orderIdCriteria As ICriteria = criteria.CreateCriteria("OrderId")
            orderIdCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", orderId))
						
            return criteria.List(Of OrderStatus)()
        End Function
		
#End Region
	End Class
End Namespace
