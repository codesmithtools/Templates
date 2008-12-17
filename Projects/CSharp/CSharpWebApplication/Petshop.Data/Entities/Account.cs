using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace Petshop.Data
{
    public partial class Account
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/
        
        #region Metadata
        
        private class Metadata
        {
            // Only Attributes in the class will be preserved.
            
            public int AccountID { get; set; }
            
            public int UniqueID { get; set; }
            
            [Required]
            [StringLength(80)]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
            
            [Required]
            [StringLength(80)]
            public string FirstName { get; set; }
            
            [Required]
            [StringLength(80)]
            public string LastName { get; set; }
            
            [Required]
            [StringLength(80)]
            public string Address1 { get; set; }
            
            [StringLength(80)]
            public string Address2 { get; set; }
            
            [Required]
            [StringLength(80)]
            public string City { get; set; }
            
            [Required]
            [StringLength(80)]
            public string State { get; set; }
            
            [Required]
            [StringLength(20)]
            public string Zip { get; set; }
            
            [Required]
            [StringLength(20)]
            public string Country { get; set; }
            
            [StringLength(20)]
            [DataType(DataType.PhoneNumber)]
            public string Phone { get; set; }
            
        }
        
        #endregion
    }
}

