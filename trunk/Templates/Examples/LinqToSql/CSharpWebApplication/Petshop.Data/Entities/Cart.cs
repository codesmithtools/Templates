using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace Petshop.Data
{
    public partial class Cart
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/
        
        #region Metadata
        
        private class Metadata
        {
            // Only Attributes in the class will be preserved.
            
            public int CartID { get; set; }
            
            public int UniqueID { get; set; }
            
            [Required]
            [StringLength(10)]
            public string ItemId { get; set; }
            
            [Required]
            [StringLength(80)]
            public string Name { get; set; }
            
            [Required]
            [StringLength(80)]
            public string Type { get; set; }
            
            public decimal Price { get; set; }
            
            [Required]
            [StringLength(10)]
            public string CategoryId { get; set; }
            
            [Required]
            [StringLength(10)]
            public string ProductId { get; set; }
            
            public bool IsShoppingCart { get; set; }
            
            public int Quantity { get; set; }
            
        }
        
        #endregion
    }
}

