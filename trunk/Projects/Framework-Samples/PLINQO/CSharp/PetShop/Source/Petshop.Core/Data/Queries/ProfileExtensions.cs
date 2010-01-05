using System;
using System.Data.Linq;
using System.Linq;

namespace PetShop.Core.Data
{
    public static partial class ProfileExtensions
    {
        //Add query extension methods here.

        /// <summary>
        /// Gets an instance by using a unique index.
        /// </summary>
        /// <returns>An instance of the entity or null if not found.</returns>
        public static PetShop.Core.Data.Profile GetProfile(this IQueryable<PetShop.Core.Data.Profile> queryable, string username)
        {
            var entity = queryable as System.Data.Linq.Table<PetShop.Core.Data.Profile>;
            if (entity != null && entity.Context.LoadOptions == null)
                return Query.GetByUsernameApplicationName.Invoke((PetShop.Core.Data.PetShopDataContext)entity.Context, username, PetShopConstants.APPLICATION_NAME);

            return queryable.FirstOrDefault(p => p.Username == username
                    && p.ApplicationName == PetShopConstants.APPLICATION_NAME);
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