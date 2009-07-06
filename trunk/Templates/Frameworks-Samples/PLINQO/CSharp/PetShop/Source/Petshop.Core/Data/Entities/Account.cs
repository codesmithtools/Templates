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
    public partial class Account
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

            public int AccountId { get; set; }

            public int UniqueID { get; set; }

            [Required]
            [DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
            public string Email { get; set; }

            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required]
            public string Address1 { get; set; }

            public string Address2 { get; set; }

            [Required]
            public string City { get; set; }

            [Required]
            public string State { get; set; }

            [Required]
            public string Zip { get; set; }

            [Required]
            public string Country { get; set; }

            [DataType(System.ComponentModel.DataAnnotations.DataType.PhoneNumber)]
            public string Phone { get; set; }

            public Profile Profile { get; set; }

        }

        #endregion
    }
}