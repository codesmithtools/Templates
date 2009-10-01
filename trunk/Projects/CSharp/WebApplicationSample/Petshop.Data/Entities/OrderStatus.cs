using System;
using System.Data.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace Petshop.Data
{
    public partial class OrderStatus
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/
        
        #region Metadata
        
        internal class Metadata
        {
            // Only Attributes in the class will be preserved.

            public int OrderId { get; set; }

            public int LineNum { get; set; }

            public System.DateTime Timestamp { get; set; }

            [Required]
            [StringLength(2)]
            public string Status { get; set; }

            public Orders Orders { get; set; }

        }
        
        #endregion
    }
}

