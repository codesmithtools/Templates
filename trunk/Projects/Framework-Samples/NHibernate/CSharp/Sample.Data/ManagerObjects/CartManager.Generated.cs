using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Sample.Data.Generated.BusinessObjects;
using Sample.Data.Generated.Base;

namespace Sample.Data.Generated.ManagerObjects
{
    public partial interface ICartManager : IManagerBase<Cart, int>
    {
		// Get Methods
		IList<Cart> GetByUniqueID(System.Int32 profile);
		IList<Cart> GetByIsShoppingCart(System.Boolean isShoppingCart);
    }

    partial class CartManager : ManagerBase<Cart, int>, ICartManager
    {
		#region Constructors
		
		public CartManager() : base()
        {
        }
        public CartManager(INHibernateSession session) : base(session)
        {
        }
		
		#endregion
		
        #region Get Methods

		
		public IList<Cart> GetByUniqueID(System.Int32 profile)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(Cart));
			
			
			ICriteria profileCriteria = criteria.CreateCriteria("Profile");
            profileCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", profile));
			
			return criteria.List<Cart>();
        }
		
		public IList<Cart> GetByIsShoppingCart(System.Boolean isShoppingCart)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(Cart));
			
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("IsShoppingCart", isShoppingCart));
			
			return criteria.List<Cart>();
        }
		
		#endregion
    }
}