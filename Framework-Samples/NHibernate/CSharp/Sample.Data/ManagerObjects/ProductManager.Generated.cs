using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface IProductManager : IManagerBase<Sample.Data.Generated.BusinessObjects.Product, string>
    {
		// Get Methods
		IList<Product> GetByCategoryId(System.String category);
		IList<Product> GetByName(System.String name);
		IList<Product> GetByCategoryIdName(System.String category, System.String name);
		Product GetByCategoryIdProductIdName(System.String category, System.String productId, System.String name);
    }

    partial class ProductManager : ManagerBase<Sample.Data.Generated.BusinessObjects.Product, string>, IProductManager
    {
		#region Constructors
		
		public ProductManager() : base()
        {
        }
        public ProductManager(INHibernateSession session) : base(session)
        {
        }
		
		#endregion
		
        #region Get Methods

		
		public IList<Product> GetByCategoryId(System.String category)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(Product));
			
			
			ICriteria categoryCriteria = criteria.CreateCriteria("Category");
            categoryCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", category));
			
			return criteria.List<Product>();
        }
		
		public IList<Product> GetByName(System.String name)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(Product));
			
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("Name", name));
			
			return criteria.List<Product>();
        }
		
		public IList<Product> GetByCategoryIdName(System.String category, System.String name)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(Product));
			
			
			ICriteria categoryCriteria = criteria.CreateCriteria("Category");
            categoryCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", category));
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("Name", name));
			
			return criteria.List<Product>();
        }
		
		public Product GetByCategoryIdProductIdName(System.String category, System.String productId, System.String name)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(Product));
			
			
			ICriteria categoryCriteria = criteria.CreateCriteria("Category");
            categoryCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", category));
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("ProductId", productId));
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("Name", name));
			
			IList<Product> result = criteria.List<Product>();
			return (result.Count > 0) ? result[0] : null;
        }
		
		#endregion
    }
}