using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;
using PLINQO.Tracker.Data.Rules;
using PLINQO.Tracker.Data.Rules.Attributes;

namespace PLINQO.Tracker.Data
{
    public partial class Task
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/

        static partial void AddSharedRules()
        {
            RuleManager.AddShared<Task>(new TaskAssignmentRule());
        }

        #region Metadata

        [CodeSmith.Data.Audit.Audit]
        internal class Metadata
        {
            // Only Attributes in the class will be preserved.

            public int Id { get; set; }

            [Required(ErrorMessage="Status is Required")]
            public int StatusId { get; set; }

            public Priority PriorityId { get; set; }

            [CurrentUserName(EntityState.New)]
            public int CreatedId { get; set; }

            [Required(ErrorMessage = "Summary is Required")]
            public string Summary { get; set; }

            [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
            public string Details { get; set; }

            public System.DateTime StartDate { get; set; }

            public System.DateTime DueDate { get; set; }

            public System.DateTime CompleteDate { get; set; }

            public int AssignedId { get; set; }

            [Now(EntityState.New)]
            [CodeSmith.Data.Audit.NotAudited]
            public System.DateTime CreatedDate { get; set; }

            [Now(EntityState.Dirty)]
            [CodeSmith.Data.Audit.NotAudited]
            public System.DateTime ModifiedDate { get; set; }

            public System.Data.Linq.Binary RowVersion { get; set; }

            [UserName(EntityState.Dirty)]
            [CodeSmith.Data.Audit.NotAudited]
            public string LastModifiedBy { get; set; }

            public Status Status { get; set; }

            public User AssignedUser { get; set; }

            public User CreatedUser { get; set; }

            public TaskExtended TaskExtended { get; set; }

            public EntitySet<Audit> AuditList { get; set; }

        }

        #endregion
    }
}