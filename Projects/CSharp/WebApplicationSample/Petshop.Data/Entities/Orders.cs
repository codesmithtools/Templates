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
    public partial class Orders
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

            [Required]
            [StringLength(20)]
            public string UserId { get; set; }

            public System.DateTime OrderDate { get; set; }

            [Required]
            [StringLength(80)]
            public string ShipAddr1 { get; set; }

            [StringLength(80)]
            public string ShipAddr2 { get; set; }

            [Required]
            [StringLength(80)]
            public string ShipCity { get; set; }

            [Required]
            [StringLength(80)]
            public string ShipState { get; set; }

            [Required]
            [StringLength(20)]
            public string ShipZip { get; set; }

            [Required]
            [StringLength(20)]
            public string ShipCountry { get; set; }

            [Required]
            [StringLength(80)]
            public string BillAddr1 { get; set; }

            [StringLength(80)]
            public string BillAddr2 { get; set; }

            [Required]
            [StringLength(80)]
            public string BillCity { get; set; }

            [Required]
            [StringLength(80)]
            public string BillState { get; set; }

            [Required]
            [StringLength(20)]
            public string BillZip { get; set; }

            [Required]
            [StringLength(20)]
            public string BillCountry { get; set; }

            [Required]
            [StringLength(80)]
            public string Courier { get; set; }

            public decimal TotalPrice { get; set; }

            [Required]
            [StringLength(80)]
            public string BillToFirstName { get; set; }

            [Required]
            [StringLength(80)]
            public string BillToLastName { get; set; }

            [Required]
            [StringLength(80)]
            public string ShipToFirstName { get; set; }

            [Required]
            [StringLength(80)]
            public string ShipToLastName { get; set; }

            public int AuthorizationNumber { get; set; }

            [Required]
            [StringLength(20)]
            public string Locale { get; set; }

            public EntitySet<LineItem> LineItemList { get; set; }

            public EntitySet<OrderStatus> OrderStatusList { get; set; }

        }
        
        #endregion
    }
}

