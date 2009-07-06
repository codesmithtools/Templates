using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using PetShop.Core.Data;

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
            using (var context = new PetShopDataContext())
            {
                productsList.DataSource = context.Product.ByCategoryId(categoryId).ToList();
            }
            productsList.DataBind();
        }
    }
}