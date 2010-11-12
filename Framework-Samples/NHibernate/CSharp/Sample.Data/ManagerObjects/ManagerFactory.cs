using System;
using System.Collections.Generic;
using System.Text;

using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public interface IManagerFactory
    {
		// Get Methods
		IAccountManager GetAccountManager();
		IAccountManager GetAccountManager(INHibernateSession session);
		ICartManager GetCartManager();
		ICartManager GetCartManager(INHibernateSession session);
		ICategoryManager GetCategoryManager();
		ICategoryManager GetCategoryManager(INHibernateSession session);
		IInventoryManager GetInventoryManager();
		IInventoryManager GetInventoryManager(INHibernateSession session);
		IItemManager GetItemManager();
		IItemManager GetItemManager(INHibernateSession session);
		ILineItemManager GetLineItemManager();
		ILineItemManager GetLineItemManager(INHibernateSession session);
		IOrderManager GetOrderManager();
		IOrderManager GetOrderManager(INHibernateSession session);
		IOrderStatusManager GetOrderStatusManager();
		IOrderStatusManager GetOrderStatusManager(INHibernateSession session);
		IProductManager GetProductManager();
		IProductManager GetProductManager(INHibernateSession session);
		IProfileManager GetProfileManager();
		IProfileManager GetProfileManager(INHibernateSession session);
		ISupplierManager GetSupplierManager();
		ISupplierManager GetSupplierManager(INHibernateSession session);
    }

    public class ManagerFactory : IManagerFactory
    {
        #region Constructors

        public ManagerFactory()
        {
        }

        #endregion

        #region Get Methods

		public IAccountManager GetAccountManager()
        {
            return new AccountManager();
        }
		public IAccountManager GetAccountManager(INHibernateSession session)
        {
            return new AccountManager(session);
        }
		public ICartManager GetCartManager()
        {
            return new CartManager();
        }
		public ICartManager GetCartManager(INHibernateSession session)
        {
            return new CartManager(session);
        }
		public ICategoryManager GetCategoryManager()
        {
            return new CategoryManager();
        }
		public ICategoryManager GetCategoryManager(INHibernateSession session)
        {
            return new CategoryManager(session);
        }
		public IInventoryManager GetInventoryManager()
        {
            return new InventoryManager();
        }
		public IInventoryManager GetInventoryManager(INHibernateSession session)
        {
            return new InventoryManager(session);
        }
		public IItemManager GetItemManager()
        {
            return new ItemManager();
        }
		public IItemManager GetItemManager(INHibernateSession session)
        {
            return new ItemManager(session);
        }
		public ILineItemManager GetLineItemManager()
        {
            return new LineItemManager();
        }
		public ILineItemManager GetLineItemManager(INHibernateSession session)
        {
            return new LineItemManager(session);
        }
		public IOrderManager GetOrderManager()
        {
            return new OrderManager();
        }
		public IOrderManager GetOrderManager(INHibernateSession session)
        {
            return new OrderManager(session);
        }
		public IOrderStatusManager GetOrderStatusManager()
        {
            return new OrderStatusManager();
        }
		public IOrderStatusManager GetOrderStatusManager(INHibernateSession session)
        {
            return new OrderStatusManager(session);
        }
		public IProductManager GetProductManager()
        {
            return new ProductManager();
        }
		public IProductManager GetProductManager(INHibernateSession session)
        {
            return new ProductManager(session);
        }
		public IProfileManager GetProfileManager()
        {
            return new ProfileManager();
        }
		public IProfileManager GetProfileManager(INHibernateSession session)
        {
            return new ProfileManager(session);
        }
		public ISupplierManager GetSupplierManager()
        {
            return new SupplierManager();
        }
		public ISupplierManager GetSupplierManager(INHibernateSession session)
        {
            return new SupplierManager(session);
        }
        
        #endregion
    }
}
