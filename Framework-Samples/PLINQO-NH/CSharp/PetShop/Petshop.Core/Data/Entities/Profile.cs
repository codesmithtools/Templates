using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using Petshop.Data;

namespace Petshop.Data.Entities
{
	public partial class Profile
    {
        List<Cart> _ShoppingCart;
        public virtual List<Cart> ShoppingCart
        {
            get
            {
                if (null == _ShoppingCart)
                    using (var context = new PetshopDataContext {ObjectTrackingEnabled = false})
                        _ShoppingCart = context.Cart
                            .GetCart(UniqueID, true)
                            .ToList();

                return _ShoppingCart;
            }
        }

        List<Cart> _WishList;
        public virtual List<Cart> WishList
        {
            get
            {
                if (null == _WishList)
                    using (var context = new PetshopDataContext {ObjectTrackingEnabled = false})
                        _WishList = context.Cart
                            .GetCart(UniqueID, false)
                            .ToList();

                return _WishList;
            }
        }
	}
}