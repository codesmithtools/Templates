using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Collections.Generic;
using System.Data.Linq;
using NHibernate.Linq;
using Petshop.Data;
using Petshop.Data.Entities;
using CodeSmith.Data.Linq;


namespace PetShop.UI.Controls
{
    public partial class ItemsControl : UserControl
    {
        protected Inventory GetInventory(string itemId)
        {
            Inventory inventory;
            using( var context = new PetshopDataContext())
            {
                inventory = context.Inventory.GetByKey(itemId);
                context.Inventory.Detach(inventory);
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

            using (var context = new PetshopDataContext())
            {
                /*var options = new DataLoadOptions();
                options.LoadWith<Item>(i => i.Product);
                context.LoadOptions = options;*/

                itemsGrid.DataSource = context.Item
                    .ByProduct(productId)
                    .Fetch(i => i.Product)
                    .ToList();
            }
            itemsGrid.DataBind();
        }
    }
}