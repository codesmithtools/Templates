using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using PetShop.Business;

namespace PetShop.UI.Controls
{
    public partial class ItemsControl : UserControl
    {
        /// <summary>
        /// Rebind control 
        /// </summary>
        protected void PageChanged(object sender, DataGridPageChangedEventArgs e)
        {
            //reset index
            itemsGrid.CurrentPageIndex = e.NewPageIndex;

            //get category id
            string productId = Request.QueryString["productId"];

            itemsGrid.DataSource = Product.GetByProductId(productId).Items;
            itemsGrid.DataBind();
        }
    }
}