using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using Petshop.Data;
using Petshop.Data.Entities;

namespace PetShop.UI.Controls
{
    public partial class ProductsControl : UserControl
    {
        /// <summary>
        /// Rebind control 
        /// </summary>
        protected void PageChanged(object sender, DataGridPageChangedEventArgs e)
        {
            //reset index
            productsList.CurrentPageIndex = e.NewPageIndex;

            //get category id
            string categoryId = Request.QueryString["categoryId"];

            //bind data(
            using (var context = new PetshopDataContext())
            {
                productsList.DataSource = context.Product.ByCategory(categoryId).ToList();
            }
            productsList.DataBind();
        }
    }
}