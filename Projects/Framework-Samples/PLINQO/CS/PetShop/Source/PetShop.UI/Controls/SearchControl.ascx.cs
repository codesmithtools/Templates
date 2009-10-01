using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using PetShop.Core.Data;

namespace PetShop.UI.Controls
{
    public partial class SearchControl : UserControl
    {
        /// <summary>
        /// Rebind control 
        /// </summary>
        protected void PageChanged(object sender, DataGridPageChangedEventArgs e)
        {
            //reset index
            searchList.CurrentPageIndex = e.NewPageIndex;

            //get category id
            string keywordKey = Request.QueryString["keywords"];

            var list = new List<Product>();
            using (var context = new PetShopDataContext())
            {
                list = context.Product.Search(keywordKey).ToList();
            }

            //bind data
            searchList.DataSource = list;
            searchList.DataBind();
        }
    }
}