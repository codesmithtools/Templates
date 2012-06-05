using System;

using System.Collections.Generic;
using Petshop.Data.Entities;

namespace PetShop.UI.Controls
{
    public partial class CartList : System.Web.UI.UserControl
    {
        /// <summary>
        /// Bind control
        /// </summary>
        public void Bind(List<Cart> cart)
        {
            if (cart != null)
            {
                repOrdered.DataSource = cart;
                repOrdered.DataBind();
            }

        }
    }
}