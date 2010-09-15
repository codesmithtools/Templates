﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v5.2.3, CSLA Templates: v3.0.1.1934, CSLA Framework: v3.8.4.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'CartList.cs'.
//
//     Template: EditableChildList.Generated.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
#region Using declarations

using System;
using System.Collections.Generic;

using Csla;
using Csla.Data;

#endregion

namespace PetShop.Tests.ObjF.ParameterizedSQL
{
    [Serializable]
    [Csla.Server.ObjectFactory(FactoryNames.CartListFactoryName)]
    public partial class CartList : BusinessListBase< CartList, Cart >
    {
        #region Constructor(s)

        private CartList()
        { 
            AllowNew = true;
            MarkAsChild();
        }
        
        #endregion

        #region Synchronous Factory Methods 
        
        internal static CartList NewList()
        {
            return DataPortal.CreateChild< CartList >();
        }     

        internal static CartList GetByCartId(System.Int32 cartId)
        {
            var criteria = new CartCriteria{CartId = cartId};
            
            
            return DataPortal.FetchChild< CartList >(criteria);
        }

        internal static CartList GetByUniqueID(System.Int32 uniqueID)
        {
            var criteria = new CartCriteria{UniqueID = uniqueID};
            
            
            return DataPortal.FetchChild< CartList >(criteria);
        }

        internal static CartList GetByIsShoppingCart(System.Boolean isShoppingCart)
        {
            var criteria = new CartCriteria{IsShoppingCart = isShoppingCart};
            
            
            return DataPortal.FetchChild< CartList >(criteria);
        }

        internal static CartList GetAll()
        {
            return DataPortal.FetchChild< CartList >(new CartCriteria());
        }

        #endregion


        #region Method Overrides
        
        protected override object AddNewCore()
        {
            Cart item = PetShop.Tests.ObjF.ParameterizedSQL.Cart.NewCart();

            bool cancel = false;
            OnAddNewCore(ref item, ref cancel);
            if (!cancel)
            {
                // Check to see if someone set the item to null in the OnAddNewCore.
                if(item == null)
                    item = PetShop.Tests.ObjF.ParameterizedSQL.Cart.NewCart();

                // Pass the parent value down to the child.
                //Profile profile = this.Parent as Profile;
                //if(profile != null)
                //    item.UniqueID = profile.UniqueID;


                Add(item);
            }

            return item;
        }
        
        #endregion

        #region Property overrides

        /// <summary>
        /// Returns true if any children are dirty
        /// </summary>
        public new bool IsDirty
        {
            get
            {
                foreach(Cart item in this.Items)
                {
                    if(item.IsDirty) return true;
                }
                
                return false;
            }
        }

        #endregion

        #region DataPortal partial methods

        partial void OnCreating(ref bool cancel);
        partial void OnCreated();
        partial void OnFetching(CartCriteria criteria, ref bool cancel);
        partial void OnFetched();
        partial void OnMapping(SafeDataReader reader, ref bool cancel);
        partial void OnMapped();
        partial void OnUpdating(ref bool cancel);
        partial void OnUpdated();
        partial void OnAddNewCore(ref Cart item, ref bool cancel);

        #endregion

        #region Exists Command

        public static bool Exists(CartCriteria criteria)
        {
            return PetShop.Tests.ObjF.ParameterizedSQL.Cart.Exists(criteria);
        }

        #endregion

    }
}