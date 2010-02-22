﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v5.2.1, CSLA Templates: v1.5.0.1413, CSLA Framework: v3.8.2.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'Order.cs'.
//
//     Template: Criteria.Generated.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
#region Using declarations

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Csla;

#endregion

namespace PetShop.Tests.ParameterizedSQL
{
    [Serializable]
    public partial class OrderCriteria : CriteriaBase, IGeneratedCriteria
    {
        #region Private Read-Only Members
        
        private readonly Dictionary<string, object> _bag = new Dictionary<string, object>();
        
        #endregion
        
        #region Constructors

        public OrderCriteria() : base(typeof(Order)){}

        public OrderCriteria(System.Int32 orderId) : base(typeof(Order))
        {
            OrderId = orderId;
        }
        
        #endregion
        
        #region Public Properties
        
        #region Read-Write

        public System.Int32 OrderId
        {
            get { return GetValue< System.Int32 >("OrderId"); }
            set { _bag["OrderId"] = value; }
        }

        public System.String UserId
        {
            get { return GetValue< System.String >("UserId"); }
            set { _bag["UserId"] = value; }
        }

        public System.DateTime OrderDate
        {
            get { return GetValue< System.DateTime >("OrderDate"); }
            set { _bag["OrderDate"] = value; }
        }

        public System.String ShipAddr1
        {
            get { return GetValue< System.String >("ShipAddr1"); }
            set { _bag["ShipAddr1"] = value; }
        }

        public System.String ShipAddr2
        {
            get { return GetValue< System.String >("ShipAddr2"); }
            set { _bag["ShipAddr2"] = value; }
        }

        public System.String ShipCity
        {
            get { return GetValue< System.String >("ShipCity"); }
            set { _bag["ShipCity"] = value; }
        }

        public System.String ShipState
        {
            get { return GetValue< System.String >("ShipState"); }
            set { _bag["ShipState"] = value; }
        }

        public System.String ShipZip
        {
            get { return GetValue< System.String >("ShipZip"); }
            set { _bag["ShipZip"] = value; }
        }

        public System.String ShipCountry
        {
            get { return GetValue< System.String >("ShipCountry"); }
            set { _bag["ShipCountry"] = value; }
        }

        public System.String BillAddr1
        {
            get { return GetValue< System.String >("BillAddr1"); }
            set { _bag["BillAddr1"] = value; }
        }

        public System.String BillAddr2
        {
            get { return GetValue< System.String >("BillAddr2"); }
            set { _bag["BillAddr2"] = value; }
        }

        public System.String BillCity
        {
            get { return GetValue< System.String >("BillCity"); }
            set { _bag["BillCity"] = value; }
        }

        public System.String BillState
        {
            get { return GetValue< System.String >("BillState"); }
            set { _bag["BillState"] = value; }
        }

        public System.String BillZip
        {
            get { return GetValue< System.String >("BillZip"); }
            set { _bag["BillZip"] = value; }
        }

        public System.String BillCountry
        {
            get { return GetValue< System.String >("BillCountry"); }
            set { _bag["BillCountry"] = value; }
        }

        public System.String Courier
        {
            get { return GetValue< System.String >("Courier"); }
            set { _bag["Courier"] = value; }
        }

        public System.Decimal TotalPrice
        {
            get { return GetValue< System.Decimal >("TotalPrice"); }
            set { _bag["TotalPrice"] = value; }
        }

        public System.String BillToFirstName
        {
            get { return GetValue< System.String >("BillToFirstName"); }
            set { _bag["BillToFirstName"] = value; }
        }

        public System.String BillToLastName
        {
            get { return GetValue< System.String >("BillToLastName"); }
            set { _bag["BillToLastName"] = value; }
        }

        public System.String ShipToFirstName
        {
            get { return GetValue< System.String >("ShipToFirstName"); }
            set { _bag["ShipToFirstName"] = value; }
        }

        public System.String ShipToLastName
        {
            get { return GetValue< System.String >("ShipToLastName"); }
            set { _bag["ShipToLastName"] = value; }
        }

        public System.Int32 AuthorizationNumber
        {
            get { return GetValue< System.Int32 >("AuthorizationNumber"); }
            set { _bag["AuthorizationNumber"] = value; }
        }

        public System.String Locale
        {
            get { return GetValue< System.String >("Locale"); }
            set { _bag["Locale"] = value; }
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

        /// <summary>
        /// Returns a list of all the modified properties and values.
        /// </summary>
        public string TableFullName
        {
            get
            {
                return "[dbo].Orders";
            }
        }

        #endregion

        #endregion

        #region Overrides
        
        public override string ToString()
        {
            if (_bag.Count == 0)
                return "No criterion was specified";

            var result = string.Empty;
            foreach (KeyValuePair<string, object> key in _bag)
            {
                result += string.Format("[{0}] = '{1}' AND ", key.Key, key.Value);
            }

            return result.Remove(result.Length - 5, 5);
        }

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