
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Petshop.Data
{
    /// <summary>
    /// A base class for Linq entities that implements notification events.
    /// </summary>
    [Serializable]
    [DataContract( IsReference=true )]
    public abstract partial class LinqEntityBase :
        INotifyPropertyChanging,
        INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinqEntityBase"/> class.
        /// </summary>
        protected LinqEntityBase()
        { }

        #region Notification Events

        /// <summary>
        /// Implements a PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise the PropertyChanged event for a specific property.
        /// </summary>
        /// <param name="propertyName">Name of the property that has changed.</param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Implements a PropertyChanging event.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Raise the PropertyChanging event for a specific property.
        /// </summary>
        /// <param name="propertyName">Name of the property that is changing.</param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            
        }
        #endregion
    }
}