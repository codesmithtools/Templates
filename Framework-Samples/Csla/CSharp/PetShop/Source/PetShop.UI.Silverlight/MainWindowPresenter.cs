using System;
using System.Windows;
using System.Windows.Controls;
using Bxf;
using PetShop.UI.Silverlight.ViewModels;

namespace PetShop.UI.Silverlight
{
    public class MainWindowPresenter : DependencyObject
    {
        public MainWindowPresenter()
        {
            var presenter = (IPresenter) Shell.Instance;
            presenter.OnShowError += (message, title) => MessageBox.Show(message, title, MessageBoxButton.OK);
            presenter.OnShowStatus += (status) => { };
            presenter.OnShowView += (view, region) => { MainContent = view.ViewInstance; };

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    Initialize();
                }
                catch (Exception ex)
                {
                    Shell.Instance.ShowError(ex.Message, "Startup error");
                }
            }
        }

        private async void Initialize()
        {
            Shell.Instance.ShowView(typeof(CategoryListPage).AssemblyQualifiedName,
                                    "categoryListViewModelViewSource",
                                    await new CategoryListViewModel().InitAsync(), 
                                    "Main");
        }

        public static readonly DependencyProperty MainContentProperty = DependencyProperty.Register("MainContent", typeof (UserControl), typeof (MainWindowPresenter), null);

        public UserControl MainContent
        {
            get { return (UserControl) GetValue(MainContentProperty); }
            set { SetValue(MainContentProperty, value); }
        }
    }
}