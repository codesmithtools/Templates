using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace Tracker.Data
{
    public partial class Guid
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/

        #region Metadata

        [CodeSmith.Data.Audit.Audit]
        internal class Metadata
        {
            // Only Attributes in the class will be preserved.

            public System.Guid Id { get; set; }

            public System.Guid AlternateId { get; set; }

        }

        #endregion
    }
}