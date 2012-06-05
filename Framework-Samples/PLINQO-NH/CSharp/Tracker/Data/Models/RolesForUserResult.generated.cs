using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Tracker.Data;

namespace Tracker.Data.Entities
{
	public partial class RolesForUserResult
    {
        public RolesForUserResult(System.Int32 identification, System.String name, System.String description, System.DateTime createdDate, System.DateTime modifiedDate, System.Byte[] rowVersion, System.Int32 userId, System.Int32 roleId)
        {
            Identification = identification;
            Name = name;
            Description = description;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
            RowVersion = rowVersion;
            UserId = userId;
            RoleId = roleId;
        }
        
        public System.Int32 Identification { get; set; }
        
        public System.String Name { get; set; }
        
        public System.String Description { get; set; }
        
        public System.DateTime CreatedDate { get; set; }
        
        public System.DateTime ModifiedDate { get; set; }
        
        public System.Byte[] RowVersion { get; set; }
        
        public System.Int32 UserId { get; set; }
        
        public System.Int32 RoleId { get; set; }
	}
}