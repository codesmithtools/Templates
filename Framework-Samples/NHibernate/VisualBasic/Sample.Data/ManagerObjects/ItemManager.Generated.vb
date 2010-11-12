Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	Public Interface IItemManager
		Inherits IManagerBase(Of Item, System.String)
		
		' Get Methods
		Function GetByProductId(ByVal product As System.String) As IList(Of Item)
		Function GetBySupplier(ByVal supplier As System.Int32) As IList(Of Item)
		Function GetByProductIdItemIdListPriceName(ByVal product As System.String, ByVal itemId As System.String, ByVal listPrice As System.Decimal, ByVal name As System.String) As IList(Of Item)
		
	End Interface

	Partial Class ItemManager
		Inherits ManagerBase(Of Item, System.String)
		Implements IItemManager
#region "Constructors"

		Public Sub New()
			MyBase.New()
		End Sub
		Public Sub New(ByVal session As INHibernateSession)
			MyBase.New(session)
		End Sub
#End Region

#region "Get Methods"

		Public Function GetByProductId(ByVal product As System.String) As IList(Of Item) Implements IItemManager.GetByProductId
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(Item))
			
			
			Dim productCriteria As ICriteria = criteria.CreateCriteria("Product")
            productCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", product))
						
            return criteria.List(Of Item)()
        End Function
		Public Function GetBySupplier(ByVal supplier As System.Int32) As IList(Of Item) Implements IItemManager.GetBySupplier
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(Item))
			
			
			Dim supplierCriteria As ICriteria = criteria.CreateCriteria("Supplier")
            supplierCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", supplier))
						
            return criteria.List(Of Item)()
        End Function
		Public Function GetByProductIdItemIdListPriceName(ByVal product As System.String, ByVal itemId As System.String, ByVal listPrice As System.Decimal, ByVal name As System.String) As IList(Of Item) Implements IItemManager.GetByProductIdItemIdListPriceName
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(Item))
			
			
			Dim productCriteria As ICriteria = criteria.CreateCriteria("Product")
            productCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", product))
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("ItemId", itemId))
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("ListPrice", listPrice))
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("Name", name))
						
            return criteria.List(Of Item)()
        End Function
		
#End Region
	End Class
End Namespace
