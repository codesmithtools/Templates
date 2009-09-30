using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace PetShop.Core.Data
{
    public partial class Cart
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

            public int UniqueID { get; set; }

            [Required]
            public string ItemId { get; set; }

            [Required]
            public string Name { get; set; }

            [Required]
            public string Type { get; set; }

            public decimal Price { get; set; }

            [Required]
            public string CategoryId { get; set; }

            [Required]
            public string ProductId { get; set; }

            public bool IsShoppingCart { get; set; }

            public int Quantity { get; set; }

            public int CartID { get; set; }

            public Profile Profile { get; set; }

        }

        #endregion
    }
}