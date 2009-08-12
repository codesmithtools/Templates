//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CSLA 3.7.X CodeSmith Templates.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'LineItem.cs'.
//
//     Template: Criteria.Generated.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
#region using declarations

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Csla;

#endregion

namespace PetShop.Business
{
	[Serializable]
	public partial class LineItemCriteria : CriteriaBase
	{
        #region Private Read-Only Members
        
        private readonly Dictionary<string, object> _bag = new Dictionary<string, object>();
        
        #endregion
        
        #region Constructors

        public LineItemCriteria() : base(typeof(LineItem)){}

        public LineItemCriteria(int orderId, int lineNum) : base(typeof(LineItem))
        {
            OrderId = orderId;
            LineNum = lineNum;
        }

        
        #endregion
        
		#region Public Properties
        
        #region Read-Write
		
        public int OrderId
		{
            get { return GetValue< int >("OrderId"); }				
			set { _bag["OrderId"] = value; }
		}
		
        public int LineNum
		{
            get { return GetValue< int >("LineNum"); }				
			set { _bag["LineNum"] = value; }
		}

        #endregion
        
        #region Read-Only
        
        /// <summary>
        /// Returns a list of all the modified properties and values.
        /// </summary>
        public Dictionary<string, object> StateBag
	    {
	        get
	        {
	            return _bag;
	        }
	    }
        
        #endregion

        #endregion
        
        #region Private Methods
        
        private T GetValue<T>(string name)
        {
            object value;
            if (_bag.TryGetValue(name, out value))
                return (T) value;
        
            return default(T);
        }
        
        #endregion
	}
}