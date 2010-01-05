using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetShop.Core.Data;
using System.Data.Linq;

namespace PetShop.Core.Utility
{
    public class CartHelper
    {

        public static void ClearCart(List<Cart> items)
        {
            using (var context = new PetShopDataContext())
            {
                context.Cart.AttachAll(items);
                context.Cart.DeleteAllOnSubmit(items);
                context.SubmitChanges();
            }
        }

        public static void SetQuantity(List<Cart> items, string itemId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ItemId == itemId);
            using (var context = new PetShopDataContext())
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
            using (var context = new PetShopDataContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<Item>(i => i.Product);

                var item = items.FirstOrDefault(i => i.ItemId == itemId);

                if (item != null)
                {
                    context.Cart.Attach(item);
                    item.Quantity++;

                }
                else
                {

                    var cartItem = context.Item.GetByKey(itemId);
                    var cart = new Cart();
                    cart.UniqueID = uniqueId;
                    cart.ItemId = itemId;
                    cart.Name = cartItem.Name;
                    cart.ProductId = cartItem.ProductId;
                    cart.IsShoppingCart = isShoppingCart;
                    cart.Price = cartItem.ListPrice ?? cartItem.UnitCost ?? 0;
                    cart.Type = cartItem.Product.Name;
                    cart.CategoryId = cartItem.Product.CategoryId;
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

            using (var context = new PetShopDataContext())
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

            using (var context = new PetShopDataContext())
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

            using (var context = new PetShopDataContext())
            {
                context.Cart.Delete(item.CartId);
            }
            items.Remove(item);
        }

        public static void SaveOrderLineItems(List<Cart> cart, int orderId)
        {
            int lineNum = 0;

            using (var context = new PetShopDataContext())
            {
                foreach (var item in cart)
                {
                    var lineItem = new LineItem();
                    lineItem.OrderId = orderId;
                    lineItem.ItemId = item.ItemId;
                    lineItem.LineNum = ++lineNum;
                    lineItem.Quantity = item.Quantity;
                    lineItem.UnitPrice = item.Price;
                    context.LineItem.InsertOnSubmit(lineItem);
                }
                context.SubmitChanges();
            }
        }


    }
}
