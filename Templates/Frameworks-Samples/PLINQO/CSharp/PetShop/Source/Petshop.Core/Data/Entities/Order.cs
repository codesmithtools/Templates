using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;
using PetShop.Core.Model;

namespace PetShop.Core.Data
{
    public partial class Order
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/

        public CreditCard CreditCard { get; set; }

        #region Metadata

        [CodeSmith.Data.Audit.Audit]
        internal class Metadata
        {
            // Only Attributes in the class will be preserved.

            public int OrderId { get; set; }

            [Required]
            public string UserId { get; set; }

            public System.DateTime OrderDate { get; set; }

            [Required]
            public string ShipAddr1 { get; set; }

            public string ShipAddr2 { get; set; }

            [Required]
            public string ShipCity { get; set; }

            [Required]
            public string ShipState { get; set; }

            [Required]
            public string ShipZip { get; set; }

            [Required]
            public string ShipCountry { get; set; }

            [Required]
            public string BillAddr1 { get; set; }

            public string BillAddr2 { get; set; }

            [Required]
            public string BillCity { get; set; }

            [Required]
            public string BillState { get; set; }

            [Required]
            public string BillZip { get; set; }

            [Required]
            public string BillCountry { get; set; }

            [Required]
            public string Courier { get; set; }

            public decimal TotalPrice { get; set; }

            [Required]
            public string BillToFirstName { get; set; }

            [Required]
            public string BillToLastName { get; set; }

            [Required]
            public string ShipToFirstName { get; set; }

            [Required]
            public string ShipToLastName { get; set; }

            public int AuthorizationNumber { get; set; }

            [Required]
            public string Locale { get; set; }

            public EntitySet<LineItem> LineItemList { get; set; }

            public EntitySet<OrderStatus> OrderStatusList { get; set; }

        }

        #endregion
    }
}