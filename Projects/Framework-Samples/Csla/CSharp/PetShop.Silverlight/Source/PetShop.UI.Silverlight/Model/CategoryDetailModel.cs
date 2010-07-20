using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using PetShop.Business;

namespace PetShop.UI.Silverlight
{
    public class CategoryDetailModel : FrameworkElement
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(List<Category>), typeof(CategoryDetailModel), null);

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
