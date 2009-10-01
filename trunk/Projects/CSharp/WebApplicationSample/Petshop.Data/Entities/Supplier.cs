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
    public partial class Supplier
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/
        
        #region Metadata
        
        internal class Metadata
        {
            // Only Attributes in the class will be preserved.

            public int SuppId { get; set; }

            [StringLength(80)]
            public string Name { get; set; }

            [Required]
            [StringLength(2)]
            public string Status { get; set; }

            [StringLength(80)]
            public string Addr1 { get; set; }

            [StringLength(80)]
            public string Addr2 { get; set; }

            [StringLength(80)]
            public string City { get; set; }

            [StringLength(80)]
            public string State { get; set; }

            [StringLength(5)]
            public string Zip { get; set; }

            [StringLength(40)]
            [DataType(DataType.PhoneNumber)]
            public string Phone { get; set; }

            public EntitySet<Item> ItemList { get; set; }

        }
        
        #endregion
    }
}

