//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CSLA 3.8.X CodeSmith Templates.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'Product.cs'.
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

namespace PetShop.Business
{
    [Serializable]
    public partial class ProductCriteria : CriteriaBase
    {
        #region Private Read-Only Members
        
        private readonly Dictionary<string, object> _bag = new Dictionary<string, object>();
        
        #endregion
        
        #region Constructors

        public ProductCriteria() : base(typeof(Product)){}

        public ProductCriteria(System.String productId) : base(typeof(Product))
        {
            ProductId = productId;
        }

        
        #endregion
        
        #region Public Properties
        
        #region Read-Write

        public System.String ProductId
        {
            get { return GetValue< System.String >("ProductId"); }
            set { _bag["ProductId"] = value; }
        }

        public System.String CategoryId
        {
            get { return GetValue< System.String >("CategoryId"); }
            set { _bag["CategoryId"] = value; }
        }

        public System.String Name
        {
            get { return GetValue< System.String >("Name"); }
            set { _bag["Name"] = value; }
        }

        public System.String Descn
        {
            get { return GetValue< System.String >("Descn"); }
            set { _bag["Descn"] = value; }
        }

        public System.String Image
        {
            get { return GetValue< System.String >("Image"); }
            set { _bag["Image"] = value; }
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