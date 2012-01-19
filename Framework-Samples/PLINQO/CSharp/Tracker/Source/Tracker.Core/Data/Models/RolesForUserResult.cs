using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace Tracker.Core.Data
{
    public partial class RolesForUserResult
    {
        // Place custom code here.

        #region Metadata
        // For more information about how to use the metadata class visit:
        // http://www.plinqo.com/metadata.ashx
        [CodeSmith.Data.Audit.Audit]
        internal class Metadata
        {
             // WARNING: Only attributes inside of this class will be preserved.

            public int Id { get; set; }

            [Required]
            public string Name { get; set; }

            public string Description { get; set; }

            [Now(EntityState.New)]
            [CodeSmith.Data.Audit.NotAudited]
            public System.DateTime CreatedDate { get; set; }

            [Now(EntityState.Dirty)]
            [CodeSmith.Data.Audit.NotAudited]
            public System.DateTime ModifiedDate { get; set; }

            public System.Data.Linq.Binary RowVersion { get; set; }

            public int UserId { get; set; }

            public int RoleId { get; set; }

        }

        #endregion
    }
}