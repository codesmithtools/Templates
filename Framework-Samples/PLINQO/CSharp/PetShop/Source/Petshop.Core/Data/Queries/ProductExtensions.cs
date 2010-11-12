using System;
using System.Data.Linq;
using System.Linq;

namespace PetShop.Core.Data
{
    public static partial class ProductExtensions
    {
        //Add query extension methods here.

        /// <summary>
        /// Gets a query for <see cref="Product.CategoryId"/>.
        /// </summary>
        public static IQueryable<PetShop.Core.Data.Product> Search(this IQueryable<PetShop.Core.Data.Product> queryable, string keywords)
        {
            return queryable.Where(p => p.Name.ToLower().Contains(keywords.ToLower()) || p.Descn.ToLower().Contains(keywords.ToLower()));
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