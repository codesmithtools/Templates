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
    public partial class Inventory
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/
        
        #region Metadata
        
        internal class Metadata
        {
            // Only Attributes in the class will be preserved.

            [Required]
            [StringLength(10)]
            public string ItemId { get; set; }

            public int Qty { get; set; }

        }
        
        #endregion
    }
}

