﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v6.0.3, CSLA Templates: v4.0.0.0, CSLA Framework: v4.3.10.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'Order.cs'.
//
//     Template: SwitchableObject.Generated.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
using System;

using Csla;
using Csla.Data;
using System.Data.SqlClient;
using Csla.Rules;

namespace PetShop.Tests.ObjF.StoredProcedures
{
    [Serializable]
    [Csla.Server.ObjectFactory(FactoryNames.OrderFactoryName)]
    public partial class Order : BusinessBase<Order>
    {
        #region Contructor(s)

        public Order()
        { /* Require use of factory methods */ }

        #endregion

        #region Business Rules

        /// <summary>
        /// Contains the Codesmith generated validation rules.
        /// </summary>
        protected override void AddBusinessRules()
        {
            // Call the base class, if this call isn't made than any declared System.ComponentModel.DataAnnotations rules will not work.
            base.AddBusinessRules();

            if(AddBusinessValidationRules())
                return;

            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_userIdProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_userIdProperty, 20));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_shipAddr1Property));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_shipAddr1Property, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_shipAddr2Property, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_shipCityProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_shipCityProperty, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_shipStateProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_shipStateProperty, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_shipZipProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_shipZipProperty, 20));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_shipCountryProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_shipCountryProperty, 20));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_billAddr1Property));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_billAddr1Property, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_billAddr2Property, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_billCityProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_billCityProperty, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_billStateProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_billStateProperty, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_billZipProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_billZipProperty, 20));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_billCountryProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_billCountryProperty, 20));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_courierProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_courierProperty, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_billToFirstNameProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_billToFirstNameProperty, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_billToLastNameProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_billToLastNameProperty, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_shipToFirstNameProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_shipToFirstNameProperty, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_shipToLastNameProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_shipToLastNameProperty, 80));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_localeProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(_localeProperty, 20));
        }

        #endregion


        #region Properties

        private static readonly PropertyInfo<System.Int32> _orderIdProperty = RegisterProperty<System.Int32>(p => p.OrderId, "Order Id");
        [System.ComponentModel.DataObjectField(true, true)]
        public System.Int32 OrderId
        {
            get { return GetProperty(_orderIdProperty); }
            internal set{ SetProperty(_orderIdProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _userIdProperty = RegisterProperty<System.String>(p => p.UserId, "User Id");
        public System.String UserId
        {
            get { return GetProperty(_userIdProperty); }
            set{ SetProperty(_userIdProperty, value); }
        }

        private static readonly PropertyInfo<System.DateTime> _orderDateProperty = RegisterProperty<System.DateTime>(p => p.OrderDate, "Order Date");
        public System.DateTime OrderDate
        {
            get { return GetProperty(_orderDateProperty); }
            set{ SetProperty(_orderDateProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _shipAddr1Property = RegisterProperty<System.String>(p => p.ShipAddr1, "Ship Addr 1");
        public System.String ShipAddr1
        {
            get { return GetProperty(_shipAddr1Property); }
            set{ SetProperty(_shipAddr1Property, value); }
        }

        private static readonly PropertyInfo<System.String> _shipAddr2Property = RegisterProperty<System.String>(p => p.ShipAddr2, "Ship Addr 2", (System.String)null);
        public System.String ShipAddr2
        {
            get { return GetProperty(_shipAddr2Property); }
            set{ SetProperty(_shipAddr2Property, value); }
        }

        private static readonly PropertyInfo<System.String> _shipCityProperty = RegisterProperty<System.String>(p => p.ShipCity, "Ship City");
        public System.String ShipCity
        {
            get { return GetProperty(_shipCityProperty); }
            set{ SetProperty(_shipCityProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _shipStateProperty = RegisterProperty<System.String>(p => p.ShipState, "Ship State");
        public System.String ShipState
        {
            get { return GetProperty(_shipStateProperty); }
            set{ SetProperty(_shipStateProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _shipZipProperty = RegisterProperty<System.String>(p => p.ShipZip, "Ship Zip");
        public System.String ShipZip
        {
            get { return GetProperty(_shipZipProperty); }
            set{ SetProperty(_shipZipProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _shipCountryProperty = RegisterProperty<System.String>(p => p.ShipCountry, "Ship Country");
        public System.String ShipCountry
        {
            get { return GetProperty(_shipCountryProperty); }
            set{ SetProperty(_shipCountryProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _billAddr1Property = RegisterProperty<System.String>(p => p.BillAddr1, "Bill Addr 1");
        public System.String BillAddr1
        {
            get { return GetProperty(_billAddr1Property); }
            set{ SetProperty(_billAddr1Property, value); }
        }

        private static readonly PropertyInfo<System.String> _billAddr2Property = RegisterProperty<System.String>(p => p.BillAddr2, "Bill Addr 2", (System.String)null);
        public System.String BillAddr2
        {
            get { return GetProperty(_billAddr2Property); }
            set{ SetProperty(_billAddr2Property, value); }
        }

        private static readonly PropertyInfo<System.String> _billCityProperty = RegisterProperty<System.String>(p => p.BillCity, "Bill City");
        public System.String BillCity
        {
            get { return GetProperty(_billCityProperty); }
            set{ SetProperty(_billCityProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _billStateProperty = RegisterProperty<System.String>(p => p.BillState, "Bill State");
        public System.String BillState
        {
            get { return GetProperty(_billStateProperty); }
            set{ SetProperty(_billStateProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _billZipProperty = RegisterProperty<System.String>(p => p.BillZip, "Bill Zip");
        public System.String BillZip
        {
            get { return GetProperty(_billZipProperty); }
            set{ SetProperty(_billZipProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _billCountryProperty = RegisterProperty<System.String>(p => p.BillCountry, "Bill Country");
        public System.String BillCountry
        {
            get { return GetProperty(_billCountryProperty); }
            set{ SetProperty(_billCountryProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _courierProperty = RegisterProperty<System.String>(p => p.Courier, "Courier");
        public System.String Courier
        {
            get { return GetProperty(_courierProperty); }
            set{ SetProperty(_courierProperty, value); }
        }

        private static readonly PropertyInfo<System.Decimal> _totalPriceProperty = RegisterProperty<System.Decimal>(p => p.TotalPrice, "Total Price");
        public System.Decimal TotalPrice
        {
            get { return GetProperty(_totalPriceProperty); }
            set{ SetProperty(_totalPriceProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _billToFirstNameProperty = RegisterProperty<System.String>(p => p.BillToFirstName, "Bill To First Name");
        public System.String BillToFirstName
        {
            get { return GetProperty(_billToFirstNameProperty); }
            set{ SetProperty(_billToFirstNameProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _billToLastNameProperty = RegisterProperty<System.String>(p => p.BillToLastName, "Bill To Last Name");
        public System.String BillToLastName
        {
            get { return GetProperty(_billToLastNameProperty); }
            set{ SetProperty(_billToLastNameProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _shipToFirstNameProperty = RegisterProperty<System.String>(p => p.ShipToFirstName, "Ship To First Name");
        public System.String ShipToFirstName
        {
            get { return GetProperty(_shipToFirstNameProperty); }
            set{ SetProperty(_shipToFirstNameProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _shipToLastNameProperty = RegisterProperty<System.String>(p => p.ShipToLastName, "Ship To Last Name");
        public System.String ShipToLastName
        {
            get { return GetProperty(_shipToLastNameProperty); }
            set{ SetProperty(_shipToLastNameProperty, value); }
        }

        private static readonly PropertyInfo<System.Int32> _authorizationNumberProperty = RegisterProperty<System.Int32>(p => p.AuthorizationNumber, "Authorization Number");
        public System.Int32 AuthorizationNumber
        {
            get { return GetProperty(_authorizationNumberProperty); }
            set{ SetProperty(_authorizationNumberProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _localeProperty = RegisterProperty<System.String>(p => p.Locale, "Locale");
        public System.String Locale
        {
            get { return GetProperty(_localeProperty); }
            set{ SetProperty(_localeProperty, value); }
        }




        // OneToMany
        private static readonly PropertyInfo<LineItemList> _lineItemsProperty = RegisterProperty<LineItemList>(p => p.LineItems, Csla.RelationshipTypes.Child);
        public LineItemList LineItems
        {
            get
            {
                bool cancel = false;
                OnChildLoading(_lineItemsProperty, ref cancel);
    
                if (!cancel)
                {
                    if(!FieldManager.FieldExists(_lineItemsProperty))
                    {
                        var criteria = new PetShop.Tests.ObjF.StoredProcedures.LineItemCriteria {OrderId = OrderId};
                        
    
                        if(!PetShop.Tests.ObjF.StoredProcedures.LineItemList.Exists(criteria))
                            LoadProperty(_lineItemsProperty, PetShop.Tests.ObjF.StoredProcedures.LineItemList.NewList());
                        else
                            LoadProperty(_lineItemsProperty, PetShop.Tests.ObjF.StoredProcedures.LineItemList.GetByOrderId(OrderId));
                    }
                }

                return GetProperty(_lineItemsProperty);
            }
        }

        // OneToMany
        private static readonly PropertyInfo<OrderStatusList> _orderStatusesProperty = RegisterProperty<OrderStatusList>(p => p.OrderStatuses, Csla.RelationshipTypes.Child);
        public OrderStatusList OrderStatuses
        {
            get
            {
                bool cancel = false;
                OnChildLoading(_orderStatusesProperty, ref cancel);
    
                if (!cancel)
                {
                    if(!FieldManager.FieldExists(_orderStatusesProperty))
                    {
                        var criteria = new PetShop.Tests.ObjF.StoredProcedures.OrderStatusCriteria {OrderId = OrderId};
                        
    
                        if(!PetShop.Tests.ObjF.StoredProcedures.OrderStatusList.Exists(criteria))
                            LoadProperty(_orderStatusesProperty, PetShop.Tests.ObjF.StoredProcedures.OrderStatusList.NewList());
                        else
                            LoadProperty(_orderStatusesProperty, PetShop.Tests.ObjF.StoredProcedures.OrderStatusList.GetByOrderId(OrderId));
                    }
                }

                return GetProperty(_orderStatusesProperty);
            }
        }


        #endregion

        #region Synchronous Root Factory Methods 

        /// <summary>
        /// Creates a new object of type <see cref="Order"/>. 
        /// </summary>
        /// <returns>Returns a newly instantiated collection of type <see cref="Order"/>.</returns>    
        public static Order NewOrder()
        {
            return DataPortal.Create<Order>();
        }

        /// <summary>
        /// Returns a <see cref="Order"/> object of the specified criteria. 
        /// </summary>
        /// <param name="orderId">No additional detail available.</param>
        /// <returns>A <see cref="Order"/> object of the specified criteria.</returns>
        public static Order GetByOrderId(System.Int32 orderId)
        {
            var criteria = new OrderCriteria {OrderId = orderId};
            
            
            return DataPortal.Fetch<Order>(criteria);
        }

        public static void DeleteOrder(System.Int32 orderId)
        {
                DataPortal.Delete<Order>(new OrderCriteria (orderId));
        }

        #endregion

        #region Asynchronous Root Factory Methods
        
        public static void NewOrderAsync(EventHandler<DataPortalResult<Order>> handler)
        {
            var dp = new DataPortal<Order>();
            dp.CreateCompleted += handler;
            dp.BeginCreate();
        }

        public static void GetByOrderIdAsync(System.Int32 orderId, EventHandler<DataPortalResult<Order>> handler)
        {
            var criteria = new OrderCriteria{OrderId = orderId};
            

            var dp = new DataPortal<Order>();
            dp.FetchCompleted += handler;
            dp.BeginFetch(criteria);
        }

        public static void DeleteOrderAsync(System.Int32 orderId, EventHandler<DataPortalResult<Order>> handler)
        {
            var criteria = new OrderCriteria{OrderId = orderId};
            

            var dp = new DataPortal<Order>();
            dp.DeleteCompleted += handler;
            dp.BeginDelete(criteria);
        }
        
        #endregion

        #region Synchronous Child Factory Methods 

        /// <summary>
        /// Creates a new object of type <see cref="Order"/>. 
        /// </summary>
        /// <returns>Returns a newly instantiated collection of type <see cref="Order"/>.</returns>
        internal static Order NewOrderChild()
        {
            return DataPortal.CreateChild<Order>();
        }

        /// <summary>
        /// Returns a <see cref="Order"/> object of the specified criteria. 
        /// </summary>
        /// <param name="orderId">No additional detail available.</param>
        /// <returns>A <see cref="Order"/> object of the specified criteria.</returns>

        internal static Order GetByOrderIdChild(System.Int32 orderId)
        {
            var criteria = new OrderCriteria {OrderId = orderId};
            

            return DataPortal.Fetch<Order>(criteria);
        }

        #endregion

        #region Asynchronous Child Factory Methods
        
        internal static void NewOrderChildAsync(EventHandler<DataPortalResult<Order>> handler)
        {
            DataPortal<Order> dp = new DataPortal<Order>();
            dp.CreateCompleted += handler;
            dp.BeginCreate();
        }

        internal static void GetByOrderIdChildAsync(System.Int32 orderId, EventHandler<DataPortalResult<Order>> handler)
        {
            var criteria = new OrderCriteria{ OrderId = orderId};
            
            
            // Mark as child?
            var dp = new DataPortal<Order>();
            dp.FetchCompleted += handler;
            dp.BeginFetch(criteria);
        }

        #endregion

        #region DataPortal partial methods

        /// <summary>
        /// Codesmith generated stub method that is called when creating the <see cref="Order"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object creation should proceed.</param>
        partial void OnCreating(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Order"/> object has been created. 
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// Codesmith generated stub method that is called when fetching the <see cref="Order"/> object. 
        /// </summary>
        /// <param name="criteria"><see cref="OrderCriteria"/> object containg the criteria of the object to fetch.</param>
        /// <param name="cancel">Value returned from the method indicating whether the object fetching should proceed.</param>
        partial void OnFetching(OrderCriteria criteria, ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Order"/> object has been fetched. 
        /// </summary>    
        partial void OnFetched();

        /// <summary>
        /// Codesmith generated stub method that is called when mapping the <see cref="Order"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object mapping should proceed.</param>
        partial void OnMapping(ref bool cancel);
 
        /// <summary>
        /// Codesmith generated stub method that is called when mapping the <see cref="Order"/> object. 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancel">Value returned from the method indicating whether the object mapping should proceed.</param>
        partial void OnMapping(SafeDataReader reader, ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Order"/> object has been mapped. 
        /// </summary>
        partial void OnMapped();

        /// <summary>
        /// Codesmith generated stub method that is called when inserting the <see cref="Order"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object insertion should proceed.</param>
        partial void OnInserting(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Order"/> object has been inserted. 
        /// </summary>
        partial void OnInserted();

        /// <summary>
        /// Codesmith generated stub method that is called when updating the <see cref="Order"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object creation should proceed.</param>
        partial void OnUpdating(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Order"/> object has been updated. 
        /// </summary>
        partial void OnUpdated();

        /// <summary>
        /// Codesmith generated stub method that is called when self deleting the <see cref="Order"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object self deletion should proceed.</param>
        partial void OnSelfDeleting(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Order"/> object has been deleted. 
        /// </summary>
        partial void OnSelfDeleted();

        /// <summary>
        /// Codesmith generated stub method that is called when deleting the <see cref="Order"/> object. 
        /// </summary>
        /// <param name="criteria"><see cref="OrderCriteria"/> object containg the criteria of the object to delete.</param>
        /// <param name="cancel">Value returned from the method indicating whether the object deletion should proceed.</param>
        partial void OnDeleting(OrderCriteria criteria, ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Order"/> object with the specified criteria has been deleted. 
        /// </summary>
        partial void OnDeleted();
        partial void OnChildLoading(Csla.Core.IPropertyInfo childProperty, ref bool cancel);

        #endregion

        #region ChildPortal partial methods


        /// <summary>
        /// Codesmith generated stub method that is called when creating the child <see cref="Order"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object creation should proceed.</param>
        partial void OnChildCreating(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the child <see cref="Order"/> object has been created. 
        /// </summary>
        partial void OnChildCreated();

        /// <summary>
        /// Codesmith generated stub method that is called when fetching the child <see cref="Order"/> object. 
        /// </summary>
        /// <param name="criteria"><see cref="OrderCriteria"/> object containg the criteria of the object to fetch.</param>
        /// <param name="cancel">Value returned from the method indicating whether the object fetching should proceed.</param>
        partial void OnChildFetching(OrderCriteria criteria, ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the child <see cref="Order"/> object has been fetched. 
        /// </summary>
        partial void OnChildFetched();

        /// <summary>
        /// Codesmith generated stub method that is called when inserting the child <see cref="Order"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object insertion should proceed.</param>
        partial void OnChildInserting(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called when inserting the child <see cref="Order"/> object. 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cancel">Value returned from the method indicating whether the object insertion should proceed.</param>
        partial void OnChildInserting(SqlConnection connection, ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the child <see cref="Order"/> object has been inserted. 
        /// </summary>
        partial void OnChildInserted();

        /// <summary>
        /// Codesmith generated stub method that is called when updating the child <see cref="Order"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object creation should proceed.</param>
        partial void OnChildUpdating(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called when updating the child <see cref="Order"/> object. 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cancel">Value returned from the method indicating whether the object creation should proceed.</param>
        partial void OnChildUpdating(SqlConnection connection, ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the child <see cref="Order"/> object has been updated. 
        /// </summary>
        partial void OnChildUpdated();

        /// <summary>
        /// Codesmith generated stub method that is called when self deleting the child <see cref="Order"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object self deletion should proceed.</param>
        partial void OnChildSelfDeleting(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the child <see cref="Order"/> object has been deleted. 
        /// </summary>
        partial void OnChildSelfDeleted();
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



                if (FieldManager.FieldExists(_lineItemsProperty) && LineItems.IsDirty) return true;
                if (FieldManager.FieldExists(_orderStatusesProperty) && OrderStatuses.IsDirty) return true;

                return false;
            }
        }

        #endregion


        #region Exists Command

        /// <summary>
        /// Determines if a record exists in the Orders table in the database for the specified criteria. 
        /// </summary>
        /// <param name="criteria">The criteria parameter is an <see cref="Order"/> object.</param>
        /// <returns>A boolean value of true is returned if a record is found.</returns>
        public static bool Exists(OrderCriteria criteria)
        {
            return PetShop.Tests.ObjF.StoredProcedures.ExistsCommand.Execute(criteria);
        }

        /// <summary>
        /// Determines if a record exists in the Orders table in the database for the specified criteria. 
        /// </summary>
        public static void ExistsAsync(OrderCriteria criteria, EventHandler<DataPortalResult<ExistsCommand>> handler)
        {
            PetShop.Tests.ObjF.StoredProcedures.ExistsCommand.ExecuteAsync(criteria, handler);
        }

        #endregion

    }
}