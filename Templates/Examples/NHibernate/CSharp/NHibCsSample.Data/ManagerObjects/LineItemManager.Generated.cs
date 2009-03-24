using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibCsSample.Generated.BusinessObjects;
using NHibCsSample.Generated.Base;

namespace NHibCsSample.Generated.ManagerObjects
{
    public partial interface ILineItemManager : IManagerBase<LineItem, string>
    {
		// Get Methods
		LineItem GetById(System.Int32 orderId, System.Int32 lineNum);
		IList<LineItem> GetByOrderId(System.Int32 orderId);
    }

    partial class LineItemManager : ManagerBase<LineItem, string>, ILineItemManager
    {
		#region Constructors
		
		public LineItemManager() : base()
        {
        }
        public LineItemManager(INHibernateSession session) : base(session)
        {
        }
		
		#endregion
		
        #region Get Methods

		public override LineItem GetById(string id)
		{
			string[] keys = id.Split('^');
			
			if(keys.Length != 2)
				throw new Exception("Invalid Id for LineItemManager.GetById");
			
			return GetById(System.Int32.Parse(keys[0]), System.Int32.Parse(keys[1]));
		}
		public LineItem GetById(System.Int32 orderId, System.Int32 lineNum)
		{
			ICriteria criteria = Session.GetISession().CreateCriteria(typeof(LineItem));
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("OrderId", orderId));
			criteria.Add(NHibernate.Criterion.Expression.Eq("LineNum", lineNum));
			
			LineItem result = (LineItem)criteria.UniqueResult();

            if (result == null)
                throw new NHibernate.ObjectDeletedException("", null, null);

            return result;
		}
		
		
		public IList<LineItem> GetByOrderId(System.Int32 orderId)
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(LineItem));
			
			
			ICriteria orderIdCriteria = criteria.CreateCriteria("OrderId");
            orderIdCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", orderId));
			
			return criteria.List<LineItem>();
        }
		
		#endregion
    }
}