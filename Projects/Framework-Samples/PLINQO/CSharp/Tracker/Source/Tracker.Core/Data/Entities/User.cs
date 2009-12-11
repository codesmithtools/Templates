using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;
using CodeSmith.Data.Rules.Validation;
using Tracker.Core.Rules;

namespace Tracker.Core.Data
{
    public partial class User
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/

        public string FullName
        {
            get { return String.Concat(this.FirstName, String.IsNullOrEmpty(this.FirstName) ? String.Empty : " ", this.LastName).Trim(); }
        }

        static partial void AddSharedRules()
        {
            CodeSmith.Data.Rules.RuleManager.AddShared<User>(new UserExistsRule());
        }

        #region Metadata

        [CodeSmith.Data.Audit.Audit]
        internal class Metadata
        {
            // Only Attributes in the class will be preserved.

            public int Id { get; set; }

            [DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
            public string EmailAddress { get; set; }

            [Required]
            public string FirstName { get; set; }

            [Required]
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

            [CodeSmith.Data.Audit.NotAudited]
            [Now(EntityState.Dirty)]
            public System.DateTime LastActivityDate { get; set; }

            public System.DateTime LastPasswordChangeDate { get; set; }

            public string AvatarType { get; set; }

            public EntitySet<Audit> AuditList { get; set; }

            public EntitySet<Task> AssignedTaskList { get; set; }

            public EntitySet<Task> CreatedTaskList { get; set; }

            public EntitySet<UserRole> UserRoleList { get; set; }

        }

        #endregion
    }
}