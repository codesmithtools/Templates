﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v5.2.1, CSLA Templates: v1.5.0.1413, CSLA Framework: v3.8.2.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'OrderStatus.cs'.
//
//     Template path: EditableRoot.Generated.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
#region Using declarations

using System;

using Csla;
using Csla.Data;
using Csla.Validation;

#endregion

namespace PetShop.Tests.ObjF.StoredProcedures
{
    [Serializable]
    [Csla.Server.ObjectFactory(FactoryNames.OrderStatusFactoryName)]
    public partial class OrderStatus : BusinessBase< OrderStatus >
    {
        #region Contructor(s)

        private OrderStatus()
        { /* Require use of factory methods */ }

        internal OrderStatus(System.Int32 orderId, System.Int32 lineNum)
        {
            using(BypassPropertyChecks)
            {
                LoadProperty(_orderIdProperty, orderId);
                LoadProperty(_lineNumProperty, lineNum);
            }
        }
        #endregion

        #region Validation Rules

        protected override void AddBusinessRules()
        {
            if(AddBusinessValidationRules())
                return;

            ValidationRules.AddRule(CommonRules.StringRequired, _statusProperty);
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(_statusProperty, 2));
        }

        #endregion

        #region Properties

        private static readonly PropertyInfo< System.Int32 > _orderIdProperty = RegisterProperty< System.Int32 >(p => p.OrderId);
		[System.ComponentModel.DataObjectField(true, false)]
        public System.Int32 OrderId
        {
            get { return GetProperty(_orderIdProperty); }
            set{ SetProperty(_orderIdProperty, value); }
        }

        private static readonly PropertyInfo< System.Int32 > _originalOrderIdProperty = RegisterProperty< System.Int32 >(p => p.OriginalOrderId);
        /// <summary>
        /// Holds the original value for OrderId. This is used for non identity primary keys.
        /// </summary>
        internal System.Int32 OriginalOrderId
        {
            get { return GetProperty(_originalOrderIdProperty); }
            set{ SetProperty(_originalOrderIdProperty, value); }
        }

        private static readonly PropertyInfo< System.Int32 > _lineNumProperty = RegisterProperty< System.Int32 >(p => p.LineNum);
		[System.ComponentModel.DataObjectField(true, false)]
        public System.Int32 LineNum
        {
            get { return GetProperty(_lineNumProperty); }
            set{ SetProperty(_lineNumProperty, value); }
        }

        private static readonly PropertyInfo< System.Int32 > _originalLineNumProperty = RegisterProperty< System.Int32 >(p => p.OriginalLineNum);
        /// <summary>
        /// Holds the original value for LineNum. This is used for non identity primary keys.
        /// </summary>
        internal System.Int32 OriginalLineNum
        {
            get { return GetProperty(_originalLineNumProperty); }
            set{ SetProperty(_originalLineNumProperty, value); }
        }

        private static readonly PropertyInfo< System.DateTime > _timestampProperty = RegisterProperty< System.DateTime >(p => p.Timestamp);
        public System.DateTime Timestamp
        {
            get { return GetProperty(_timestampProperty); }
            set{ SetProperty(_timestampProperty, value); }
        }

        private static readonly PropertyInfo< System.String > _statusProperty = RegisterProperty< System.String >(p => p.Status);
        public System.String Status
        {
            get { return GetProperty(_statusProperty); }
            set{ SetProperty(_statusProperty, value); }
        }

        //AssociatedManyToOne
        private static readonly PropertyInfo< Order > _orderMemberProperty = RegisterProperty< Order >(p => p.OrderMember, Csla.RelationshipTypes.Child);
        public Order OrderMember
        {
            get
            {
                if(!FieldManager.FieldExists(_orderMemberProperty))
                {
                    if(IsNew || !PetShop.Tests.ObjF.StoredProcedures.Order.Exists(new PetShop.Tests.ObjF.StoredProcedures.OrderCriteria {OrderId = OrderId}))
                        LoadProperty(_orderMemberProperty, PetShop.Tests.ObjF.StoredProcedures.Order.NewOrder());
                    else
                        LoadProperty(_orderMemberProperty, PetShop.Tests.ObjF.StoredProcedures.Order.GetByOrderId(OrderId));
                }

                return GetProperty(_orderMemberProperty); 
            }
        }


        #endregion

        #region Factory Methods 

        public static OrderStatus NewOrderStatus()
        {
            return DataPortal.Create< OrderStatus >();
        }

        public static OrderStatus GetByOrderIdLineNum(System.Int32 orderId, System.Int32 lineNum)
        {
            return DataPortal.Fetch< OrderStatus >(
                new OrderStatusCriteria {OrderId = orderId, LineNum = lineNum});
        }

        public static OrderStatus GetByOrderId(System.Int32 orderId)
        {
            return DataPortal.Fetch< OrderStatus >(
                new OrderStatusCriteria {OrderId = orderId});
        }

        public static void DeleteOrderStatus(System.Int32 orderId, System.Int32 lineNum)
        {
            DataPortal.Delete(new OrderStatusCriteria {OrderId = orderId, LineNum = lineNum});
        }

        #endregion

        #region Exists Command

        public static bool Exists(OrderStatusCriteria criteria)
        {
            return ExistsCommand.Execute(criteria);
        }

        #endregion

        #region Overridden properties

        /// <summary>
        /// Returns true if the business object or any of its children properties are dirty.
        /// </summary>
        public override bool IsDirty
        {
            get
            {
                if (base.IsDirty) return true;
                if (FieldManager.FieldExists(_orderMemberProperty) && OrderMember.IsDirty) return true;

                return false;
            }
        }

        #endregion


        #region Protected Overriden Method(s)

        // NOTE: This is needed for Composite Keys. 
        private readonly Guid _guidID = Guid.NewGuid();
        protected override object GetIdValue()
        {
            return _guidID;
        }

        #endregion
    }
}