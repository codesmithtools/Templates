using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface IOrderStatusManager : IManagerBase<OrderStatus, string>
    {
		// Get Methods
		OrderStatus GetById(System.Int32 orderId, System.Int32 lineNum);
		IList<OrderStatus> GetByOrderId(System.Int32 orderId);
    }

    partial class OrderStatusManager : ManagerBase<OrderStatus, string>, IOrderStatusManager
    {
		#region Constructors
		
		public OrderStatusManager() : base()
        {
        }
        public OrderStatusManager(INHibernateSession session) : base(session)
        {
        }
		
		#endregion
		
        #region Get Methods

		public override OrderStatus GetById(string id)
		{
			string[] keys = id.Split('^');
			
			if(keys.Length != 2)
				throw new Exception("Invalid Id for OrderStatusManager.GetById");
			
			return GetById(System.Int32.Parse(keys[0]), System.Int32.Parse(keys[1]));
		}
		public OrderStatus GetById(System.Int32 orderId, System.Int32 lineNum)
		{
			ICriteria criteria = Session.GetISession().CreateCriteria(typeof(OrderStatus));
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("OrderId", orderId));
			criteria.Add(NHibernate.Criterion.Expression.Eq("LineNum", lineNum));
			
			OrderStatus result = (OrderStatus)criteria.UniqueResult();

            if (result == null)
                throw new NHibernate.ObjectDeletedException("", null, null);

            return result;
		}
		
		
		public IList<OrderStatus> GetByOrderId(System.Int32 orderId)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(OrderStatus));
			
			
			ICriteria orderIdCriteria = criteria.CreateCriteria("OrderId");
            orderIdCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", orderId));
			
			return criteria.List<OrderStatus>();
        }
		
		#endregion
    }
}