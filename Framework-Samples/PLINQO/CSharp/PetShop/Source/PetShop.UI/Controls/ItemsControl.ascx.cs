using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using PetShop.Core.Data;
using System.Collections.Generic;
using System.Data.Linq;


namespace PetShop.UI.Controls
{
    public partial class ItemsControl : UserControl
    {
        protected Inventory GetInventory(string itemId)
        {
            var inventory = new Inventory();
            using( var context = new PetShopDataContext())
            {
                inventory = context.Inventory.GetByKey(itemId);
                inventory.Detach();
            }
            return inventory;
        }

        /// <summary>
        /// Rebind control 
        /// </summary>
        protected void PageChanged(object sender, DataGridPageChangedEventArgs e)
        {
            //reset index
            itemsGrid.CurrentPageIndex = e.NewPageIndex;

            //get category id
            string productId = Request.QueryString["productId"];

            using (var context = new PetShopDataContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<Item>(i => i.Product);
                context.LoadOptions = options;

                itemsGrid.DataSource = context.Item.ByProductId(productId).ToList();
            }
            itemsGrid.DataBind();
        }
    }
}