using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PetShop.Business;

namespace PetShop.UI.Silverlight
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            //BusyAnimation.IsRunning = true;
            ////BusyAnimation.IsEnabled = false;
            //CategoryList.GetAllAsync((o, e) =>
            //        {
            //            if (e.Error != null) throw e.Error;

            //            categoryListDataGrid.DataContext = e.Object;

            //            BusyAnimation.IsRunning = false;
            //        });
        }

        //private void Save_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void Delete_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void Create_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void ImportData_Click(object sender, RoutedEventArgs e)
        //{

        //}


        private void CslaDataProvider_DataChanged(object sender, EventArgs e)
        {
            if (((Csla.Xaml.CslaDataProvider)sender).Error != null)
            {
                int i = 0;
                //System.Windows.Browser.HtmlPage.Window.Alert(((Csla.Xaml.CslaDataProvider)sender).Error.Message);
            }

        }

        private void CslaDataProvider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Error" && ((Csla.Silverlight.CslaDataProvider)sender).Error != null)
            //  System.Windows.Browser.HtmlPage.Window.Alert(((Csla.Silverlight.CslaDataProvider)sender).Error.Message);

        }
    }
}
