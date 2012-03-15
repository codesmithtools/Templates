﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v6.0.3, CSLA Templates: v4.0.0.0, CSLA Framework: v4.3.10.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'ItemList.cs'.
//
//     Template: EditableChildList.Generated.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

using Csla;
using Csla.Data;

namespace PetShop.Tests.ObjF.StoredProcedures
{
    [Serializable]
    [Csla.Server.ObjectFactory(FactoryNames.ItemListFactoryName)]
    public partial class ItemList : BusinessListBase<ItemList, Item>
    {
        #region Constructor(s)

        public ItemList()
        { 
            AllowNew = true;
            MarkAsChild();
        }
        
        #endregion

        #region Synchronous Factory Methods 

        /// <summary>
        /// Creates a new object of type <see cref="ItemList"/>. 
        /// </summary>
        /// <returns>Returns a newly instantiated collection of type <see cref="ItemList"/>.</returns>
        internal static ItemList NewList()
        {
            return DataPortal.CreateChild<ItemList>();
        }     

        /// <summary>
        /// Returns a <see cref="ItemList"/> object of the specified criteria. 
        /// </summary>
        /// <param name="itemId">No additional detail available.</param>
        /// <returns>A <see cref="ItemList"/> object of the specified criteria.</returns>
        internal static ItemList GetByItemId(System.String itemId)
        {
            var criteria = new ItemCriteria{ItemId = itemId};
            
            
            return DataPortal.Fetch<ItemList>(criteria);
        }

        /// <summary>
        /// Returns a <see cref="ItemList"/> object of the specified criteria. 
        /// </summary>
        /// <param name="productId">No additional detail available.</param>
        /// <param name="itemId">No additional detail available.</param>
        /// <param name="listPrice">No additional detail available.</param>
        /// <param name="name">No additional detail available.</param>
        /// <returns>A <see cref="ItemList"/> object of the specified criteria.</returns>
        internal static ItemList GetByProductIdItemIdListPriceName(System.String productId, System.String itemId, System.Decimal? listPrice, System.String name)
        {
            var criteria = new ItemCriteria{ProductId = productId, ItemId = itemId, Name = name};
                            if(listPrice.HasValue) criteria.ListPrice = listPrice.Value;
            
            return DataPortal.Fetch<ItemList>(criteria);
        }

        /// <summary>
        /// Returns a <see cref="ItemList"/> object of the specified criteria. 
        /// </summary>
        /// <param name="productId">No additional detail available.</param>
        /// <returns>A <see cref="ItemList"/> object of the specified criteria.</returns>
        internal static ItemList GetByProductId(System.String productId)
        {
            var criteria = new ItemCriteria{ProductId = productId};
            
            
            return DataPortal.Fetch<ItemList>(criteria);
        }

        /// <summary>
        /// Returns a <see cref="ItemList"/> object of the specified criteria. 
        /// </summary>
        /// <param name="supplier">No additional detail available.</param>
        /// <returns>A <see cref="ItemList"/> object of the specified criteria.</returns>
        internal static ItemList GetBySupplier(System.Int32? supplier)
        {
            var criteria = new ItemCriteria{};
                            if(supplier.HasValue) criteria.Supplier = supplier.Value;
            
            return DataPortal.Fetch<ItemList>(criteria);
        }

        internal static ItemList GetByCriteria(ItemCriteria criteria)
        {
            return DataPortal.Fetch<ItemList>(criteria);
        }

        internal static ItemList GetAll()
        {
            return DataPortal.Fetch<ItemList>(new ItemCriteria());
        }

        #endregion

        #region Asynchronous Factory Methods

        internal static void NewListAsync(EventHandler<DataPortalResult<ItemList>> handler)
        {
            var dp = new DataPortal<ItemList>();
            dp.CreateCompleted += handler;
            dp.BeginCreate();
        }

        internal static void GetByItemIdAsync(System.String itemId, EventHandler<DataPortalResult<AsyncChildLoader<ItemList>>> handler)
        {
            var criteria = new ItemCriteria{ItemId = itemId};
            
            DataPortal.BeginFetch<AsyncChildLoader<ItemList>>(criteria,handler);
        }

        internal static void GetByProductIdItemIdListPriceNameAsync(System.String productId, System.String itemId, System.Decimal? listPrice, System.String name, EventHandler<DataPortalResult<AsyncChildLoader<ItemList>>> handler)
        {
            var criteria = new ItemCriteria{ProductId = productId, ItemId = itemId, Name = name};
                            if(listPrice.HasValue) criteria.ListPrice = listPrice.Value;
            DataPortal.BeginFetch<AsyncChildLoader<ItemList>>(criteria,handler);
        }

        internal static void GetByProductIdAsync(System.String productId, EventHandler<DataPortalResult<AsyncChildLoader<ItemList>>> handler)
        {
            var criteria = new ItemCriteria{ProductId = productId};
            
            DataPortal.BeginFetch<AsyncChildLoader<ItemList>>(criteria,handler);
        }

