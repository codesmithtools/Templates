using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface IItemManager : IManagerBase<Item, string>
    {
		// Get Methods
		IList<Item> GetByProductId(System.String product);
		IList<Item> GetBySupplier(System.Int32 supplier);
		Item GetByProductIdItemIdListPriceName(System.String product, System.String itemId, System.Decimal listPrice, System.String name);
    }

    partial class ItemManager : ManagerBase<Item, string>, IItemManager
    {
		#region Constructors
		
		public ItemManager() : base()
        {
        }
        public ItemManager(INHibernateSession session) : base(session)
        {
        }
		
		#endregion
		
        #region Get Methods

		
		public IList<Item> GetByProductId(System.String product)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(Item));
			
			
			ICriteria productCriteria = criteria.CreateCriteria("Product");
            productCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", product));
			
			return criteria.List<Item>();
        }
		
		public IList<Item> GetBySupplier(System.Int32 supplier)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(Item));
			
			
			ICriteria supplierCriteria = criteria.CreateCriteria("Supplier");
            supplierCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", supplier));
			
			return criteria.List<Item>();
        }
		
		public Item GetByProductIdItemIdListPriceName(System.String product, System.String itemId, System.Decimal listPrice, System.String name)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(Item));
			
			
			ICriteria productCriteria = criteria.CreateCriteria("Product");
            productCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", product));
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("ItemId", itemId));
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("ListPrice", listPrice));
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("Name", name));
			
			IList<Item> result = criteria.List<Item>();
			return (result.Count > 0) ? result[0] : null;
        }
		
		#endregion
    }
}