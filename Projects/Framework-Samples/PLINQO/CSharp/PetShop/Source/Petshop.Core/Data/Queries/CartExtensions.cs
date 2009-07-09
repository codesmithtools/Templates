using System;
using System.Data.Linq;
using System.Linq;

namespace PetShop.Core.Data
{
    public static partial class CartExtensions
    {
        //Add query extension methods here.

        public static IQueryable<Cart> GetCart(this IQueryable<PetShop.Core.Data.Cart> queryable, int uniqueID, bool isShoppingCart)
        {
            return queryable.ByUniqueID(uniqueID).ByIsShoppingCart(isShoppingCart);
        }

        #region Query
        // A private class for lazy loading static compiled queries.
        private static partial class Query
        {
            // Add your compiled queries here. 
        } 
        #endregion
    }
}