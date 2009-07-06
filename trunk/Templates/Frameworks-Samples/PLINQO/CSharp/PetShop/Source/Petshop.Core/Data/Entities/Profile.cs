using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace PetShop.Core.Data
{
    public partial class Profile
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/

        List<Cart> _ShoppingCart;
        public List<Cart> ShoppingCart
        {
            get
            {
                if (null == _ShoppingCart)
                {
                    using (var context = new PetShopDataContext())
                    {
                        _ShoppingCart = context.Cart.GetCart(UniqueID, true).ToList();
                        foreach (var item in _ShoppingCart)
                            item.Detach();
                    }
                }
                return _ShoppingCart;
            }
        }

        List<Cart> _WishList;
        public List<Cart> WishList
        {
            get
            {
                if (null == _WishList)
                {
                    using (var context = new PetShopDataContext())
                    {
                        _WishList = context.Cart.GetCart(UniqueID, false).ToList();
                        foreach (var item in _WishList)
                            item.Detach();
                    }
                }
                return _WishList;
            }
        }

        #region Metadata

        [CodeSmith.Data.Audit.Audit]
        internal class Metadata
        {
            // Only Attributes in the class will be preserved.

            public int UniqueID { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            public string ApplicationName { get; set; }

            public bool IsAnonymous { get; set; }

            public System.DateTime LastActivityDate { get; set; }

            public System.DateTime LastUpdatedDate { get; set; }

            public EntitySet<Account> AccountList { get; set; }

            public EntitySet<Cart> CartList { get; set; }

        }

        #endregion
    }
}