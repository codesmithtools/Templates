Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	Public Interface IProductManager
		Inherits IManagerBase(Of Product, System.String)
		
		' Get Methods
		Function GetByCategoryId(ByVal category As System.String) As IList(Of Product)
		Function GetByName(ByVal [Name] As System.String) As IList(Of Product)
		Function GetByCategoryIdName(ByVal category As System.String, ByVal [Name] As System.String) As IList(Of Product)
		Function GetByCategoryIdProductIdName(ByVal category As System.String, ByVal productId As System.String, ByVal [Name] As System.String) As IList(Of Product)
		
	End Interface

	Partial Class ProductManager
		Inherits ManagerBase(Of Product, System.String)
		Implements IProductManager
#region "Constructors"

		Public Sub New()
			MyBase.New()
		End Sub
		Public Sub New(ByVal session As INHibernateSession)
			MyBase.New(session)
		End Sub
#End Region

#region "Get Methods"

		Public Function GetByCategoryId(ByVal category As System.String) As IList(Of Product) Implements IProductManager.GetByCategoryId
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(Product))
			
			
			Dim categoryCriteria As ICriteria = criteria.CreateCriteria("Category")
            categoryCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", category))
						
            return criteria.List(Of Product)()
        End Function
		Public Function GetByName(ByVal [Name] As System.String) As IList(Of Product) Implements IProductManager.GetByName
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(Product))
			
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("Name", name))
						
            return criteria.List(Of Product)()
        End Function
		Public Function GetByCategoryIdName(ByVal category As System.String, ByVal [Name] As System.String) As IList(Of Product) Implements IProductManager.GetByCategoryIdName
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(Product))
			
			
			Dim categoryCriteria As ICriteria = criteria.CreateCriteria("Category")
            categoryCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", category))
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("Name", name))
						
            return criteria.List(Of Product)()
        End Function
		Public Function GetByCategoryIdProductIdName(ByVal category As System.String, ByVal productId As System.String, ByVal [Name] As System.String) As IList(Of Product) Implements IProductManager.GetByCategoryIdProductIdName
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(Product))
			
			
			Dim categoryCriteria As ICriteria = criteria.CreateCriteria("Category")
            categoryCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", category))
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("ProductId", productId))
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("Name", name))
						
            return criteria.List(Of Product)()
        End Function
		
#End Region
	End Class
End Namespace
