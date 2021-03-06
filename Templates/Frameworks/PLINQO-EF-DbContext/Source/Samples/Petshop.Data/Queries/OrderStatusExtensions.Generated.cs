﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a CodeSmith Template.
//
//     DO NOT MODIFY contents of this file. Changes to this
//     file will be lost if the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Petshop.Data.Queries
{
    public static partial class OrderStatusExtensions
    {

        /// <summary>
        /// Gets an instance by the primary key.
        /// </summary>
        public static Petshop.Data.Entities.OrderStatus GetByKey(this System.Linq.IQueryable<Petshop.Data.Entities.OrderStatus> queryable, int orderId, int lineNum)
        {
            var dbSet = queryable as System.Data.Entity.IDbSet<Petshop.Data.Entities.OrderStatus>;
            if (dbSet != null)
                return dbSet.Find(orderId, lineNum);
                
            return queryable.FirstOrDefault(o => o.OrderId == orderId
                && o.LineNum == lineNum);
        }

        public static IQueryable<Petshop.Data.Entities.OrderStatus> ByOrderId(this IQueryable<Petshop.Data.Entities.OrderStatus> queryable, int orderId)
        {
            return queryable.Where(o => o.OrderId == orderId);
        }
    }
}
