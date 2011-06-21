using System;
using System.Linq;
using Petshop.Data;
using Petshop.Data.Entities;

namespace Petshop.Data.Entities
{
    public static partial class CartExtensions
    {
        public static IQueryable<Cart> GetCart(this IQueryable<Cart> queryable, int uniqueID, bool isShoppingCart)
        {
            return queryable
                .ByIsShoppingCart(isShoppingCart);
        }

        #region Query

        // A private class for lazy loading static compiled queries.
        private static partial class Query
        {
            // Place custom compiled queries here. 
        } 

        #endregion
    }
}