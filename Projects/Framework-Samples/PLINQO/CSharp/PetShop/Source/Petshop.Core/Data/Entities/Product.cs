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
    public partial class Product
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
            public string ProductId { get; set; }

            [Required]
            public string CategoryId { get; set; }

            public string Name { get; set; }

            public string Descn { get; set; }

            public string Image { get; set; }

            public Category Category { get; set; }

            public EntitySet<Item> ItemList { get; set; }

        }

        #endregion
    }
}