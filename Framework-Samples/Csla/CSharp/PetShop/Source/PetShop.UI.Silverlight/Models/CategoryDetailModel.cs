using System.Collections.Generic;
using System.Windows;
using PetShop.Business;

namespace PetShop.UI.Silverlight
{
    public class CategoryDetailModel : FrameworkElement
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(List<Category>), typeof(CategoryDetailModel), null);
        public List<Category> SelectedItems
        {
            get { return (List<Category>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public void Home()
        {
            MainPageModel.ShowForm(new CategoryListPage());
        }
    }
}
