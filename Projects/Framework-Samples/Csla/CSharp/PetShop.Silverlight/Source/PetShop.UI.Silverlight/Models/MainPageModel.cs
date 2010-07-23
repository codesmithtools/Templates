using System;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.UI.Silverlight
{
    public class MainPageModel : FrameworkElement
    {
        public static readonly DependencyProperty CurrentControlProperty = DependencyProperty.Register("CurrentControl", typeof(UserControl), typeof(MainPageModel), new PropertyMetadata(null));

        public UserControl CurrentControl
        {
            get { return (UserControl)GetValue(CurrentControlProperty); }
            set { SetValue(CurrentControlProperty, value); }
        }

        private static MainPageModel _main;
        public MainPageModel()
        {
            _main = this;
            CurrentControl = new CategoryListPage();
        }

        public static void ShowForm(UserControl form)
        {
            _main.CurrentControl = form;
        }
    }
}
