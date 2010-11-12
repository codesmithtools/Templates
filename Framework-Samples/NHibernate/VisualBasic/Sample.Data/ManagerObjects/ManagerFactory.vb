Imports System
Imports System.Collections.Generic
Imports System.Text

Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	Public Interface IManagerFactory
		' Get Methods
		Function GetAccountManager() As IAccountManager
		Function GetAccountManager(ByVal session As INHibernateSession) As IAccountManager
		Function GetCartManager() As ICartManager
		Function GetCartManager(ByVal session As INHibernateSession) As ICartManager
		Function GetCategoryManager() As ICategoryManager
		Function GetCategoryManager(ByVal session As INHibernateSession) As ICategoryManager
		Function GetInventoryManager() As IInventoryManager
		Function GetInventoryManager(ByVal session As INHibernateSession) As IInventoryManager
		Function GetItemManager() As IItemManager
		Function GetItemManager(ByVal session As INHibernateSession) As IItemManager
		Function GetLineItemManager() As ILineItemManager
		Function GetLineItemManager(ByVal session As INHibernateSession) As ILineItemManager
		Function GetOrderManager() As IOrderManager
		Function GetOrderManager(ByVal session As INHibernateSession) As IOrderManager
		Function GetOrderStatusManager() As IOrderStatusManager
		Function GetOrderStatusManager(ByVal session As INHibernateSession) As IOrderStatusManager
		Function GetProductManager() As IProductManager
		Function GetProductManager(ByVal session As INHibernateSession) As IProductManager
		Function GetProfileManager() As IProfileManager
		Function GetProfileManager(ByVal session As INHibernateSession) As IProfileManager
		Function GetSupplierManager() As ISupplierManager
		Function GetSupplierManager(ByVal session As INHibernateSession) As ISupplierManager
	End Interface

	Public Class ManagerFactory
		Implements IManagerFactory
		
#region "Constructors"
		Public Sub New()
		End Sub
#End Region

#region "Get Methods"

		Public Function GetAccountManager() As IAccountManager Implements IManagerFactory.GetAccountManager
			Return New AccountManager()
		End Function
		Public Function GetAccountManager(ByVal session As INHibernateSession) As IAccountManager Implements IManagerFactory.GetAccountManager
			Return New AccountManager(session)
		End Function
		Public Function GetCartManager() As ICartManager Implements IManagerFactory.GetCartManager
			Return New CartManager()
		End Function
		Public Function GetCartManager(ByVal session As INHibernateSession) As ICartManager Implements IManagerFactory.GetCartManager
			Return New CartManager(session)
		End Function
		Public Function GetCategoryManager() As ICategoryManager Implements IManagerFactory.GetCategoryManager
			Return New CategoryManager()
		End Function
		Public Function GetCategoryManager(ByVal session As INHibernateSession) As ICategoryManager Implements IManagerFactory.GetCategoryManager
			Return New CategoryManager(session)
		End Function
		Public Function GetInventoryManager() As IInventoryManager Implements IManagerFactory.GetInventoryManager
			Return New InventoryManager()
		End Function
		Public Function GetInventoryManager(ByVal session As INHibernateSession) As IInventoryManager Implements IManagerFactory.GetInventoryManager
			Return New InventoryManager(session)
		End Function
		Public Function GetItemManager() As IItemManager Implements IManagerFactory.GetItemManager
			Return New ItemManager()
		End Function
		Public Function GetItemManager(ByVal session As INHibernateSession) As IItemManager Implements IManagerFactory.GetItemManager
			Return New ItemManager(session)
		End Function
		Public Function GetLineItemManager() As ILineItemManager Implements IManagerFactory.GetLineItemManager
			Return New LineItemManager()
		End Function
		Public Function GetLineItemManager(ByVal session As INHibernateSession) As ILineItemManager Implements IManagerFactory.GetLineItemManager
			Return New LineItemManager(session)
		End Function
		Public Function GetOrderManager() As IOrderManager Implements IManagerFactory.GetOrderManager
			Return New OrderManager()
		End Function
		Public Function GetOrderManager(ByVal session As INHibernateSession) As IOrderManager Implements IManagerFactory.GetOrderManager
			Return New OrderManager(session)
		End Function
		Public Function GetOrderStatusManager() As IOrderStatusManager Implements IManagerFactory.GetOrderStatusManager
			Return New OrderStatusManager()
		End Function
		Public Function GetOrderStatusManager(ByVal session As INHibernateSession) As IOrderStatusManager Implements IManagerFactory.GetOrderStatusManager
			Return New OrderStatusManager(session)
		End Function
		Public Function GetProductManager() As IProductManager Implements IManagerFactory.GetProductManager
			Return New ProductManager()
		End Function
		Public Function GetProductManager(ByVal session As INHibernateSession) As IProductManager Implements IManagerFactory.GetProductManager
			Return New ProductManager(session)
		End Function
		Public Function GetProfileManager() As IProfileManager Implements IManagerFactory.GetProfileManager
			Return New ProfileManager()
		End Function
		Public Function GetProfileManager(ByVal session As INHibernateSession) As IProfileManager Implements IManagerFactory.GetProfileManager
			Return New ProfileManager(session)
		End Function
		Public Function GetSupplierManager() As ISupplierManager Implements IManagerFactory.GetSupplierManager
			Return New SupplierManager()
		End Function
		Public Function GetSupplierManager(ByVal session As INHibernateSession) As ISupplierManager Implements IManagerFactory.GetSupplierManager
			Return New SupplierManager(session)
		End Function
		
#End Region
	End Class
End Namespace