        internal static void GetBySupplierAsync(System.Int32? supplier, EventHandler<DataPortalResult<AsyncChildLoader<ItemList>>> handler)
        {
            var criteria = new ItemCriteria{};
                            if(supplier.HasValue) criteria.Supplier = supplier.Value;
            DataPortal.BeginFetch<AsyncChildLoader<ItemList>>(criteria,handler);
        }

        internal static void GetByCriteriaAsync(ItemCriteria criteria, EventHandler<DataPortalResult<ItemList>> handler)
        {  
            var dp = new DataPortal<ItemList>();
            dp.FetchCompleted += handler;
            dp.BeginFetch(criteria);
        }

        internal static void GetAllAsync(EventHandler<DataPortalResult<AsyncChildLoader<ItemList>>> handler)
        {
            DataPortal.BeginFetch<AsyncChildLoader<ItemList>>(new ItemCriteria(),handler);
        }

        #endregion

        #region Method Overrides
        
        protected override Item AddNewCore()
        {
            Item item = PetShop.Tests.ObjF.StoredProcedures.Item.NewItemChild();

            bool cancel = false;
            OnAddNewCore(ref item, ref cancel);
            if (!cancel)
            {
                // Check to see if someone set the item to null in the OnAddNewCore.
                if(item == null)
                    item = PetShop.Tests.ObjF.StoredProcedures.Item.NewItemChild();

                // Pass the parent value down to the child.
                //Product product = this.Parent as Product;
                //if(product != null)
                //    item.ProductId = product.ProductId;

                // Pass the parent value down to the child.
                //Supplier supplier = this.Parent as Supplier;
                //if(supplier != null)
                //    item.Supplier = supplier.SuppId;


                Add(item);
            }

            return item;
        }

        protected void AddNewCoreAsync(EventHandler<DataPortalResult<Item>> handler)
        {
            PetShop.Tests.ObjF.StoredProcedures.Item.NewItemChildAsync((o, e) =>
            {
                if(e.Error == null)
                {
                    Add(e.Object);
                    handler.Invoke(this, new DataPortalResult<Item>(e.Object, null, null));
                }
            });
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
                foreach(Item item in this.Items)
                {
                    if(item.IsDirty) return true;
                }
                
                return false;
            }
        }

        #endregion

        #region DataPortal partial methods

        /// <summary>
        /// Codesmith generated stub method that is called when creating the child <see cref="Item"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object creation should proceed.</param>
        partial void OnCreating(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the child <see cref="Item"/> object has been created. 
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// Codesmith generated stub method that is called when fetching the child <see cref="Item"/> object. 
        /// </summary>
        /// <param name="criteria"><see cref="ItemCriteria"/> object containg the criteria of the object to fetch.</param>
        /// <param name="cancel">Value returned from the method indicating whether the object fetching should proceed.</param>
        partial void OnFetching(ItemCriteria criteria, ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the child <see cref="Item"/> object has been fetched. 
        /// </summary>
        partial void OnFetched();

        /// <summary>
        /// Codesmith generated stub method that is called when mapping the child <see cref="Item"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object mapping should proceed.</param>
        partial void OnMapping(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called when mapping the child <see cref="Item"/> object. 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancel">Value returned from the method indicating whether the object mapping should proceed.</param>
        partial void OnMapping(SafeDataReader reader, ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the child <see cref="Item"/> object has been mapped. 
        /// </summary>
        partial void OnMapped();

        /// <summary>
        /// Codesmith generated stub method that is called when updating the <see cref="Item"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object creation should proceed.</param>
        partial void OnUpdating(ref bool cancel);

        /// <summary>
        /// Codesmith generated stub method that is called after the <see cref="Item"/> object has been updated. 
        /// </summary>
        partial void OnUpdated();
        partial void OnAddNewCore(ref Item item, ref bool cancel);

        #endregion
        #region Exists Command

        /// <summary>
        /// Determines if a record exists in the Item in the database for the specified criteria. 
        /// </summary>
        /// <param name="criteria">The criteria parameter is a <see cref="ItemList"/> object.</param>
        /// <returns>A boolean value of true is returned if a record is found.</returns>
        public static bool Exists(ItemCriteria criteria)
        {
            return PetShop.Tests.ObjF.StoredProcedures.Item.Exists(criteria);
        }

        /// <summary>
        /// Determines if a record exists in the Item in the database for the specified criteria. 
        /// </summary>
        public static void ExistsAsync(ItemCriteria criteria, EventHandler<DataPortalResult<ExistsCommand>> handler)
        {
            PetShop.Tests.ObjF.StoredProcedures.Item.ExistsAsync(criteria,handler);
        }

        #endregion
 
        #region Enhancements

        public Item GetItem(System.String itemId)
        {
            return this.FirstOrDefault(i => i.ItemId == itemId);
        }

        public bool Contains(System.String itemId)
        {
            return this.Count(i => i.ItemId == itemId) > 0;
        }
 
        public bool ContainsDeleted(System.String itemId)
        {
            return DeletedList.Count(i => i.ItemId == itemId) > 0;
        }
        
        public void Remove(System.String itemId)
        {
            var item = this.FirstOrDefault(i => i.ItemId == itemId);
            if (item != null)
                Remove(item);
        }

        #endregion
    }
}