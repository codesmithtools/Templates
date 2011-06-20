using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using CodeSmith.Data.Linq;
using NHibernate.Linq;
using Petshop.Data;
using Petshop.Data.Entities;

namespace PetShop.Core.Utility
{
    public class CartHelper
    {
        public static void ClearCart(List<Cart> items)
        {
            using (var context = new PetshopDataContext())
            {
                context.Cart.AttachAll(items);
                context.Cart.DeleteAllOnSubmit(items);
                context.SubmitChanges();
            }
        }

        public static void SetQuantity(List<Cart> items, string itemId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ItemId == itemId);
            using (var context = new PetshopDataContext())
            {
                context.Cart.Attach(item);
                item.Quantity = quantity;
                context.SubmitChanges();
            }
        }

        public static void Add(List<Cart> items, string itemId, int uniqueId, bool isShoppingCart)
        { 
            int index = 0;
            bool found = false;
            using (var context = new PetshopDataContext())
            {
                /*var options = new DataLoadOptions();
                options.LoadWith<Item>(i => i.Product);*/

                var item = items.FirstOrDefault(i => i.ItemId == itemId);

                if (item != null)
                {
                    context.Cart.Attach(item);
                    item.Quantity++;

                }
                else
                {
                    var cartItem = context.Item
                        .ByItemId(itemId)
                        .Fetch(i => i.Product)
                        .FirstOrDefault();
                    var profile = context.Profile.GetByKey(uniqueId);

                    var cart = new Cart();
                    cart.Profile = profile;
                    cart.ItemId = itemId;
                    cart.Name = cartItem.Name;
                    cart.ProductId = cartItem.Product.ProductId; // HERE
                    cart.IsShoppingCart = isShoppingCart;
                    cart.Price = cartItem.ListPrice ?? cartItem.UnitCost ?? 0;
                    cart.Type = cartItem.Product.Name;
                    cart.CategoryId = cartItem.Product.Category.CategoryId; // HERE
                    cart.Quantity = 1;
                    items.Add(cart);

                    context.Cart.InsertOnSubmit(cart);
                }
                context.SubmitChanges();
            }
        }

        public static decimal GetTotal(List<Cart> items)
        {
            decimal total = 0;
            items.ForEach(c => total += (c.Price * c.Quantity));
            return total;
        }

        public static void MoveToCart(Profile profile, string itemId)
        {
            var item = profile.WishList.FirstOrDefault(i => i.ItemId == itemId);

            using (var context = new PetshopDataContext())
            {
                context.Cart.Attach(item);
                item.IsShoppingCart = true;
                context.SubmitChanges();
            }
            profile.WishList.Remove(item);
            profile.ShoppingCart.Add(item);

        }

        public static void MoveToWishList(Profile profile, string itemId)
        {
            var item = profile.ShoppingCart.FirstOrDefault(i => i.ItemId == itemId);

            using (var context = new PetshopDataContext())
            {
                context.Cart.Attach(item);
                item.IsShoppingCart = false;
                context.SubmitChanges();
            }
            profile.WishList.Remove(item);
            profile.ShoppingCart.Add(item);

        }

        public static void Remove(List<Cart> items, string itemId)
        {
            var item = items.FirstOrDefault(i => i.ItemId == itemId);

            using (var context = new PetshopDataContext())
            {
                var cart = context.Cart.GetByKey(item.CartId);
                context.Cart.DeleteOnSubmit(cart);
                context.SubmitChanges();
                //context.Cart.Delete(item.CartId);
            }
            items.Remove(item);
        }

        public static void SaveOrderLineItems(List<Cart> cart, int orderId)
        {
            int lineNum = 0;

            using (var context = new PetshopDataContext())
            {
                var order = context.Order.GetByKey(orderId);
                foreach (var item in cart)
                {
                    var lineItem = new LineItem
                    {
                        Order = order,
                        ItemId = item.ItemId,
                        LineNum = ++lineNum,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price,
                    };
                    context.LineItem.InsertOnSubmit(lineItem);
                }
                context.SubmitChanges();
            }
        }
    }
}
