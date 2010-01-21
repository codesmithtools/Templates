using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace Tracker.Core.Data
{
    public partial class TaskExtended
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/

        #region Metadata

        [CodeSmith.Data.Audit.Audit]
        internal class Metadata
        {
             // WARNING: Only attributes inside of this class will be preserved.

            public int TaskId { get; set; }

            public string Browser { get; set; }

            public string Os { get; set; }

            [Now(EntityState.New)]
            [CodeSmith.Data.Audit.NotAudited]
            public System.DateTime CreatedDate { get; set; }

            [Now(EntityState.Dirty)]
            [CodeSmith.Data.Audit.NotAudited]
            public System.DateTime ModifiedDate { get; set; }

            public System.Data.Linq.Binary RowVersion { get; set; }

            public Task Task { get; set; }

        }

        #endregion
    }
}