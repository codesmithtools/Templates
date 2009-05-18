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
    public partial class Audit
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/

        // 简化字; traditional Chinese: 
        // 簡化字; pinyin: Jiǎnhuàzì or simplified Chinese: 
        // 简体字; traditional Chinese: 
        // 簡體字; pinyin: Jiǎntǐzì) 

        #region Metadata


        private class Metadata
        {
            // Only Attributes in the class will be preserved.

            public int Id { get; set; }

            [Required]
            public string Source { get; set; }

            [Required]
            public string AuditXml { get; set; }

            [Now(EntityState.New)]
            public System.DateTime CreatedDate { get; set; }

            [Now(EntityState.Dirty)]
            public System.DateTime ModifiedDate { get; set; }

            public System.Data.Linq.Binary RowVersion { get; set; }

        }

        #endregion
    }
}