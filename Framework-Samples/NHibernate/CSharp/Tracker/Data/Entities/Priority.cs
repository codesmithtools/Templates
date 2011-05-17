using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Tracker.Data;

namespace Tracker.Data.Entities
{
	public partial class Priority
    {
        // This should be an Enum.
	    public const int High = 1;
	    public const int Normal = 2;
	    public const int Low = 3;
    }
}