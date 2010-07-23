using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Csla.Xaml;

using PetShop.Business;

namespace PetShop.UI.Silverlight
{
    public class CategoryListViewModel : ViewModel<CategoryList>
    {
        #region Constructor

        public CategoryListViewModel()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

            this.PropertyChanged += (o, e) =>
              {
                  if (e.PropertyName == "Error" && Error != null)
                  {
                      MessageBox.Show(Error.ToString(), "Data error", MessageBoxButton.OK);
                  }
              };

            Load();
        }

        #endregion

        #region Methods

        public void Load()
        {
            BeginRefresh("GetAllAsync");
        }

        public void LoadByID(string id)
        {
            BeginRefresh("GetByCategoryIdAsync", id);
        }

        public void ShowItem(object sender, ExecuteEventArgs e)
        {
            SelectedData = (Category)e.MethodParameter;
        }

        public void ProcessItemsTrigger(object sender, ExecuteEventArgs e)
        {
            // copy selected items into known list type
            var selection = ((System.Collections.IEnumerable)e.MethodParameter).Cast<Category>().ToList();

            // display detail form
            var form = new CategoryDetailPage();
            var vm = form.Resources["ViewModel"] as CategoryDetailModel;
            if (vm != null)
                vm.SelectedItems = selection;
            
            MainPageModel.ShowForm(form);
        }

        public void ProcessItemsExecute(object sender, ExecuteEventArgs e)
        {
            // copy selected items to known list type
            var listBox = e.TriggerSource.Tag as ListBox;

            if (listBox != null)
            {
                var selection = listBox.SelectedItems.Cast<Category>().ToList();

                // process selection
                var form = new CategoryDetailPage();
                var vm = form.Resources["ViewModel"] as CategoryDetailModel;
                if (vm != null)
                    vm.SelectedItems = selection;

                MainPageModel.ShowForm(form);
            }
        }

        #endregion

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
