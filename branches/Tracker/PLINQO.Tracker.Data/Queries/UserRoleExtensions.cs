using System;
using System.Data.Linq;
using System.Linq;

namespace PLINQO.Tracker.Data
{
    public static partial class UserRoleExtensions
    {
        //Add query extension methods here.

        public static UserRole GetUserRole(this IQueryable<UserRole> queryable, string role, int userId)
        {
            return queryable.FirstOrDefault(ur => ur.Role.Name == role && ur.UserId == userId);
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