using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace Petshop.Data
{
    public partial class Item
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/
        
        #region Metadata
        
        private class Metadata
        {
            // Only Attributes in the class will be preserved.
            
            [Required]
            [StringLength(10)]
            public string ItemId { get; set; }
            
            [Required]
            [StringLength(10)]
            public string ProductId { get; set; }
            
            public decimal ListPrice { get; set; }
            
            public decimal UnitCost { get; set; }
            
            public int Supplier { get; set; }
            
            [StringLength(2)]
            public string Status { get; set; }
            
            [StringLength(80)]
            public string Name { get; set; }
            
            [StringLength(80)]
            public string Image { get; set; }
            
        }
        
        #endregion
    }
}

