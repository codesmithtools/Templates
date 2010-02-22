﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v5.2.1, CSLA Templates: v1.5.0.1413, CSLA Framework: v3.8.2.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'Item.cs'.
//
//     Template: SwitchableObject.Generated.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
#region Using declarations

using System;

using Csla;
using Csla.Data;
using Csla.Validation;

#endregion

namespace PetShop.Tests.ObjF.ParameterizedSQL
{
    [Serializable]
    [Csla.Server.ObjectFactory(FactoryNames.ItemFactoryName)]
    public partial class Item : BusinessBase< Item >
    {
        #region Contructor(s)

        private Item()
        { /* Require use of factory methods */ }

        internal Item(System.String itemId)
        {
            using(BypassPropertyChecks)
            {
                LoadProperty(_itemIdProperty, itemId);
            }
        }
        #endregion

        #region Validation Rules

        protected override void AddBusinessRules()
        {
            if(AddBusinessValidationRules())
                return;

            ValidationRules.AddRule(CommonRules.StringRequired, _itemIdProperty);
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(_itemIdProperty, 10));
            ValidationRules.AddRule(CommonRules.StringRequired, _productIdProperty);
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(_productIdProperty, 10));
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(_statusProperty, 2));
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(_nameProperty, 80));
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(_imageProperty, 80));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Used for optimistic concurrency.
        /// </summary>
        [NotUndoable]
        internal System.Byte[] Timestamp = new System.Byte[8];

        private static readonly PropertyInfo< System.String > _itemIdProperty = RegisterProperty< System.String >(p => p.ItemId);
		[System.ComponentModel.DataObjectField(true, false)]
        public System.String ItemId
        {
            get { return GetProperty(_itemIdProperty); }
            set{ SetProperty(_itemIdProperty, value); }
        }

        private static readonly PropertyInfo< System.String > _originalItemIdProperty = RegisterProperty< System.String >(p => p.OriginalItemId);
        /// <summary>
        /// Holds the original value for ItemId. This is used for non identity primary keys.
        /// </summary>
        internal System.String OriginalItemId
        {
            get { return GetProperty(_originalItemIdProperty); }
            set{ SetProperty(_originalItemIdProperty, value); }
        }

        private static readonly PropertyInfo< System.String > _productIdProperty = RegisterProperty< System.String >(p => p.ProductId);
        public System.String ProductId
        {
            get { return GetProperty(_productIdProperty); }
            set{ SetProperty(_productIdProperty, value); }
        }

        private static readonly PropertyInfo< System.Decimal? > _listPriceProperty = RegisterProperty< System.Decimal? >(p => p.ListPrice);
        public System.Decimal? ListPrice
        {
            get { return GetProperty(_listPriceProperty); }
            set{ SetProperty(_listPriceProperty, value); }
        }

        private static readonly PropertyInfo< System.Decimal? > _unitCostProperty = RegisterProperty< System.Decimal? >(p => p.UnitCost);
        public System.Decimal? UnitCost
        {
            get { return GetProperty(_unitCostProperty); }
            set{ SetProperty(_unitCostProperty, value); }
        }

        private static readonly PropertyInfo< System.Int32? > _supplierProperty = RegisterProperty< System.Int32? >(p => p.Supplier);
        public System.Int32? Supplier
        {
            get { return GetProperty(_supplierProperty); }
            set{ SetProperty(_supplierProperty, value); }
        }

        private static readonly PropertyInfo< System.String > _statusProperty = RegisterProperty< System.String >(p => p.Status);
        public System.String Status
        {
            get { return GetProperty(_statusProperty); }
            set{ SetProperty(_statusProperty, value); }
        }

        private static readonly PropertyInfo< System.String > _nameProperty = RegisterProperty< System.String >(p => p.Name);
        public System.String Name
        {
            get { return GetProperty(_nameProperty); }
            set{ SetProperty(_nameProperty, value); }
        }

        private static readonly PropertyInfo< System.String > _imageProperty = RegisterProperty< System.String >(p => p.Image);
        public System.String Image
        {
            get { return GetProperty(_imageProperty); }
            set{ SetProperty(_imageProperty, value); }
        }

        //AssociatedManyToOne
        private static readonly PropertyInfo< Product > _productMemberProperty = RegisterProperty< Product >(p => p.ProductMember, Csla.RelationshipTypes.Child);
        public Product ProductMember
        {
            get
            {
                if(!FieldManager.FieldExists(_productMemberProperty))
                {
                    if(IsNew || !PetShop.Tests.ObjF.ParameterizedSQL.Product.Exists(new PetShop.Tests.ObjF.ParameterizedSQL.ProductCriteria {ProductId = ProductId}))
                        LoadProperty(_productMemberProperty, PetShop.Tests.ObjF.ParameterizedSQL.Product.NewProduct());
                    else
                        LoadProperty(_productMemberProperty, PetShop.Tests.ObjF.ParameterizedSQL.Product.GetByProductId(ProductId));
                }

                return GetProperty(_productMemberProperty); 
            }
        }

        //AssociatedManyToOne
        private static readonly PropertyInfo< Supplier > _supplierMemberProperty = RegisterProperty< Supplier >(p => p.SupplierMember, Csla.RelationshipTypes.Child);
        public Supplier SupplierMember
        {
            get
            {
                if(!FieldManager.FieldExists(_supplierMemberProperty))
                {
                    if(IsNew || !PetShop.Tests.ObjF.ParameterizedSQL.Supplier.Exists(new PetShop.Tests.ObjF.ParameterizedSQL.SupplierCriteria {SuppId = Supplier.Value}))
                        LoadProperty(_supplierMemberProperty, PetShop.Tests.ObjF.ParameterizedSQL.Supplier.NewSupplier());
                    else
                        LoadProperty(_supplierMemberProperty, PetShop.Tests.ObjF.ParameterizedSQL.Supplier.GetBySuppId(Supplier.Value));
                }

                return GetProperty(_supplierMemberProperty); 
            }
        }


        #endregion

        #region Root Factory Methods 
        
        public static Item NewItem()
        {
            return DataPortal.Create< Item >();
        }

        public static Item GetByItemId(System.String itemId)
        {
            return DataPortal.Fetch< Item >(
                new ItemCriteria {ItemId = itemId});
        }

        public static Item GetByProductIdItemIdListPriceName(System.String productId, System.String itemId, System.Decimal? listPrice, System.String name)
        {
            return DataPortal.Fetch< Item >(
                new ItemCriteria {ProductId = productId, ItemId = itemId, ListPrice = listPrice.Value, Name = name});
        }

        public static Item GetByProductId(System.String productId)
        {
            return DataPortal.Fetch< Item >(
                new ItemCriteria {ProductId = productId});
        }

        public static Item GetBySupplier(System.Int32? supplier)
        {
            return DataPortal.Fetch< Item >(
                new ItemCriteria {Supplier = supplier.Value});
        }

        public static void DeleteItem(System.String itemId)
        {
                DataPortal.Delete(new ItemCriteria (itemId));
        }
        
        #endregion

        #region Child Factory Methods 
        
        internal static Item NewItemChild()
        {
            return DataPortal.CreateChild< Item >();
        }
        internal static Item GetByItemIdChild(System.String itemId)
        {
            return DataPortal.FetchChild< Item >(
                new ItemCriteria {ItemId = itemId});
        }
        internal static Item GetByProductIdItemIdListPriceNameChild(System.String productId, System.String itemId, System.Decimal? listPrice, System.String name)
        {
            return DataPortal.FetchChild< Item >(
                new ItemCriteria {ProductId = productId, ItemId = itemId, ListPrice = listPrice.Value, Name = name});
        }
        internal static Item GetByProductIdChild(System.String productId)
        {
            return DataPortal.FetchChild< Item >(
                new ItemCriteria {ProductId = productId});
        }
        internal static Item GetBySupplierChild(System.Int32? supplier)
        {
            return DataPortal.FetchChild< Item >(
                new ItemCriteria {Supplier = supplier.Value});
        }

        #endregion

        #region Exists Command

        public static bool Exists(ItemCriteria criteria)
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
                if (FieldManager.FieldExists(_productMemberProperty) && ProductMember.IsDirty) return true;
                if (FieldManager.FieldExists(_supplierMemberProperty) && SupplierMember.IsDirty) return true;

                return false;
            }
        }

        #endregion


    }
}