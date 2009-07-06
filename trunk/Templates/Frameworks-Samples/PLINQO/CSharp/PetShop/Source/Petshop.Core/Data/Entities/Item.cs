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
    public partial class Item
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

            [Required]
            public string ItemId { get; set; }

            [Required]
            public string ProductId { get; set; }

            public decimal ListPrice { get; set; }

            public decimal UnitCost { get; set; }

            public int Supplier { get; set; }

            public string Status { get; set; }

            public string Name { get; set; }

            public string Image { get; set; }

            public Product Product { get; set; }

            public Supplier Supplier1 { get; set; }

        }

        #endregion
    }
}