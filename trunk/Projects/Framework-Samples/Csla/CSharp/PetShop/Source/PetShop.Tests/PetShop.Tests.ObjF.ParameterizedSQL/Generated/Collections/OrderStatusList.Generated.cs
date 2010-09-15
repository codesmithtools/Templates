﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v5.2.3, CSLA Templates: v3.0.1.1934, CSLA Framework: v3.8.4.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'OrderStatusList.cs'.
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
    [Csla.Server.ObjectFactory(FactoryNames.OrderStatusListFactoryName)]
    public partial class OrderStatusList : BusinessListBase< OrderStatusList, OrderStatus >
    {
        #region Constructor(s)

        private OrderStatusList()
        { 
            AllowNew = true;
            MarkAsChild();
        }
        
        #endregion

        #region Synchronous Factory Methods 
        
        internal static OrderStatusList NewList()
        {
            return DataPortal.CreateChild< OrderStatusList >();
        }     

        internal static OrderStatusList GetByOrderIdLineNum(System.Int32 orderId, System.Int32 lineNum)
        {
            var criteria = new OrderStatusCriteria{OrderId = orderId, LineNum = lineNum};
            
            
            return DataPortal.FetchChild< OrderStatusList >(criteria);
        }

        internal static OrderStatusList GetByOrderId(System.Int32 orderId)
        {
            var criteria = new OrderStatusCriteria{OrderId = orderId};
            
            
            return DataPortal.FetchChild< OrderStatusList >(criteria);
        }

        internal static OrderStatusList GetAll()
        {
            return DataPortal.FetchChild< OrderStatusList >(new OrderStatusCriteria());
        }

        #endregion


        #region Method Overrides
        
        protected override object AddNewCore()
        {
            OrderStatus item = PetShop.Tests.ObjF.ParameterizedSQL.OrderStatus.NewOrderStatus();

            bool cancel = false;
            OnAddNewCore(ref item, ref cancel);
            if (!cancel)
            {
                // Check to see if someone set the item to null in the OnAddNewCore.
                if(item == null)
                    item = PetShop.Tests.ObjF.ParameterizedSQL.OrderStatus.NewOrderStatus();

                // Pass the parent value down to the child.
                //Order order = this.Parent as Order;
                //if(order != null)
                //    item.OrderId = order.OrderId;


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
                foreach(OrderStatus item in this.Items)
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
        partial void OnFetching(OrderStatusCriteria criteria, ref bool cancel);
        partial void OnFetched();
        partial void OnMapping(SafeDataReader reader, ref bool cancel);
        partial void OnMapped();
        partial void OnUpdating(ref bool cancel);
        partial void OnUpdated();
        partial void OnAddNewCore(ref OrderStatus item, ref bool cancel);

        #endregion

        #region Exists Command

        public static bool Exists(OrderStatusCriteria criteria)
        {
            return PetShop.Tests.ObjF.ParameterizedSQL.OrderStatus.Exists(criteria);
        }

        #endregion

    }
}