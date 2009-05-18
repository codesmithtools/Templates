using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace Tester.Data
{
    public partial class GetUserFixedResult1
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/

        #region Metadata

        [CodeSmith.Data.Audit.Audit]
        private class Metadata
        {
            // Only Attributes in the class will be preserved.

            public int Id { get; set; }

            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
            public string EmailAddress { get; set; }

        }

        #endregion
    }
}