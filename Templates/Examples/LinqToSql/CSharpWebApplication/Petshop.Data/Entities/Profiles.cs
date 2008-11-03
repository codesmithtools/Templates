using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace Petshop.Data
{
    public partial class Profiles
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/
        
        #region Metadata
        
        private class Metadata
        {
            // Only Attributes in the class will be preserved.
            
            public int UniqueID { get; set; }
            
            [Required]
            [StringLength(256)]
            public string Username { get; set; }
            
            [Required]
            [StringLength(256)]
            public string ApplicationName { get; set; }
            
            public bool IsAnonymous { get; set; }
            
            public System.DateTime LastActivityDate { get; set; }
            
            public System.DateTime LastUpdatedDate { get; set; }
            
        }
        
        #endregion
    }
}

