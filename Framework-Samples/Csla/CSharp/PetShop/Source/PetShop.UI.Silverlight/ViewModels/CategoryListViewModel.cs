using System;
using System.Threading.Tasks;
using System.Windows;
using Csla.Xaml;
using PetShop.Business;

namespace PetShop.UI.Silverlight.ViewModels
{
    public class CategoryListViewModel : ViewModel<CategoryList>
    {
        protected async override Task<CategoryList> DoInitAsync()
        {
            return await CategoryList.GetAllAsync();
        }

        protected override void OnError(Exception error)
        {
            Bxf.Shell.Instance.ShowError(error.Message, "Error");
        }

        #region Properties

        /// <summary>
        /// Gets or sets the SelectedData object.
        /// </summary>
        public static readonly DependencyProperty SelectedDataProperty = DependencyProperty.Register("SelectedData", typeof(Category), typeof(CategoryListViewModel), new PropertyMetadata(null));
        
        /// <summary>
        /// Gets or sets the SelectedData object.
        /// </summary>
        public Category SelectedData
        {
            get { return (Category)GetValue(SelectedDataProperty); }
            set { SetValue(SelectedDataProperty, value); OnPropertyChanged("SelectedData"); }
        }

        /// <summary>
        /// Gets or sets the SelectedItems object.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(System.Collections.ObjectModel.ObservableCollection<object>), typeof(CategoryListViewModel), new PropertyMetadata(null));
        
        /// <summary>
        /// Gets or sets the SelectedItems object.
        /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<object> SelectedItems
        {
            get { return (System.Collections.ObjectModel.ObservableCollection<object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); OnPropertyChanged("SelectedItems"); }
        }


        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(CategoryDetailModel), null);
        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        #endregion
    }
}
