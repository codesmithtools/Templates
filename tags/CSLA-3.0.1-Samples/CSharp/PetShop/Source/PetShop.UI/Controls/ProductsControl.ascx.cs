using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using PetShop.Business;

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

            //bind data
            productsList.DataSource = Category.GetByCategoryId(categoryId).Products;
            productsList.DataBind();
        }
    }
}