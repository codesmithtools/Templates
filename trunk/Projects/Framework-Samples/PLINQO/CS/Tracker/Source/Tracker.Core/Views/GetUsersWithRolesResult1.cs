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
    public partial class GetUsersWithRolesResult1
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

            public int Id { get; set; }

            [Required]
            [DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
            public string EmailAddress { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public System.Data.Linq.Binary Avatar { get; set; }

            [Now(EntityState.New)]
            [CodeSmith.Data.Audit.NotAudited]
            public System.DateTime CreatedDate { get; set; }

            [Now(EntityState.Dirty)]
            [CodeSmith.Data.Audit.NotAudited]
            public System.DateTime ModifiedDate { get; set; }

            public System.Data.Linq.Binary RowVersion { get; set; }

            [Required]
            [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
            public string PasswordHash { get; set; }

            [Required]
            [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
            public string PasswordSalt { get; set; }

            public string Comment { get; set; }

            public bool IsApproved { get; set; }

            public System.DateTime LastLoginDate { get; set; }

            public System.DateTime LastActivityDate { get; set; }

            public System.DateTime LastPasswordChangeDate { get; set; }

            public string AvatarType { get; set; }

        }

        #endregion
    }
}