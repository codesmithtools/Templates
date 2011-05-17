using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Tracker.Data;

namespace Tracker.Data.Entities
{
	public partial class Status
    {
        // This should be an Enum.
	    public const int NotStarted = 1;
        public const int InProgress = 2;
        public const int Completed = 3;
        public const int Waiting = 4;
        public const int Deferred = 5;
        public const int Done = 6;
    }
}