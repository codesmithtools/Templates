﻿
//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a CodeSmith Template.
//
//     DO NOT MODIFY contents of this file. Changes to this
//     file will be lost if the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;

namespace Tracker.Data
{
    /// <summary>
    /// The DataContext manager class for the Tracker database.
    /// </summary>
    public partial class TrackerDataContext
    {
        
        private Tracker.Data.TrackerDataManager _manager = null;
        
        /// <summary>
        /// Gets the data manager.
        /// </summary>
        /// <value>The data manager.</value>
        public Tracker.Data.TrackerDataManager Manager
        {
            get
            {
                if (_manager == null)
                    _manager = new Tracker.Data.TrackerDataManager(this);
                return _manager;
            }
        }
    }
}