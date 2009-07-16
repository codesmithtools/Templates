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
    public partial class Contact
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/

        #region Metadata

        [CodeSmith.Data.Audit.Audit]
        internal new class Metadata
            : Person.Metadata
        {
            // Only Attributes in the class will be preserved.

            [DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
            public string EmailAddress { get; set; }

            [DataType(System.ComponentModel.DataAnnotations.DataType.PhoneNumber)]
            public string Phone { get; set; }

        }

        #endregion
    }
}