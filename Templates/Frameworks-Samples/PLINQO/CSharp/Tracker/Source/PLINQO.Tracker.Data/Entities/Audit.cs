using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;

namespace PLINQO.Tracker.Data
{
    public partial class Audit
    {
        // For more information about the features contained in this class, please visit our GoogleCode Wiki at...
        // http://code.google.com/p/codesmith/wiki/PLINQO
        // Also, you can watch our Video Tutorials at...
        // http://community.codesmithtools.com/

        public Audit(CodeSmith.Data.Audit.AuditLog log)
        {
            this.Date = log.Date;
            this.Content = log.ToXml();
            this.Username = log.Username;
            this.CreatedDate = System.DateTime.Now;
        }


        public string HtmlContent { get; set; }

        #region Metadata

        private class Metadata
        {
            // Only Attributes in the class will be preserved.

            public int Id { get; set; }

            [Now(EntityState.New)]
            [CodeSmith.Data.Audit.NotAudited]
            public System.DateTime CreatedDate { get; set; }

            public System.DateTime Date { get; set; }

            [Required]
            [UserName]
            public string Username { get; set; }

            [Required]
            public string Content { get; set; }

            public System.Data.Linq.Binary RowVersion { get; set; }

            public int UserId { get; set; }

            public int TaskId { get; set; }

            public Task Task { get; set; }

            public User User { get; set; }

        }

        #endregion
    }
}