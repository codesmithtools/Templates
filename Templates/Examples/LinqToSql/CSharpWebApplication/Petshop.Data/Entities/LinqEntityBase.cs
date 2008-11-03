
using System;
using System.ComponentModel;

namespace Petshop.Data
{
    /// <summary>
    /// A base class for Linq entities that implements notification events.
    /// </summary>
    public abstract partial class LinqEntityBase :
        INotifyPropertyChanging,
        INotifyPropertyChanged
    {
    }
}