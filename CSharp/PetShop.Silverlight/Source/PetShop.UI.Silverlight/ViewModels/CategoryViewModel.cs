using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Csla.Xaml;

using PetShop.Business;

namespace PetShop.UI.Silverlight
{

    public class CategoryViewModel : ViewModel<Category>
    {
        /// <summary>
        /// Gets or sets the Test object.
        /// </summary>
        public static readonly DependencyProperty TestProperty = DependencyProperty.Register("Test", typeof(object), typeof(CategoryViewModel),
            new PropertyMetadata((o, e) =>
            {
                ((CategoryViewModel)o).Model = (Category)e.NewValue;
            }));
        
        /// <summary>
        /// Gets or sets the Test object.
        /// </summary>
        public object Test
        {
            get { return GetValue(TestProperty); }
            set { SetValue(TestProperty, value); }
        }
    }
}
